using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace LogicBuilder.Workflow.ComponentModel.Serialization
{
    internal class WorkflowMarkupSerializationHelper : IWorkflowMarkupSerializationHelper
    {
        public void AdvanceReader(XmlReader reader)
        {
            //Compressed what process mapping pi used to do
            while (reader.NodeType != XmlNodeType.EndElement 
                && reader.NodeType != XmlNodeType.Element 
                && reader.NodeType != XmlNodeType.Text && reader.Read())
            {
                // Just keep reading until we find an element, text, or end element.
            }
        }

        public object CreateInstance(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return Activator.CreateInstance(type);
        }

        public bool IsMarkupExtension(XmlQualifiedName xmlQualifiedName)
        {
            bool markupExtension = false;
            if (xmlQualifiedName.Namespace.Equals(StandardXomlKeys.Definitions_XmlNs, StringComparison.Ordinal)
                && (xmlQualifiedName.Name.Equals(typeof(Array).Name) || string.Equals(xmlQualifiedName.Name, "Null", StringComparison.Ordinal) || string.Equals(xmlQualifiedName.Name, typeof(NullExtension).Name, StringComparison.Ordinal) || string.Equals(xmlQualifiedName.Name, "Type", StringComparison.Ordinal) || string.Equals(xmlQualifiedName.Name, typeof(TypeExtension).Name, StringComparison.Ordinal)))
            {
                markupExtension = true;
            }
            return markupExtension;
        }

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
