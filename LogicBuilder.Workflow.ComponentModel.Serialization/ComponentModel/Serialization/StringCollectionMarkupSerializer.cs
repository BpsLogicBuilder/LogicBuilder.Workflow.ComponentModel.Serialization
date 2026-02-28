namespace LogicBuilder.Workflow.ComponentModel.Serialization
{
    using LogicBuilder.Workflow.ComponentModel.Serialization.Factories;
    using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xml;

    internal sealed class StringCollectionMarkupSerializer : WorkflowMarkupSerializer
    {
        private readonly IFromCompactFormatDeserializer fromCompactFormatDeserializer = FromCompactFormatDeserializerFactory.Create(DependencyHelperFactory.Create());

        protected internal override PropertyInfo[] GetProperties(WorkflowMarkupSerializationManager serializationManager, object obj)
        {
            return new PropertyInfo[] { };
        }

        protected internal override bool CanSerializeToString(WorkflowMarkupSerializationManager serializationManager, object value)
        {
            if (serializationManager == null)
                throw new ArgumentNullException("serializationManager");
            if (value == null)
                throw new ArgumentNullException("value");

            return (value is ICollection<String>);
        }

        protected internal override string SerializeToString(WorkflowMarkupSerializationManager serializationManager, object value)
        {
            if (serializationManager == null)
                throw new ArgumentNullException("serializationManager");
            if (value == null)
                throw new ArgumentNullException("value");

            return SynchronizationHandlesTypeConverter.Stringify(value as ICollection<String>);
        }

        protected internal override object DeserializeFromString(WorkflowMarkupSerializationManager serializationManager, Type propertyType, string value)
        {
            if (serializationManager == null)
                throw new ArgumentNullException("serializationManager");
            if (propertyType == null)
                throw new ArgumentNullException("propertyType");
            if (value == null)
                throw new ArgumentNullException("value");

            // Work around For Bind based properties whose base type is an 
            // ICollection<string> or its derivative, special case! (A synchronization
            // handle cannot begin with a * because it won't be a language independent
            // identifier :) )
            return IsValidCompactAttributeFormat(value)
                ? fromCompactFormatDeserializer.DeserializeFromCompactFormat(serializationManager, serializationManager.WorkflowMarkupStack[typeof(XmlReader)] as XmlReader, value)
                : SynchronizationHandlesTypeConverter.UnStringify(value);
        }
    }
}
