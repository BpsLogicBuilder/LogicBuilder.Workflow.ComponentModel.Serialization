using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using System;

namespace LogicBuilder.Workflow.ComponentModel.Design
{
    internal sealed class PointMarkupSerializer : WorkflowMarkupSerializer
    {
        protected internal override bool CanSerializeToString(WorkflowMarkupSerializationManager serializationManager, object value)
        {
            return (value is Point);
        }

        protected internal override PropertyInfo[] GetProperties(WorkflowMarkupSerializationManager serializationManager, object obj)
        {
            List<PropertyInfo> properties = [];
            if (obj is Point)
            {
                properties.Add(typeof(Point).GetProperty("X"));
                properties.Add(typeof(Point).GetProperty("Y"));
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
            object point = Point.Empty;

            string pointValue = value as string;
            if (!String.IsNullOrWhiteSpace(pointValue))
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Point));
                point = converter != null && converter.CanConvertFrom(typeof(string)) && !IsValidCompactAttributeFormat(pointValue)
                    ? converter.ConvertFrom(value)
                    : base.SerializeToString(serializationManager, value);
            }

            return point;
        }
    }
}
