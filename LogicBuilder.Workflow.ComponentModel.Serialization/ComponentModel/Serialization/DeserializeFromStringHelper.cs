using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Xml;

namespace LogicBuilder.Workflow.ComponentModel.Serialization
{
    internal class DeserializeFromStringHelper(IFromCompactFormatDeserializer fromCompactFormatDeserializer, ISerializationErrorHelper serializationErrorHelper, IWorkflowMarkupSerializationHelper workflowMarkupSerializationHelper) : IDeserializeFromStringHelper
    {
        private readonly IFromCompactFormatDeserializer fromCompactFormatDeserializer = fromCompactFormatDeserializer;
        private readonly ISerializationErrorHelper serializationErrorHelper = serializationErrorHelper;
        private readonly IWorkflowMarkupSerializationHelper workflowMarkupSerializationHelper = workflowMarkupSerializationHelper;

        public object DeserializeFromString(WorkflowMarkupSerializationManager serializationManager, Type propertyType, string value)
        {
            if (serializationManager == null)
                throw new ArgumentNullException("serializationManager");
            if (propertyType == null)
                throw new ArgumentNullException("propertyType");
            if (value == null)
                throw new ArgumentNullException("value");

            object propVal;
            if (serializationManager.WorkflowMarkupStack[typeof(XmlReader)] is not XmlReader reader)
            {
                Debug.Assert(false, "XmlReader not available.");
                return null;
            }
            if (workflowMarkupSerializationHelper.IsValidCompactAttributeFormat(value))
            {
                propVal = fromCompactFormatDeserializer.DeserializeFromCompactFormat(serializationManager, reader, value);
                return propVal;
            }

            return DeserializeForNonCompactFormat(serializationManager, ref propertyType, ref value, out propVal, reader);
        }

        private object DeserializeForNonCompactFormat(WorkflowMarkupSerializationManager serializationManager, ref Type propertyType, ref string value, out object propVal, XmlReader reader)
        {
            if (value.StartsWith("{}", StringComparison.Ordinal))
                value = value.Substring(2);
            // Check for Nullable types
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                Type genericType = propertyType.GetGenericArguments()[0];
                Debug.Assert(genericType != null);
                propertyType = genericType;
            }
            if (propertyType.IsPrimitive || propertyType == typeof(System.String))
            {
                propVal = Convert.ChangeType(value, propertyType, CultureInfo.InvariantCulture);
            }
            else if (propertyType.IsEnum)
            {
                propVal = Enum.Parse(propertyType, value, true);
            }
            else if (typeof(Delegate).IsAssignableFrom(propertyType))
            {
                // Just return the method name.  This must happen after Bind syntax has been checked.
                propVal = value;
            }
            else if (typeof(TimeSpan) == propertyType)
            {
                propVal = TimeSpan.Parse(value, CultureInfo.InvariantCulture);
            }
            else if (typeof(DateTime) == propertyType)
            {
                propVal = DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            }
            else if (typeof(Guid) == propertyType)
            {
                propVal = Utility.CreateGuid(value);
            }
            else if (typeof(Type).IsAssignableFrom(propertyType))
            {
                return DeserializeToType(serializationManager, value, out propVal);
            }
            else if (typeof(IConvertible).IsAssignableFrom(propertyType))
            {
                propVal = Convert.ChangeType(value, propertyType, CultureInfo.InvariantCulture);
            }
            else if (propertyType.IsInstanceOfType(value))
            {
                propVal = value;
            }
            else
            {
                throw serializationErrorHelper.CreateSerializationError(SR.GetString(SR.Error_SerializerPrimitivePropertyNoLogic, ["", value.Trim(), ""]), reader);
            }

            return propVal;
        }

        private static object DeserializeToType(WorkflowMarkupSerializationManager serializationManager, string value, out object propVal)
        {
            propVal = serializationManager.GetType(value);
            if (propVal != null)
            {
                Type type = propVal as Type;
                if (type?.IsPrimitive == true || type?.IsEnum == true || type == typeof(System.String))
                    return type;
            }
            return value;
        }
    }
}
