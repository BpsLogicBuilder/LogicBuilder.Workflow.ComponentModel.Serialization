using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;
using System;

namespace LogicBuilder.Workflow.ComponentModel.Serialization
{
    internal class MarkupExtensionHelper() : IMarkupExtensionHelper
    {
        public object GetMarkupExtensionFromValue(object value)
        {
            if (value == null)
                return new NullExtension();
            if (value is System.Type typeValue)
                return new TypeExtension(typeValue);
            if (value is Array arrayValue)
                return new ArrayExtension(arrayValue);

            return value;
        }

        public object GetValueFromMarkupExtension(WorkflowMarkupSerializationManager serializationManager, object extension)
        {
            object value = extension;
            if (extension is MarkupExtension markupExtension)
                value = markupExtension.ProvideValue(serializationManager);
            return value;
        }
    }
}
