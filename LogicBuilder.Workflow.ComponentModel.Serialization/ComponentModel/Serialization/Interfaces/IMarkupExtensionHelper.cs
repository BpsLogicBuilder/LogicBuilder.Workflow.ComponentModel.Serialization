namespace LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces
{
    internal interface IMarkupExtensionHelper
    {
        object GetValueFromMarkupExtension(WorkflowMarkupSerializationManager serializationManager, object extension);
    }
}
