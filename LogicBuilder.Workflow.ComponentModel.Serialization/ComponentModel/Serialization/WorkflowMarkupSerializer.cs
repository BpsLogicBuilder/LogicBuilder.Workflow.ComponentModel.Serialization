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
        private readonly IObjectSerializer objectSerializer = ObjectSerializerFactory.Create();
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
            serializationManager.WorkflowMarkupStack.Push(xmlReader);

            try
            {
                while (xmlReader.Read() && xmlReader.NodeType != XmlNodeType.Element && xmlReader.NodeType != XmlNodeType.ProcessingInstruction)
                {
                    // Advance the reader until we find the root element or reach the end of the stream. This allows us to ignore any leading comments or processing instructions.
                }
                if (xmlReader.EOF)
                    return null;
                obj = DeserializeObject(serializationManager, xmlReader);

                while (xmlReader.Read() && !xmlReader.EOF)
                {
                    // Read until the end of the xml stream i.e past the </XomlDocument> tag. 
                    // If there are any exceptions log them as errors.
                }
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
            objectSerializer.SerializeObject(serializationManager, obj, writer);
        }

        #endregion

        #region Overridable Methods
        protected internal virtual void OnBeforeSerialize(WorkflowMarkupSerializationManager serializationManager, object obj)
        {
        }

        internal virtual void OnBeforeSerializeContents(WorkflowMarkupSerializationManager serializationManager, object obj)
        {

        }

        protected internal virtual void OnAfterSerialize(WorkflowMarkupSerializationManager serializationManager, object obj)
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

            if (value is Delegate delegateValue)
                return delegateValue.Method.Name;
            else if (value is DateTime dateTimeValue)
                return dateTimeValue.ToString("o", CultureInfo.InvariantCulture);
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

            return null!;//NOSONAR - this maintains the existing behavior such that the serializer does not create ab empty list in this scenario.
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

        #endregion

        #region Compact Attribute Support

        internal bool IsValidCompactAttributeFormat(string attributeValue)
        {
            return this.workflowMarkupSerializationHelper.IsValidCompactAttributeFormat(attributeValue);
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
        #endregion
    }
    #endregion
}
