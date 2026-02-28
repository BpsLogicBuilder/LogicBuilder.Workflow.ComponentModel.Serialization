namespace LogicBuilder.Workflow.ComponentModel.Serialization.Factories
{
    internal static class SimplePropertyDeserializerFactory
    {
        public static SimplePropertyDeserializer Create()
        {
            return new SimplePropertyDeserializer(MarkupExtensionHelperFactory.Create(), SerializationErrorHelperFactory.Create());
        }
    }
}
