using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LogicBuilder.Workflow.ComponentModel.Serialization
{
    internal class WorkflowMarkupSerializationHelper : IWorkflowMarkupSerializationHelper
    {
        public bool IsValidCompactAttributeFormat(string attributeValue)
        {
            return attributeValue.Length > 0 && attributeValue.StartsWith("{", StringComparison.Ordinal) && !attributeValue.StartsWith("{}", StringComparison.Ordinal) && attributeValue.EndsWith("}", StringComparison.Ordinal);
        }

        public PropertyInfo LookupProperty(IList<PropertyInfo> properties, string propertyName)
        {
            if (properties != null && !string.IsNullOrEmpty(propertyName))
            {
                return properties.FirstOrDefault(p => p.Name == propertyName);
            }

            return null;
        }

        public void RemoveEscapes(ref string value)
        {
            StringBuilder builder = null;
            bool noEscape = true;
            for (int i = 0; i < value.Length; i++)
            {
                if (noEscape && value[i] == '\\')
                {
                    if (builder == null)
                    {
                        builder = new StringBuilder(value.Length);
                        builder.Append(value.Substring(0, i));
                    }
                    noEscape = false;
                }
                else if (builder != null)
                {
                    builder.Append(value[i]);
                    noEscape = true;
                }
            }

            if (builder != null)
            {
                value = builder.ToString();
            }
        }
    }
}
