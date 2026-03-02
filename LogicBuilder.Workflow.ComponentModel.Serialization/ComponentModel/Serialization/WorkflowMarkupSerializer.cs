namespace LogicBuilder.Workflow.ComponentModel.Serialization
{
    using LogicBuilder.ComponentModel.Design.Serialization;
    using LogicBuilder.Workflow.ComponentModel.Design;
    using LogicBuilder.Workflow.ComponentModel.Serialization.Factories;
    using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;
    using System;
    using System.CodeDom;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;

    #region Class WorkflowMarkupSerializer
    //Main serialization class for persisting the XOML
    [DefaultSerializationProvider(typeof(WorkflowMarkupSerializationProvider))]
    public class WorkflowMarkupSerializer
    {
        private readonly IDeserializeFromStringHelper deserializeFromStringHelper = DeserializeFromStringHelperFactory.Create(DependencyHelperFactory.Create());
        private readonly IObjectDeserializer objectDeserializer = ObjectDeserializerFactory.Create();
        private readonly IWorkflowMarkupSerializationHelper workflowMarkupSerializationHelper = WorkflowMarkupSerializationHelperFactory.Create();

        #region Public Methods
        public object Deserialize(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            DesignerSerializationManager designerSerializationManager = new();
            using (designerSerializationManager.CreateSession())
            {
                return Deserialize(designerSerializationManager, reader);
            }
        }

        public object Deserialize(IDesignerSerializationManager serializationManager, XmlReader reader)
        {
            if (serializationManager == null)
                throw new ArgumentNullException("serializationManager");
            if (reader == null)
                throw new ArgumentNullException("reader");

            WorkflowMarkupSerializationManager markupSerializationManager = serializationManager as WorkflowMarkupSerializationManager ?? new WorkflowMarkupSerializationManager(serializationManager);
            //string fileName = markupSerializationManager.Context[typeof(string)] as string ?? String.Empty;
            object obj = DeserializeXoml(markupSerializationManager, reader);

            return obj;
        }

        private object DeserializeXoml(WorkflowMarkupSerializationManager serializationManager, XmlReader xmlReader)
        {
            if (serializationManager == null)
                throw new ArgumentNullException("serializationManager");
            if (xmlReader == null)
                throw new ArgumentNullException("xmlReader");

            Object obj = null;
            //xmlReader.WhitespaceHandling = WhitespaceHandling.None;
            serializationManager.WorkflowMarkupStack.Push(xmlReader);

            try
            {
                // 
                while (xmlReader.Read() && xmlReader.NodeType != XmlNodeType.Element && xmlReader.NodeType != XmlNodeType.ProcessingInstruction) ;
                if (xmlReader.EOF)
                    return null;
                obj = DeserializeObject(serializationManager, xmlReader);

                // Read until the end of the xml stream i.e past the </XomlDocument> tag. 
                // If there are any exceptions log them as errors.
                while (xmlReader.Read() && !xmlReader.EOF) ;
            }
            catch (XmlException xmlException)
            {
                throw new WorkflowMarkupSerializationException(xmlException.Message, xmlException, xmlException.LineNumber, xmlException.LinePosition);
            }
            catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
            {
                throw CreateSerializationError(e, xmlReader);
            }
            finally
            {
                serializationManager.WorkflowMarkupStack.Pop();
            }

            return obj;
        }

        public void Serialize(XmlWriter writer, object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (writer == null)
                throw new ArgumentNullException("writer");

            DesignerSerializationManager designerSerializationManager = new();
            using (designerSerializationManager.CreateSession())
            {
                Serialize(designerSerializationManager, writer, obj);
            }
        }

        public void Serialize(IDesignerSerializationManager serializationManager, XmlWriter writer, object obj)
        {
            if (serializationManager == null)
                throw new ArgumentNullException("serializationManager");
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (writer == null)
                throw new ArgumentNullException("writer");

            WorkflowMarkupSerializationManager markupSerializationManager = serializationManager as WorkflowMarkupSerializationManager ?? new WorkflowMarkupSerializationManager(serializationManager);
            StringWriter xomlStringWriter = new(CultureInfo.InvariantCulture);
            XmlWriter xmlWriter = Helpers.CreateXmlWriter(xomlStringWriter);
            markupSerializationManager.WorkflowMarkupStack.Push(xmlWriter);
            markupSerializationManager.WorkflowMarkupStack.Push(xomlStringWriter);

            try
            {
                SerializeObject(markupSerializationManager, obj, xmlWriter);
            }
            finally
            {
                xmlWriter.Close();
                writer.WriteRaw(xomlStringWriter.ToString());
                writer.Flush();
                markupSerializationManager.WorkflowMarkupStack.Pop();
                markupSerializationManager.WorkflowMarkupStack.Pop();
            }
        }
        #endregion

        #region Protected Methods (Non-overridable)
        internal object DeserializeObject(WorkflowMarkupSerializationManager serializationManager, XmlReader reader)
        {
            return objectDeserializer.DeserializeObject(serializationManager, reader);
        }

        internal void SerializeObject(WorkflowMarkupSerializationManager serializationManager, object obj, XmlWriter writer)
        {
            if (serializationManager == null)
                throw new ArgumentNullException("serializationManager");
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (writer == null)
                throw new ArgumentNullException("writer");

            try
            {
                serializationManager.WorkflowMarkupStack.Push(writer);
                using (new SafeXmlNodeWriter(serializationManager, obj, null, XmlNodeType.Element))
                {
                    DictionaryEntry? entry = null;
                    if (serializationManager.WorkflowMarkupStack[typeof(DictionaryEntry)] != null)
                        entry = (DictionaryEntry)serializationManager.WorkflowMarkupStack[typeof(DictionaryEntry)];

                    // To handle the case when the key and value are same in the dictionary
                    bool key = entry.HasValue && ((!entry.Value.GetType().IsValueType && entry.Value.Key == entry.Value.Value && entry.Value.Value == obj) ||
                                                    (entry.Value.GetType().IsValueType && entry.Value.Key.Equals(entry.Value.Value) && entry.Value.Value.Equals(obj))) &&
                                                    serializationManager.SerializationStack.Contains(obj);
                    if (key || !serializationManager.SerializationStack.Contains(obj))
                    {
                        serializationManager.Context.Push(obj);
                        serializationManager.SerializationStack.Push(obj);
                        try
                        {
                            SerializeContents(serializationManager, obj, writer, key);
                        }
                        finally
                        {
                            Debug.Assert(serializationManager.Context.Current == obj, "Serializer did not remove an object it pushed into stack.");
                            serializationManager.Context.Pop();
                            serializationManager.SerializationStack.Pop();
                        }
                    }
                    else
                        throw new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerStackOverflow, obj.ToString(), obj.GetType().FullName), 0, 0);
                }
            }
            finally
            {
                serializationManager.WorkflowMarkupStack.Pop();
            }
        }

        internal void SerializeContents(WorkflowMarkupSerializationManager serializationManager, object obj, XmlWriter writer, bool dictionaryKey)
        {
            if (serializationManager == null)
                throw new ArgumentNullException("serializationManager");
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (writer == null)
                throw new ArgumentNullException("writer");

            WorkflowMarkupSerializer serializer;
            try
            {
                //Now get the serializer to persist the properties, if the serializer is not found then we dont serialize the properties
                serializer = serializationManager.GetSerializer(obj.GetType(), typeof(WorkflowMarkupSerializer)) as WorkflowMarkupSerializer;

            }
            catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
            {
                serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerThrewException, obj.GetType().FullName, e.Message), e));
                return;

            }

            if (serializer == null)
            {
                serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerNotAvailableForSerialize, obj.GetType().FullName)));
                return;
            }

            try
            {
                serializer.OnBeforeSerialize(serializationManager, obj);
            }
            catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
            {
                serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerThrewException, obj.GetType().FullName, e.Message), e));
                return;
            }
            //Need the SortedDictionary to keep serialization repeatable.
            SortedDictionary<string, object> allProperties = [];
            ArrayList complexProperties = [];

            List<PropertyInfo> properties = [];
            List<EventInfo> events = [];

            // Serialize the extended properties for primitive types also
            if (obj.GetType().IsPrimitive || obj.GetType() == typeof(string) || obj.GetType() == typeof(decimal) ||
                obj.GetType() == typeof(DateTime) || obj.GetType() == typeof(TimeSpan) || obj.GetType().IsEnum ||
                obj.GetType() == typeof(Guid))
            {
                if (obj.GetType() == typeof(char) || obj.GetType() == typeof(byte) ||
                    obj.GetType() == typeof(System.Int16) || obj.GetType() == typeof(decimal) ||
                    obj.GetType() == typeof(DateTime) || obj.GetType() == typeof(TimeSpan) ||
                    obj.GetType().IsEnum || obj.GetType() == typeof(Guid))
                {
                    //These non CLS-compliant are not supported in the XmlWriter 
                    if ((obj.GetType() != typeof(char)) || (char)obj != '\0')
                    {
                        //These non CLS-compliant are not supported in the XmlReader 
                        string stringValue;
                        if (obj.GetType() == typeof(DateTime))
                        {
                            stringValue = ((DateTime)obj).ToString("o", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            TypeConverter typeConverter = TypeDescriptor.GetConverter(obj.GetType());
                            stringValue = typeConverter != null && typeConverter.CanConvertTo(typeof(string))
                                ? typeConverter.ConvertTo(null, CultureInfo.InvariantCulture, obj, typeof(string)) as string
                                : Convert.ToString(obj, CultureInfo.InvariantCulture);
                        }

                        writer.WriteValue(stringValue);
                    }
                }
                else if (obj.GetType() == typeof(string))
                {
                    string attribValue = obj as string;
                    attribValue = attribValue?.Replace('\0', ' ') ?? "";
                    if (!(attribValue.StartsWith("{", StringComparison.Ordinal) && attribValue.EndsWith("}", StringComparison.Ordinal)))
                        writer.WriteValue(attribValue);
                    else
                        writer.WriteValue("{}" + attribValue);
                }
                else
                {
                    writer.WriteValue(obj);
                }
                // For Key properties, we don't want to get the extended properties
                if (!dictionaryKey)
                    properties.AddRange(serializationManager.GetExtendedProperties(obj));
            }
            else
            {
                // Serialize properties
                //We first get all the properties, once we have them all, we start distinguishing between
                //simple and complex properties, the reason for that is XmlWriter needs to write attributes
                //first and elements later

                // Dependency events are treated as the same as dependency properties.


                try
                {
                    properties.AddRange(serializer.GetProperties(serializationManager, obj));
                    // For Key properties, we don;t want to get the extended properties
                    if (!dictionaryKey)
                        properties.AddRange(serializationManager.GetExtendedProperties(obj));
                    events.AddRange(serializer.GetEvents(serializationManager, obj));
                }
                catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
                {
                    serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerThrewException, obj.GetType().FullName, e.Message), e));
                    return;
                }
            }

            foreach (PropertyInfo propInfo in properties.Where(p => p != null && !allProperties.ContainsKey(p.Name)))
            {
                // Do not serialize properties that have corresponding dynamic properties.
                allProperties.Add(propInfo.Name, propInfo);
            }

            foreach (EventInfo eventInfo in events.Where(e => e != null && !allProperties.ContainsKey(e.Name)))
            {
                // Do not serialize events that have corresponding dynamic properties.
                allProperties.Add(eventInfo.Name, eventInfo);
            }

            using (ContentProperty contentProperty = new(serializationManager, serializer, obj))
            {
                foreach (object propertyObj in allProperties.Values)
                {
                    string propertyName = String.Empty;
                    object propertyValue = null;
                    Type propertyInfoType = null;

                    try
                    {
                        if (propertyObj is PropertyInfo property)
                        {
                            // If the property has parameters we can not serialize it , we just move on.
                            ParameterInfo[] indexParameters = property.GetIndexParameters();
                            if (indexParameters != null && indexParameters.Length > 0)
                                continue;

                            propertyName = property.Name;
                            propertyValue = null;
                            if (property.CanRead)
                            {
                                propertyValue = property.GetValue(obj, null);
                            }
                            propertyInfoType = property.PropertyType;
                        }
                    }
                    catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
                    {
                        while (e is TargetInvocationException && e.InnerException != null)
                            e = e.InnerException;

                        serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerPropertyGetFailed, [propertyName, obj.GetType().FullName, e.Message])));
                        continue;
                    }

                    if (propertyObj is PropertyInfo propertyInfo && contentProperty.Property == propertyInfo)
                        continue;

                    Type propertyValueType = null;
                    if (propertyValue != null)
                    {
                        propertyValue = GetMarkupExtensionFromValue(propertyValue);
                        propertyValueType = propertyValue.GetType();
                    }
                    else if (propertyObj is PropertyInfo)
                    {
                        propertyValue = new NullExtension();
                        propertyValueType = propertyValue.GetType();
                        Attribute[] attributes = Attribute.GetCustomAttributes(propertyObj as PropertyInfo, typeof(DefaultValueAttribute), true);
                        if (attributes.Length > 0)
                        {
                            DefaultValueAttribute defaultValueAttr = attributes[0] as DefaultValueAttribute;
                            if (defaultValueAttr?.Value == null)
                                propertyValue = null;
                        }
                    }
                    if (propertyValue != null)
                        propertyValueType = propertyValue.GetType();

                    //Now get the serializer to persist the properties, if the serializer is not found then we dont serialize the properties
                    serializationManager.Context.Push(propertyObj);
                    WorkflowMarkupSerializer propValueSerializer = null;
                    try
                    {
                        propValueSerializer = serializationManager.GetSerializer(propertyValueType, typeof(WorkflowMarkupSerializer)) as WorkflowMarkupSerializer;
                    }
                    catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
                    {
                        serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerThrewException, obj.GetType().FullName, e.Message), e));
                        serializationManager.Context.Pop();
                        continue;
                    }
                    if (propValueSerializer == null)
                    {
                        serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerNotAvailableForSerialize, propertyValueType?.FullName ?? "")));
                        serializationManager.Context.Pop();
                        continue;
                    }

                    // ask serializer if we can serialize
                    try
                    {
                        if (propValueSerializer.ShouldSerializeValue(serializationManager, propertyValue))
                        {
                            //NOTE: THE FOLLOWING CONDITION ABOUT propertyInfoType != typeof(object) is VALID AS WE SHOULD NOT SERIALIZE A PROPERTY OF TYPE OBJECT TO STRING
                            //IF WE DO THAT THEN WE DO NOT KNOWN WHAT WAS THE TYPE OF ORIGINAL OBJECT AND SERIALIZER WONT BE ABLE TO GET THE STRING BACK INTO THE CORRECT TYPE,
                            //AS THE TYPE INFORMATION IS LOST
                            if (propValueSerializer.CanSerializeToString(serializationManager, propertyValue) && propertyInfoType != typeof(object))
                            {
                                using (new SafeXmlNodeWriter(serializationManager, obj, propertyObj, XmlNodeType.Attribute))
                                {
                                    //This is a work around to special case the markup extension serializer as it writes to the stream using writer
                                    if (propValueSerializer is MarkupExtensionSerializer)
                                    {
                                        propValueSerializer.SerializeToString(serializationManager, propertyValue);
                                    }
                                    else
                                    {
                                        string stringValue = propValueSerializer.SerializeToString(serializationManager, propertyValue);
                                        if (!string.IsNullOrEmpty(stringValue))
                                        {
                                            stringValue = stringValue.Replace('\0', ' ');
                                            if (propertyValue is MarkupExtension || !(stringValue.StartsWith("{", StringComparison.Ordinal) && stringValue.EndsWith("}", StringComparison.Ordinal)))
                                                writer.WriteString(stringValue);
                                            else
                                                writer.WriteString("{}" + stringValue);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                complexProperties.Add(propertyObj);
                            }
                        }
                    }
                    catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
                    {
                        serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerNoSerializeLogic, [propertyName, obj.GetType().FullName]), e));
                    }
                    finally
                    {
                        Debug.Assert(serializationManager.Context.Current == propertyObj, "Serializer did not remove an object it pushed into stack.");
                        serializationManager.Context.Pop();
                    }
                }

                try
                {
                    serializer.OnBeforeSerializeContents(serializationManager, obj);
                }
                catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
                {
                    serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerThrewException, obj.GetType().FullName, e.Message), e));
                    return;
                }

                // serialize compound properties as child elements of the current node.
                foreach (object propertyObj in complexProperties)
                {
                    // get value and check for null
                    string propertyName = String.Empty;
                    object propertyValue = null;
                    Type ownerType = null;
                    bool isReadOnly = false;

                    try
                    {
                        if (propertyObj is PropertyInfo property)
                        {
                            propertyName = property.Name;
                            propertyValue = property.CanRead ? property.GetValue(obj, null) : null;
                            ownerType = obj.GetType();
                            isReadOnly = (!property.CanWrite);
                        }
                    }
                    catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
                    {
                        while (e is TargetInvocationException && e.InnerException != null)
                            e = e.InnerException;

                        serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerPropertyGetFailed, propertyName, ownerType?.FullName ?? "", e.Message)));
                        continue;
                    }

                    if (propertyObj is PropertyInfo propertyInfo && propertyInfo == contentProperty.Property)
                        continue;

                    if (propertyValue != null)
                    {
                        propertyValue = GetMarkupExtensionFromValue(propertyValue);

                        if (serializationManager.GetSerializer(propertyValue.GetType(), typeof(WorkflowMarkupSerializer)) is WorkflowMarkupSerializer propValueSerializer)
                        {
                            using (new SafeXmlNodeWriter(serializationManager, obj, propertyObj, XmlNodeType.Element))
                            {
                                if (isReadOnly)
                                    propValueSerializer.SerializeContents(serializationManager, propertyValue, writer, false);
                                else
                                    propValueSerializer.SerializeObject(serializationManager, propertyValue, writer);
                            }
                        }
                        else
                        {
                            serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerNotAvailableForSerialize, propertyValue.GetType().FullName)));
                        }
                    }
                }

                // serialize the contents
                try
                {
                    object contents = contentProperty.GetContents();
                    if (contents != null)
                    {
                        contents = GetMarkupExtensionFromValue(contents);

                        if (serializationManager.GetSerializer(contents.GetType(), typeof(WorkflowMarkupSerializer)) is not WorkflowMarkupSerializer propValueSerializer)
                        {
                            serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerNotAvailableForSerialize, contents.GetType())));
                        }
                        else
                        {
                            //



                            //NOTE: THE FOLLOWING CONDITION ABOUT contentProperty.Property.PropertyType != typeof(object) is VALID AS WE SHOULD NOT SERIALIZE A PROPERTY OF TYPE OBJECT TO STRING
                            //IF WE DO THAT THEN WE DO NOT KNOWN WHAT WAS THE TYPE OF ORIGINAL OBJECT AND SERIALIZER WONT BE ABLE TO GET THE STRING BACK INTO THE CORRECT TYPE,
                            //AS THE TYPE INFORMATION IS LOST
                            if (propValueSerializer.CanSerializeToString(serializationManager, contents) &&
                                (contentProperty.Property == null || contentProperty.Property.PropertyType != typeof(object)))
                            {
                                string stringValue = propValueSerializer.SerializeToString(serializationManager, contents);
                                if (!string.IsNullOrEmpty(stringValue))
                                {
                                    stringValue = stringValue.Replace('\0', ' ');
                                    if (contents is MarkupExtension || !(stringValue.StartsWith("{", StringComparison.Ordinal) && stringValue.EndsWith("}", StringComparison.Ordinal)))
                                        writer.WriteString(stringValue);
                                    else
                                        writer.WriteString("{}" + stringValue);
                                }
                            }
                            else if (CollectionMarkupSerializer.IsValidCollectionType(contents.GetType()))
                            {
                                if (contentProperty.Property == null)
                                {
                                    IEnumerable enumerableContents = contents as IEnumerable ?? Array.Empty<object>();
                                    foreach (object childObj in enumerableContents)
                                    {
                                        if (childObj == null)
                                        {
                                            SerializeObject(serializationManager, new NullExtension(), writer);
                                        }
                                        else
                                        {
                                            object childObj2 = childObj;
                                            bool dictionaryEntry = (childObj2 is DictionaryEntry);
                                            try
                                            {
                                                if (dictionaryEntry)
                                                {
                                                    serializationManager.WorkflowMarkupStack.Push(childObj);
                                                    childObj2 = ((DictionaryEntry)childObj2).Value;
                                                }
                                                childObj2 = GetMarkupExtensionFromValue(childObj2);
                                                if (serializationManager.GetSerializer(childObj2.GetType(), typeof(WorkflowMarkupSerializer)) is WorkflowMarkupSerializer childObjectSerializer)
                                                    childObjectSerializer.SerializeObject(serializationManager, childObj2, writer);
                                                else
                                                    serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerNotAvailableForSerialize, childObj2.GetType())));
                                            }
                                            finally
                                            {
                                                if (dictionaryEntry)
                                                    serializationManager.WorkflowMarkupStack.Pop();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    propValueSerializer.SerializeContents(serializationManager, contents, writer, false);
                                }
                            }
                            else
                            {
                                propValueSerializer.SerializeObject(serializationManager, contents, writer);
                            }
                        }
                    }
                }
                catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
                {
                    serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerThrewException, obj.GetType().FullName, e.Message), e));
                    return;
                }
            }

            try
            {
                serializer.OnAfterSerialize(serializationManager, obj);
            }
            catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
            {
                serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerThrewException, obj.GetType().FullName, e.Message), e));
                return;
            }
        }
        #endregion

        #region Overridable Methods
        protected virtual void OnBeforeSerialize(WorkflowMarkupSerializationManager serializationManager, object obj)
        {
        }

        internal virtual void OnBeforeSerializeContents(WorkflowMarkupSerializationManager serializationManager, object obj)
        {

        }

        protected virtual void OnAfterSerialize(WorkflowMarkupSerializationManager serializationManager, object obj)
        {
        }

        protected internal virtual void OnBeforeDeserialize(WorkflowMarkupSerializationManager serializationManager, object obj)
        {
        }

        internal virtual void OnBeforeDeserializeContents(WorkflowMarkupSerializationManager serializationManager, object obj)
        {

        }

        protected internal virtual void OnAfterDeserialize(WorkflowMarkupSerializationManager serializationManager, object obj)
        {
        }

        protected internal virtual bool ShouldSerializeValue(WorkflowMarkupSerializationManager serializationManager, object value)
        {
            if (serializationManager == null)
                throw new ArgumentNullException("serializationManager");

            if (value == null)
                return false;

            try
            {
                if (serializationManager.Context.Current is PropertyInfo property)
                {
                    Attribute[] attributes = Attribute.GetCustomAttributes(property, typeof(DefaultValueAttribute), true);
                    if (attributes.Length > 0)
                    {
                        DefaultValueAttribute defaultValueAttr = attributes[0] as DefaultValueAttribute;
                        if (defaultValueAttr?.Value is IConvertible && value is IConvertible && object.Equals(Convert.ChangeType(defaultValueAttr.Value, property.PropertyType, CultureInfo.InvariantCulture), Convert.ChangeType(value, property.PropertyType, CultureInfo.InvariantCulture)))
                            return false;
                    }
                }
            }
            catch (Exception ex) when (!ExceptionUtility.IsCriticalException(ex))
            {
                //We purposely eat all the exceptions as Convert.ChangeType can throw but in that case
                //we continue with serialization
            }

            return true;
        }

        protected internal virtual bool CanSerializeToString(WorkflowMarkupSerializationManager serializationManager, object value)
        {
            if (serializationManager == null)
                throw new ArgumentNullException("serializationManager");
            if (value == null)
                throw new ArgumentNullException("value");

            Type valueType = value.GetType();
            if (valueType.IsPrimitive || valueType == typeof(System.String) || valueType.IsEnum
                || typeof(Delegate).IsAssignableFrom(valueType) || typeof(IConvertible).IsAssignableFrom(valueType)
                || valueType == typeof(TimeSpan) || valueType == typeof(Guid) || valueType == typeof(DateTime))
                return true;

            return false;
        }

        protected internal virtual string SerializeToString(WorkflowMarkupSerializationManager serializationManager, object value)
        {
            if (serializationManager == null)
                throw new ArgumentNullException("serializationManager");
            if (value == null)
                throw new ArgumentNullException("value");

            if (typeof(Delegate).IsAssignableFrom(value.GetType()))
                return ((Delegate)value).Method.Name;
            else if (typeof(DateTime).IsAssignableFrom(value.GetType()))
                return ((DateTime)value).ToString("o", CultureInfo.InvariantCulture);
            else
                return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        protected internal virtual object DeserializeFromString(WorkflowMarkupSerializationManager serializationManager, Type propertyType, string value)
        {
            return deserializeFromStringHelper.DeserializeFromString(serializationManager, propertyType, value);
        }

        protected internal virtual IList GetChildren(WorkflowMarkupSerializationManager serializationManager, object obj)
        {
            if (serializationManager == null)
                throw new ArgumentNullException("serializationManager");
            if (obj == null)
                throw new ArgumentNullException("obj");

            return Array.Empty<object>();
        }

        protected internal virtual void ClearChildren(WorkflowMarkupSerializationManager serializationManager, object obj)
        {
            if (serializationManager == null)
                throw new ArgumentNullException("serializationManager");
            if (obj == null)
                throw new ArgumentNullException("obj");
        }

        protected internal virtual void AddChild(WorkflowMarkupSerializationManager serializationManager, object parentObject, object childObj)
        {
            if (serializationManager == null)
                throw new ArgumentNullException("serializationManager");
            if (parentObject == null)
                throw new ArgumentNullException("parentObject");
            if (childObj == null)
                throw new ArgumentNullException("childObj");

            throw new InvalidOperationException(SR.GetString(SR.Error_SerializerNoChildNotion, [parentObject.GetType().FullName]));
        }

        protected virtual object CreateInstance(WorkflowMarkupSerializationManager serializationManager, Type type)
        {
            if (serializationManager == null)
                throw new ArgumentNullException("serializationManager");
            if (type == null)
                throw new ArgumentNullException("type");

            return Activator.CreateInstance(type);
        }

        protected internal virtual PropertyInfo[] GetProperties(WorkflowMarkupSerializationManager serializationManager, object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (serializationManager == null)
                throw new ArgumentNullException("serializationManager");

            List<PropertyInfo> properties = [];

            object[] attributes = obj.GetType().GetCustomAttributes(typeof(RuntimeNamePropertyAttribute), true);
            string name = null;
            if (attributes.Length > 0)
                name = (attributes[0] as RuntimeNamePropertyAttribute).Name;

            foreach (PropertyInfo property in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
            {
                DesignerSerializationVisibility visibility = Helpers.GetSerializationVisibility(property);
                if (visibility == DesignerSerializationVisibility.Hidden)
                    continue;

                if (visibility != DesignerSerializationVisibility.Content 
                    && (!property.CanWrite || property.GetSetMethod() == null)
                    && (obj is not CodeObject || !typeof(ICollection).IsAssignableFrom(property.PropertyType)))
                {
                    // work around for CodeObject which are ICollection needs to be serialized.
                    continue;
                }

                if (name == null || !name.Equals(property.Name))
                    properties.Add(property);
                else
                    properties.Add(new ExtendedPropertyInfo(property, OnGetRuntimeNameValue, OnSetRuntimeNameValue, OnGetRuntimeQualifiedName));
            }

            return [.. properties];
        }

        protected internal virtual EventInfo[] GetEvents(WorkflowMarkupSerializationManager serializationManager, object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (serializationManager == null)
                throw new ArgumentNullException("serializationManager");

            List<EventInfo> events = [];
            foreach (EventInfo evt 
                in obj.GetType().GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                .Where(e => Helpers.GetSerializationVisibility(e) != DesignerSerializationVisibility.Hidden))
            {
                events.Add(evt);
            }

            return [.. events];
        }

        internal virtual ExtendedPropertyInfo[] GetExtendedProperties(WorkflowMarkupSerializationManager manager, object extendee)
        {
            return [];
        }
        private static object OnGetRuntimeNameValue(ExtendedPropertyInfo extendedProperty, object extendee)
        {
            return extendedProperty.RealPropertyInfo.GetValue(extendee, null);
        }

        private static void OnSetRuntimeNameValue(ExtendedPropertyInfo extendedProperty, object extendee, object value)
        {
            if (extendee != null && value != null)
                extendedProperty.RealPropertyInfo.SetValue(extendee, value, null);
        }

        private static XmlQualifiedName OnGetRuntimeQualifiedName(ExtendedPropertyInfo extendedProperty, WorkflowMarkupSerializationManager manager, out string prefix)
        {
            prefix = StandardXomlKeys.Definitions_XmlNs_Prefix;
            return new XmlQualifiedName(extendedProperty.Name, StandardXomlKeys.Definitions_XmlNs);
        }

        #endregion

        #region Private Helpers

        #region Exception Handling
        internal static WorkflowMarkupSerializationException CreateSerializationError(Exception e, XmlReader reader)
        {
            return CreateSerializationError(null, e, reader);
        }
        internal static WorkflowMarkupSerializationException CreateSerializationError(string message, XmlReader reader)
        {
            return CreateSerializationError(message, null, reader);
        }
        internal static WorkflowMarkupSerializationException CreateSerializationError(string message, Exception e, XmlReader reader)
        {
            string errorMsg = message;
            if (string.IsNullOrEmpty(errorMsg))
                errorMsg = e?.Message ?? string.Empty;

            return reader is IXmlLineInfo xmlLineInfo
                ? new WorkflowMarkupSerializationException(errorMsg, xmlLineInfo.LineNumber, xmlLineInfo.LinePosition)
                : new WorkflowMarkupSerializationException(errorMsg, 0, 0);
        }
        #endregion

        #region SafeXmlNodeWriter
        private sealed class SafeXmlNodeWriter : IDisposable
        {
            private readonly XmlNodeType xmlNodeType;
            private readonly WorkflowMarkupSerializationManager serializationManager = null;

            public SafeXmlNodeWriter(WorkflowMarkupSerializationManager serializationManager, object owner, object property, XmlNodeType xmlNodeType)
            {
                this.serializationManager = serializationManager;
                this.xmlNodeType = xmlNodeType;

                XmlWriter writer = serializationManager.WorkflowMarkupStack[typeof(XmlWriter)] as XmlWriter ?? throw new InvalidOperationException(SR.GetString(SR.Error_InternalSerializerError));
                string prefix = String.Empty;
                string tagName;
                string xmlns;
                if (property is MemberInfo memberInfo)
                {
                    if (property is ExtendedPropertyInfo extendedProperty)
                    {
                        XmlQualifiedName qualifiedName = extendedProperty.GetXmlQualifiedName(this.serializationManager, out prefix);
                        tagName = qualifiedName.Name;
                        xmlns = qualifiedName.Namespace;
                    }
                    else if (this.xmlNodeType == XmlNodeType.Element)
                    {
                        XmlQualifiedName qualifiedName = this.serializationManager.GetXmlQualifiedName(owner.GetType(), out prefix);
                        tagName = qualifiedName.Name + "." + memberInfo.Name;
                        xmlns = qualifiedName.Namespace;
                    }
                    else
                    {
                        tagName = memberInfo.Name;
                        xmlns = String.Empty;
                    }
                }
                else
                {
                    XmlQualifiedName qualifiedName = this.serializationManager.GetXmlQualifiedName(owner.GetType(), out prefix);
                    tagName = qualifiedName.Name;
                    xmlns = qualifiedName.Namespace;
                }

                //verify the node name is valid. This may happen for design time names as 
                // "(Parameter) PropName"
                tagName = XmlConvert.EncodeName(tagName);

                if (this.xmlNodeType == XmlNodeType.Element)
                {
                    writer.WriteStartElement(prefix, tagName, xmlns);
                    this.serializationManager.WriterDepth += 1;
                }
                else if (this.xmlNodeType == XmlNodeType.Attribute)
                {
                    writer.WriteStartAttribute(prefix, tagName, xmlns);
                }
            }

            #region IDisposable Members
            void IDisposable.Dispose()
            {
                if (this.serializationManager.WorkflowMarkupStack[typeof(XmlWriter)] is XmlWriter writer && writer.WriteState != WriteState.Error)
                {
                    if (this.xmlNodeType == XmlNodeType.Element)
                    {
                        writer.WriteEndElement();
                        this.serializationManager.WriterDepth -= 1;
                    }
                    else if (writer.WriteState == WriteState.Attribute)
                    {
                        writer.WriteEndAttribute();
                    }
                }
            }
            #endregion
        }
        #endregion

        #region Property Info Lookup
        private static PropertyInfo LookupProperty(IList<PropertyInfo> properties, string propertyName)
        {
            if (properties != null && !string.IsNullOrEmpty(propertyName))
            {
                return properties.FirstOrDefault(p => p.Name == propertyName);
            }

            return null;
        }
        #endregion

        #endregion

        #region Compact Attribute Support

        internal bool IsValidCompactAttributeFormat(string attributeValue)
        {
            return this.workflowMarkupSerializationHelper.IsValidCompactAttributeFormat(attributeValue);
        }

        #endregion

        #region ContentProperty Support
        [ExcludeFromCodeCoverage]
        private sealed class ContentProperty : IDisposable
        {
            private readonly WorkflowMarkupSerializationManager serializationManager;
            private readonly WorkflowMarkupSerializer parentObjectSerializer;
            private readonly object parentObject;

            private readonly PropertyInfo contentProperty;
            private readonly WorkflowMarkupSerializer contentPropertySerializer;

            public ContentProperty(WorkflowMarkupSerializationManager serializationManager, WorkflowMarkupSerializer parentObjectSerializer, object parentObject)
            {
                this.serializationManager = serializationManager;
                this.parentObjectSerializer = parentObjectSerializer;
                this.parentObject = parentObject;

                this.contentProperty = GetContentProperty(this.serializationManager, this.parentObject);
                if (this.contentProperty != null)
                {
                    this.contentPropertySerializer = this.serializationManager.GetSerializer(this.contentProperty.PropertyType, typeof(WorkflowMarkupSerializer)) as WorkflowMarkupSerializer;
                    if (this.contentPropertySerializer != null)
                    {
                        InitializeContentPropertySerializer(serializationManager);
                    }
                    else
                    {
                        this.serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerNotAvailableForSerialize, this.contentProperty.PropertyType.FullName)));
                    }
                }
            }

            private static object GetValueFromMarkupExtension(WorkflowMarkupSerializationManager manager, object extension)
            {
                object value = extension;
                if (extension is MarkupExtension markupExtension)
                    value = markupExtension.ProvideValue(manager);
                return value;
            }

            private void InitializeContentPropertySerializer(WorkflowMarkupSerializationManager serializationManager)
            {
                try
                {
                    XmlReader reader = this.serializationManager.WorkflowMarkupStack[typeof(XmlReader)] as XmlReader;
                    object contentPropertyValue = null;
                    if (reader == null)
                    {
                        contentPropertyValue = this.contentProperty.GetValue(this.parentObject, null);
                    }
                    else if (!this.contentProperty.PropertyType.IsValueType &&
                            !this.contentProperty.PropertyType.IsPrimitive &&
                            this.contentProperty.PropertyType != typeof(string) &&
                            !IsMarkupExtensionType(this.contentProperty.PropertyType) &&
                            this.contentProperty.CanWrite)
                    {
                        if (serializationManager.GetSerializer(this.contentProperty.PropertyType, typeof(WorkflowMarkupSerializer)) is not WorkflowMarkupSerializer serializer)
                        {
                            serializationManager.ReportError(CreateSerializationError(SR.GetString(SR.Error_SerializerNotAvailable, this.contentProperty.PropertyType.FullName), reader));
                            return;
                        }
                        try
                        {
                            contentPropertyValue = serializer.CreateInstance(serializationManager, this.contentProperty.PropertyType);
                        }
                        catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
                        {
                            serializationManager.ReportError(CreateSerializationError(SR.GetString(SR.Error_SerializerCreateInstanceFailed, this.contentProperty.PropertyType.FullName, e.Message), reader));
                            return;
                        }
                        this.contentProperty.SetValue(this.parentObject, contentPropertyValue, null);
                    }

                    if (contentPropertyValue != null && reader != null)
                    {
                        this.contentPropertySerializer.OnBeforeDeserialize(this.serializationManager, contentPropertyValue);
                        this.contentPropertySerializer.OnBeforeDeserializeContents(this.serializationManager, contentPropertyValue);
                    }
                }
                catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
                {
                    this.serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerThrewException, this.parentObject.GetType(), e.Message), e));
                }
            }

            private static bool IsMarkupExtensionType(Type type)
            {
                return (typeof(MarkupExtension).IsAssignableFrom(type) ||
                        typeof(System.Type).IsAssignableFrom(type) ||
                        typeof(System.Array).IsAssignableFrom(type));
            }

            public void Dispose()
            {
                Dispose(true);
            }

            private bool disposed;
            private void Dispose(bool disposing)
            {
                if (this.disposed)
                    return;

                if (disposing && this.serializationManager.WorkflowMarkupStack[typeof(XmlReader)] is XmlReader && this.contentProperty != null && this.contentPropertySerializer != null)
                {
                    try
                    {
                        object contentPropertyValue = this.contentProperty.GetValue(this.parentObject, null);
                        if (contentPropertyValue != null)
                            this.contentPropertySerializer.OnAfterDeserialize(this.serializationManager, contentPropertyValue);
                    }
                    catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
                    {
                        this.serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerThrewException, this.parentObject.GetType(), e.Message), e));
                    }
                }

                this.disposed = true;
            }

            internal PropertyInfo Property
            {
                get
                {
                    return this.contentProperty;
                }
            }

            internal object GetContents()
            {
                return this.contentProperty != null
                    ? this.contentProperty.GetValue(this.parentObject, null)
                    : this.parentObjectSerializer.GetChildren(this.serializationManager, this.parentObject);
            }

            internal void SetContents(IList<ContentInfo> contents)
            {
                if (contents.Count == 0)
                    return;

                if (this.contentProperty == null)
                {
                    int i = 0;
                    try
                    {
                        foreach (ContentInfo contentInfo in contents)
                        {
                            this.parentObjectSerializer.AddChild(this.serializationManager, this.parentObject, contentInfo.Content);
                            i += 1;
                        }
                    }
                    catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
                    {
                        this.serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerThrewException, this.parentObject.GetType(), e.Message), e, contents[i].LineNumber, contents[i].LinePosition));
                    }
                }
                else if (this.contentPropertySerializer != null)
                {
                    object propertyValue = this.contentProperty.GetValue(this.parentObject, null);
                    if (CollectionMarkupSerializer.IsValidCollectionType(this.contentProperty.PropertyType))
                    {
                        SetPropertyValueForCollection(contents, propertyValue);
                    }
                    else
                    {
                        SetPropertyValueForObject(contents);
                    }
                }
            }

            private void SetPropertyValueForCollection(IList<ContentInfo> contents, object propertyValue)
            {
                if (propertyValue == null)
                {
                    this.serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_ContentPropertyCanNotBeNull, this.contentProperty.Name, this.parentObject.GetType().FullName)));
                    return;
                }

                //Notify serializer about begining of deserialization process
                int i = 0;
                try
                {
                    foreach (ContentInfo contentInfo in contents)
                    {
                        this.contentPropertySerializer.AddChild(this.serializationManager, propertyValue, contentInfo.Content);
                        i++;
                    }
                }
                catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
                {
                    this.serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerThrewException, this.parentObject.GetType(), e.Message), e, contents[i].LineNumber, contents[i].LinePosition));
                }
            }

            private void SetPropertyValueForObject(IList<ContentInfo> contents)
            {
                if (!this.contentProperty.CanWrite)
                {
                    this.serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_ContentPropertyNoSetter, this.contentProperty.Name, this.parentObject.GetType()), contents[0].LineNumber, contents[0].LinePosition));
                    return;
                }

                if (contents.Count > 1)
                    this.serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_ContentPropertyNoMultipleContents, this.contentProperty.Name, this.parentObject.GetType()), contents[1].LineNumber, contents[1].LinePosition));

                object content = contents[0].Content;
                if (!this.contentProperty.PropertyType.IsInstanceOfType(content) && content is string contentString)
                {
                    try
                    {
                        content = this.contentPropertySerializer.DeserializeFromString(this.serializationManager, this.contentProperty.PropertyType, contentString);
                        content = GetValueFromMarkupExtension(this.serializationManager, content);
                    }
                    catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
                    {
                        this.serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerThrewException, this.parentObject.GetType(), e.Message), e, contents[0].LineNumber, contents[0].LinePosition));
                        return;
                    }
                }

                if (content == null)
                {
                    this.serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_ContentCanNotBeConverted, content as string, contentProperty.Name, this.parentObject.GetType().FullName, this.contentProperty.PropertyType.FullName), contents[0].LineNumber, contents[0].LinePosition));
                }
                else if (!contentProperty.PropertyType.IsInstanceOfType(content))
                {
                    this.serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_ContentPropertyValueInvalid, content.GetType(), this.contentProperty.Name, this.contentProperty.PropertyType.FullName), contents[0].LineNumber, contents[0].LinePosition));
                }
                else
                {
                    try
                    {
                        if (this.contentProperty.PropertyType == typeof(string))
                        {
                            content = new WorkflowMarkupSerializer().DeserializeFromString(this.serializationManager, this.contentProperty.PropertyType, content as string);
                            content = GetValueFromMarkupExtension(this.serializationManager, content);
                        }
                        this.contentProperty.SetValue(this.parentObject, content, null);
                    }
                    catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
                    {
                        this.serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerThrewException, this.parentObject.GetType(), e.Message), e, contents[0].LineNumber, contents[0].LinePosition));
                    }
                }
            }

            private static PropertyInfo GetContentProperty(WorkflowMarkupSerializationManager serializationManagerLocal, object parentObjectLocal)
            {
                PropertyInfo contentPropertyLocal = null;

                string contentPropertyName = String.Empty;
                object[] contentPropertyAttributes = parentObjectLocal.GetType().GetCustomAttributes(typeof(ContentPropertyAttribute), true);
                if (contentPropertyAttributes != null && contentPropertyAttributes.Length > 0)
                    contentPropertyName = ((ContentPropertyAttribute)contentPropertyAttributes[0]).Name;

                if (!String.IsNullOrEmpty(contentPropertyName))
                {
                    contentPropertyLocal = parentObjectLocal.GetType().GetProperty(contentPropertyName, BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public);
                    if (contentPropertyLocal == null)
                        serializationManagerLocal.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_ContentPropertyCouldNotBeFound, contentPropertyName, parentObjectLocal.GetType().FullName)));
                }

                return contentPropertyLocal;
            }
        }

        [ExcludeFromCodeCoverage]
        private readonly struct ContentInfo(object content, int lineNumber, int linePosition)
        {
            public readonly int LineNumber = lineNumber;
            public readonly int LinePosition = linePosition;
            public readonly object Content = content;
        }
        #endregion

        #region MarkupExtension Support
        internal static string EnsureMarkupExtensionTypeName(Type type)
        {
            string extensionName = type.Name;
            if (extensionName.EndsWith(StandardXomlKeys.MarkupExtensionSuffix, StringComparison.OrdinalIgnoreCase))
                extensionName = extensionName.Substring(0, extensionName.Length - StandardXomlKeys.MarkupExtensionSuffix.Length);
            return extensionName;
        }

        internal static string EnsureMarkupExtensionTypeName(XmlQualifiedName xmlQualifiedName)
        {
            string typeName = xmlQualifiedName.Name;
            if (xmlQualifiedName.Namespace.Equals(StandardXomlKeys.Definitions_XmlNs, StringComparison.Ordinal)
                && typeName.Equals(typeof(Array).Name, StringComparison.Ordinal))
            {
                typeName = typeof(ArrayExtension).Name;
            }
            return typeName;
        }

        private static object GetMarkupExtensionFromValue(object value)
        {
            if (value == null)
                return new NullExtension();
            if (value is System.Type typeValue)
                return new TypeExtension(typeValue);
            if (value is Array arrayValue)
                return new ArrayExtension(arrayValue);

            return value;
        }
        #endregion
    }
    #endregion
}
