using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;

namespace LogicBuilder.Workflow.ComponentModel.Serialization.Factories
{
    internal static class SerializationErrorHelperFactory
    {
        internal static ISerializationErrorHelper Create()
        {
            return new SerializationErrorHelper();
        }
    }
}
