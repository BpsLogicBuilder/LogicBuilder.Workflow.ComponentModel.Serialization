using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;

namespace LogicBuilder.Workflow.ComponentModel.Serialization
{
    internal class MarkupExtensionHelper() : IMarkupExtensionHelper
    {
        public object GetValueFromMarkupExtension(WorkflowMarkupSerializationManager serializationManager, object extension)
        {
            object value = extension;
            if (extension is MarkupExtension markupExtension)
                value = markupExtension.ProvideValue(serializationManager);
            return value;
        }
    }
}
