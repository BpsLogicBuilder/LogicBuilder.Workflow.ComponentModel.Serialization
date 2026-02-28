using LogicBuilder.Workflow.ComponentModel.Serialization.Factories;
using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;

namespace LogicBuilder.Workflow.ComponentModel.Serialization
{
    internal class DependencyHelper : IDependencyHelper
    {
        public DependencyHelper()
        {
            DeserializeFromStringHelper = DeserializeFromStringHelperFactory.Create(this);
            FromCompactFormatDeserializer = FromCompactFormatDeserializerFactory.Create(this);
        }

        public IDeserializeFromStringHelper DeserializeFromStringHelper {  get; }

        public IFromCompactFormatDeserializer FromCompactFormatDeserializer { get; }
    }
}
