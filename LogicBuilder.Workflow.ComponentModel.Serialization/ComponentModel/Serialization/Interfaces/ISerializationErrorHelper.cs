using System;
using System.Xml;

namespace LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces
{
    internal interface ISerializationErrorHelper
    {
        WorkflowMarkupSerializationException CreateSerializationError(Exception e, XmlReader reader);
        WorkflowMarkupSerializationException CreateSerializationError(string message, XmlReader reader);
        WorkflowMarkupSerializationException CreateSerializationError(string message, Exception e, XmlReader reader);
    }
}
