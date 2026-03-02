using System.Xml;

namespace LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces
{
    internal interface IObjectDeserializer
    {
        object DeserializeObject(WorkflowMarkupSerializationManager serializationManager, XmlReader reader);
        object CreateInstance(WorkflowMarkupSerializationManager serializationManager, XmlQualifiedName xmlQualifiedName, XmlReader reader);
        void DeserializeCompoundProperty(WorkflowMarkupSerializationManager serializationManager, XmlReader reader, object obj);
        void DeserializeContents(WorkflowMarkupSerializationManager serializationManager, object obj, XmlReader reader);
        string GetClrFullName(WorkflowMarkupSerializationManager serializationManager, XmlQualifiedName xmlQualifiedName);
    }
}
