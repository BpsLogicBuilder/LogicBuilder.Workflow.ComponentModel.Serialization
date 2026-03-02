using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;

namespace LogicBuilder.Workflow.ComponentModel.Serialization.Factories
{
    internal static class ObjectDeserializerFactory
    {
        public static IObjectDeserializer Create()
        { 
            return new ObjectDeserializer
            (
                MarkupExtensionHelperFactory.Create(),
                SerializationErrorHelperFactory.Create(),
                SimplePropertyDeserializerFactory.Create(),
                WorkflowMarkupSerializationHelperFactory.Create(),
                DeserializeFromStringHelperFactory.Create(DependencyHelperFactory.Create())
            );
        }
    }
}
