namespace LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces
{
    internal interface IMarkupExtensionHelper
    {
        object GetMarkupExtensionFromValue(object value);
        object GetValueFromMarkupExtension(WorkflowMarkupSerializationManager serializationManager, object extension);
    }
}
