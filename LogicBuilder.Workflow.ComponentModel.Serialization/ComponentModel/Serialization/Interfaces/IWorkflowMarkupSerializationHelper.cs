using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces
{
    internal interface IWorkflowMarkupSerializationHelper
    {
        void AdvanceReader(XmlReader reader);
        object CreateInstance(Type type);
        bool IsMarkupExtension(XmlQualifiedName xmlQualifiedName);
        bool IsValidCompactAttributeFormat(string attributeValue);
        PropertyInfo LookupProperty(IList<PropertyInfo> properties, string propertyName);
        // Remove any '\' escape characters from the passed string.  This does a simple
        // pass through the string and won't do anything if there are no '\' characters.
        void RemoveEscapes(ref string value);
    }
}
