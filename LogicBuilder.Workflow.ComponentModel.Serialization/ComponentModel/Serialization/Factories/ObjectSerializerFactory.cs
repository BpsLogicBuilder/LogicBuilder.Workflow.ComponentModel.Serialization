using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;

namespace LogicBuilder.Workflow.ComponentModel.Serialization.Factories
{
    internal static class ObjectSerializerFactory
    {
        public static IObjectSerializer Create()
        {
            return new ObjectSerializer
            (
                DeserializeFromStringHelperFactory.Create(DependencyHelperFactory.Create()), 
                MarkupExtensionHelperFactory.Create(), 
                SerializationErrorHelperFactory.Create(), 
                WorkflowMarkupSerializationHelperFactory.Create()
            );
        }
    }
}
