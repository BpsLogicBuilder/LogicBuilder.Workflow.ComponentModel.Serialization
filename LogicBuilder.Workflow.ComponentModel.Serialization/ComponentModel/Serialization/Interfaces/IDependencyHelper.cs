namespace LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces
{
    internal interface IDependencyHelper
    {
        IDeserializeFromStringHelper DeserializeFromStringHelper { get; }
        IFromCompactFormatDeserializer FromCompactFormatDeserializer { get; }
    }
}
