using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using System;

namespace LogicBuilder.Workflow.ComponentModel.Design
{
    internal sealed class SizeMarkupSerializer : WorkflowMarkupSerializer
    {
        protected internal override bool CanSerializeToString(WorkflowMarkupSerializationManager serializationManager, object value)
        {
            return (value is System.Drawing.Size);
        }

        protected internal override PropertyInfo[] GetProperties(WorkflowMarkupSerializationManager serializationManager, object obj)
        {
            List<PropertyInfo> properties = [];
            if (obj is Size)
            {
                properties.Add(typeof(Size).GetProperty("Width"));
                properties.Add(typeof(Size).GetProperty("Height"));
            }
            return [.. properties];
        }

        protected internal override string SerializeToString(WorkflowMarkupSerializationManager serializationManager, object value)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(value);
            return converter != null && converter.CanConvertTo(typeof(string))
                ? converter.ConvertTo(value, typeof(string)) as string
                : base.SerializeToString(serializationManager, value);
        }

        protected internal override object DeserializeFromString(WorkflowMarkupSerializationManager serializationManager, Type propertyType, string value)
        {
            object size = Size.Empty;

            string sizeValue = value as string;
            if (!String.IsNullOrWhiteSpace(sizeValue))
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Size));
                size = converter != null && converter.CanConvertFrom(typeof(string)) && !IsValidCompactAttributeFormat(sizeValue)
                    ? converter.ConvertFrom(value)
                    : base.SerializeToString(serializationManager, value);
            }

            return size;
        }
    }
}
