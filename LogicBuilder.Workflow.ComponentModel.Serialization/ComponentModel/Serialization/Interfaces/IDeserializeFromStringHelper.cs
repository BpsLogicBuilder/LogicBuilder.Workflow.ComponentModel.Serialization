using System;

namespace LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces
{
    internal interface IDeserializeFromStringHelper
    {
        object DeserializeFromString(WorkflowMarkupSerializationManager serializationManager, Type propertyType, string value);
    }
}
