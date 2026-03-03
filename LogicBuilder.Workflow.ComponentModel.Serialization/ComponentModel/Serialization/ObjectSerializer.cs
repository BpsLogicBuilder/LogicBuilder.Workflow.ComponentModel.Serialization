using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace LogicBuilder.Workflow.ComponentModel.Serialization
{
    internal class ObjectSerializer(
        IDeserializeFromStringHelper deserializeFromStringHelper,
        IMarkupExtensionHelper markupExtensionHelper,
        ISerializationErrorHelper serializationErrorHelper,
        IWorkflowMarkupSerializationHelper workflowMarkupSerializationHelper) : IObjectSerializer
    {
        private readonly IDeserializeFromStringHelper deserializeFromStringHelper = deserializeFromStringHelper;
        private readonly IMarkupExtensionHelper markupExtensionHelper = markupExtensionHelper;
        private readonly ISerializationErrorHelper serializationErrorHelper = serializationErrorHelper;
        private readonly IWorkflowMarkupSerializationHelper workflowMarkupSerializationHelper = workflowMarkupSerializationHelper;

        public void SerializeContents(WorkflowMarkupSerializationManager serializationManager, object obj, XmlWriter writer, bool dictionaryKey)
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
            if (IsConvertibleObject(obj))
            {
                ProcessConvertibleObjects(serializationManager, obj, writer, dictionaryKey, properties);
            }
            else
            {
                if (!ProcessComplexObjects(serializationManager, obj, dictionaryKey, serializer, properties, events))
                    return;
            }

            UpdatePropertiesAndEvents(allProperties, properties, events);

            if (!SerializeProperties(serializationManager, obj, writer, serializer, allProperties, complexProperties))
                return;

            try
            {
                serializer.OnAfterSerialize(serializationManager, obj);
            }
            catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
            {
                serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerThrewException, obj.GetType().FullName, e.Message), e));
            }
        }

        public void SerializeObject(WorkflowMarkupSerializationManager serializationManager, object obj, XmlWriter writer)
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

        private static WorkflowMarkupSerializer GetPropValueSerializer(WorkflowMarkupSerializationManager serializationManager, object obj, Type propertyValueType)
        {
            WorkflowMarkupSerializer propValueSerializer;
            try
            {
                propValueSerializer = serializationManager.GetSerializer(propertyValueType, typeof(WorkflowMarkupSerializer)) as WorkflowMarkupSerializer;
            }
            catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
            {
                serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerThrewException, obj.GetType().FullName, e.Message), e));
                serializationManager.Context.Pop();
                return null;
            }
            if (propValueSerializer == null)
            {
                serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerNotAvailableForSerialize, propertyValueType?.FullName ?? "")));
                serializationManager.Context.Pop();
                return null;
            }

            return propValueSerializer;
        }

        private static bool IsConvertibleObject(object obj)
        {
            return obj.GetType().IsPrimitive
                            || (obj is string)
                            || (obj is decimal)
                            || (obj is DateTime)
                            || (obj is TimeSpan)
                            || obj.GetType().IsEnum
                            || (obj is Guid);
        }

        private static bool ProcessComplexObjects(WorkflowMarkupSerializationManager serializationManager, object obj, bool dictionaryKey, WorkflowMarkupSerializer serializer, List<PropertyInfo> properties, List<EventInfo> events)
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
                return false;
            }

            return true;
        }

        private static void ProcessConvertibleObjects(WorkflowMarkupSerializationManager serializationManager, object obj, XmlWriter writer, bool dictionaryKey, List<PropertyInfo> properties)
        {
            if ((obj is char)
                || (obj is byte)
                || (obj is short)
                || (obj is decimal)
                || (obj is DateTime)
                || (obj is TimeSpan)
                || obj.GetType().IsEnum
                || (obj is Guid))
            {
                ProcessNonCompliantObjects(obj, writer);
            }
            else if (obj is string attribValue)
            {
                attribValue = attribValue.Replace('\0', ' ');
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

        private static void ProcessNonCompliantObjects(object obj, XmlWriter writer)
        {
            //These non CLS-compliant are not supported in the XmlWriter 
            if ((obj is not char charObject) || charObject != '\0')
            {
                //These non CLS-compliant are not supported in the XmlReader 
                string stringValue;
                if (obj is DateTime dateTimeValue)
                {
                    stringValue = dateTimeValue.ToString("o", CultureInfo.InvariantCulture);
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

        private void SerializeCollection(WorkflowMarkupSerializationManager serializationManager, XmlWriter writer, object contents)
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
                        childObj2 = markupExtensionHelper.GetMarkupExtensionFromValue(childObj2);
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

        private bool SerializeProperties(WorkflowMarkupSerializationManager serializationManager, object obj, XmlWriter writer, WorkflowMarkupSerializer serializer, SortedDictionary<string, object> allProperties, ArrayList complexProperties)
        {
            using ContentProperty contentProperty = new(serializationManager, serializer, obj, deserializeFromStringHelper, markupExtensionHelper, serializationErrorHelper, workflowMarkupSerializationHelper);
            foreach (object propertyObj in allProperties.Values)
            {
                WritePropertyToString(serializationManager, obj, writer, complexProperties, contentProperty, propertyObj);
            }

            try
            {
                serializer.OnBeforeSerializeContents(serializationManager, obj);
            }
            catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
            {
                serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerThrewException, obj.GetType().FullName, e.Message), e));
                return false;
            }

            // serialize compound properties as child elements of the current node.
            foreach (object propertyObj in complexProperties)
            {
                WriteComplexProperty(serializationManager, obj, writer, contentProperty, propertyObj);
            }

            // serialize the contents
            try
            {
                object contents = contentProperty.GetContents();
                if (contents == null)
                    return true;

                contents = markupExtensionHelper.GetMarkupExtensionFromValue(contents);

                if (serializationManager.GetSerializer(contents.GetType(), typeof(WorkflowMarkupSerializer)) is not WorkflowMarkupSerializer propValueSerializer)
                {
                    serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerNotAvailableForSerialize, contents.GetType())));
                }
                else
                {
                    SerializeTheContents(serializationManager, writer, contentProperty, contents, propValueSerializer);
                }

                return true;
            }
            catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
            {
                serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerThrewException, obj.GetType().FullName, e.Message), e));
                return false;
            }
        }

        private void SerializeTheContents(WorkflowMarkupSerializationManager serializationManager, XmlWriter writer, ContentProperty contentProperty, object contents, WorkflowMarkupSerializer propValueSerializer)
        {
            if (ShouldSerializeAsString(serializationManager, contentProperty, contents, propValueSerializer))
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
                    SerializeCollection(serializationManager, writer, contents);
                }
                else
                {
                    SerializeContents(serializationManager, contents, writer, false);
                }
            }
            else
            {
                SerializeObject(serializationManager, contents, writer);
            }
        }

        private static void SerializeValue(WorkflowMarkupSerializationManager serializationManager, object obj, XmlWriter writer, object propertyObj, object propertyValue, WorkflowMarkupSerializer propValueSerializer)
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

        private static bool SetNameValueAndInfoType(object obj, object propertyObj, ref string propertyName, ref object propertyValue, ref Type propertyInfoType)
        {
            if (propertyObj is PropertyInfo property)
            {
                // If the property has parameters we can not serialize it , we just move on.
                ParameterInfo[] indexParameters = property.GetIndexParameters();
                if (indexParameters != null && indexParameters.Length > 0)
                    return false;

                propertyName = property.Name;
                propertyValue = null;
                if (property.CanRead)
                {
                    propertyValue = property.GetValue(obj, null);
                }
                propertyInfoType = property.PropertyType;
            }

            return true;
        }

        private void SetPropertyValueAndValuetype(object propertyObj, ref object propertyValue, ref Type propertyValueType)
        {
            if (propertyValue != null)
            {
                propertyValue = markupExtensionHelper.GetMarkupExtensionFromValue(propertyValue);
                propertyValueType = propertyValue.GetType();
            }
            else if (propertyObj is PropertyInfo pInfo)
            {
                propertyValue = new NullExtension();
                propertyValueType = propertyValue.GetType();
                Attribute[] attributes = Attribute.GetCustomAttributes(pInfo, typeof(DefaultValueAttribute), true);
                if (attributes.Length > 0)
                {
                    DefaultValueAttribute defaultValueAttr = attributes[0] as DefaultValueAttribute;
                    if (defaultValueAttr?.Value == null)
                        propertyValue = null;
                }
            }
            if (propertyValue != null)
                propertyValueType = propertyValue.GetType();
        }

        private static bool ShouldSerializeAsString(WorkflowMarkupSerializationManager serializationManager, ContentProperty contentProperty, object contents, WorkflowMarkupSerializer propValueSerializer)
        {
            //NOTE: THE FOLLOWING CONDITION ABOUT contentProperty.Property.PropertyType != typeof(object) is VALID AS WE SHOULD NOT SERIALIZE A PROPERTY OF TYPE OBJECT TO STRING
            //IF WE DO THAT THEN WE DO NOT KNOWN WHAT WAS THE TYPE OF ORIGINAL OBJECT AND SERIALIZER WONT BE ABLE TO GET THE STRING BACK INTO THE CORRECT TYPE,
            //AS THE TYPE INFORMATION IS LOST
            return propValueSerializer.CanSerializeToString(serializationManager, contents) &&
                            (contentProperty.Property == null || contentProperty.Property.PropertyType != typeof(object));
        }

        private static void UpdatePropertiesAndEvents(SortedDictionary<string, object> allProperties, List<PropertyInfo> properties, List<EventInfo> events)
        {
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
        }

        private void WriteComplexProperty(WorkflowMarkupSerializationManager serializationManager, object obj, XmlWriter writer, ContentProperty contentProperty, object propertyObj)
        {
            Trace.Assert(obj != null, "obj cannot be null");

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
                    isReadOnly = !property.CanWrite;
                }
            }
            catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
            {
                while (e is TargetInvocationException && e.InnerException != null)
                    e = e.InnerException;

                serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerPropertyGetFailed, propertyName, ownerType.FullName, e.Message)));
                return;
            }

            if (propertyObj is PropertyInfo propertyInfo && propertyInfo == contentProperty.Property)
                return;

            if (propertyValue == null)
                return;

            propertyValue = markupExtensionHelper.GetMarkupExtensionFromValue(propertyValue);

            if (serializationManager.GetSerializer(propertyValue.GetType(), typeof(WorkflowMarkupSerializer)) is WorkflowMarkupSerializer)
            {
                using (new SafeXmlNodeWriter(serializationManager, obj, propertyObj, XmlNodeType.Element))
                {
                    if (isReadOnly)
                        SerializeContents(serializationManager, propertyValue, writer, false);
                    else
                        SerializeObject(serializationManager, propertyValue, writer);
                }
            }
            else
            {
                serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerNotAvailableForSerialize, propertyValue.GetType().FullName)));
            }
        }

        private void WritePropertyToString(WorkflowMarkupSerializationManager serializationManager, object obj, XmlWriter writer, ArrayList complexProperties, ContentProperty contentProperty, object propertyObj)
        {
            string propertyName = String.Empty;
            object propertyValue = null;
            Type propertyInfoType = null;

            try
            {
                if (!SetNameValueAndInfoType(obj, propertyObj, ref propertyName, ref propertyValue, ref propertyInfoType))
                    return;
            }
            catch (Exception e) when (!ExceptionUtility.IsCriticalException(e))
            {
                while (e is TargetInvocationException && e.InnerException != null)
                    e = e.InnerException;

                serializationManager.ReportError(new WorkflowMarkupSerializationException(SR.GetString(SR.Error_SerializerPropertyGetFailed, [propertyName, obj.GetType().FullName, e.Message])));
                return;
            }

            if (propertyObj is PropertyInfo propertyInfo && contentProperty.Property == propertyInfo)
                return;

            Type propertyValueType = null;
            SetPropertyValueAndValuetype(propertyObj, ref propertyValue, ref propertyValueType);

            //Now get the serializer to persist the properties, if the serializer is not found then we dont serialize the properties
            serializationManager.Context.Push(propertyObj);
            WorkflowMarkupSerializer propValueSerializer = GetPropValueSerializer(serializationManager, obj, propertyValueType);
            if (propValueSerializer == null)
                return;

            try
            {
                // ask serializer if we can serialize
                if (!propValueSerializer.ShouldSerializeValue(serializationManager, propertyValue))
                    return;

                //NOTE: THE FOLLOWING CONDITION ABOUT propertyInfoType != typeof(object) is VALID AS WE SHOULD NOT SERIALIZE A PROPERTY OF TYPE OBJECT TO STRING
                //IF WE DO THAT THEN WE DO NOT KNOWN WHAT WAS THE TYPE OF ORIGINAL OBJECT AND SERIALIZER WONT BE ABLE TO GET THE STRING BACK INTO THE CORRECT TYPE,
                //AS THE TYPE INFORMATION IS LOST
                if (propValueSerializer.CanSerializeToString(serializationManager, propertyValue) && propertyInfoType != typeof(object))
                {
                    SerializeValue(serializationManager, obj, writer, propertyObj, propertyValue, propValueSerializer);
                }
                else
                {
                    complexProperties.Add(propertyObj);
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
    }
}
