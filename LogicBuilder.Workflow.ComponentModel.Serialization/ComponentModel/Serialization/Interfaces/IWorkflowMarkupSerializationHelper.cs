using System.Collections.Generic;
using System.Reflection;

namespace LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces
{
    internal interface IWorkflowMarkupSerializationHelper
    {
        bool IsValidCompactAttributeFormat(string attributeValue);
        PropertyInfo LookupProperty(IList<PropertyInfo> properties, string propertyName);
        // Remove any '\' escape characters from the passed string.  This does a simple
        // pass through the string and won't do anything if there are no '\' characters.
        void RemoveEscapes(ref string value);
    }
}
