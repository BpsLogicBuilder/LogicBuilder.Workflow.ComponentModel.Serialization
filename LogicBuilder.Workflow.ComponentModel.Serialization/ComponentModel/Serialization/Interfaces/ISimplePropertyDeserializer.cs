using System.Xml;

namespace LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces
{
    internal interface ISimplePropertyDeserializer
    {
        void DeserializeSimpleProperty(WorkflowMarkupSerializationManager serializationManager, XmlReader reader, object obj, string value);
    }
}
