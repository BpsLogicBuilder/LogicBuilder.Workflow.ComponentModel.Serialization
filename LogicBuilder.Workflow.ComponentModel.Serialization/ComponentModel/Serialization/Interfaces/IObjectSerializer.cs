using System.Xml;

namespace LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces
{
    internal interface IObjectSerializer
    {
        void SerializeContents(WorkflowMarkupSerializationManager serializationManager, object obj, XmlWriter writer, bool dictionaryKey);
        void SerializeObject(WorkflowMarkupSerializationManager serializationManager, object obj, XmlWriter writer);
    }
}
