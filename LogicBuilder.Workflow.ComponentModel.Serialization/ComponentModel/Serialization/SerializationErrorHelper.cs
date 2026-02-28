using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;
using System;
using System.Xml;

namespace LogicBuilder.Workflow.ComponentModel.Serialization
{
    internal class SerializationErrorHelper : ISerializationErrorHelper
    {
        public WorkflowMarkupSerializationException CreateSerializationError(Exception e, XmlReader reader)
        {
            return CreateSerializationError(null, e, reader);
        }

        public WorkflowMarkupSerializationException CreateSerializationError(string message, XmlReader reader)
        {
            return CreateSerializationError(message, null, reader);
        }

        public WorkflowMarkupSerializationException CreateSerializationError(string message, Exception e, XmlReader reader)
        {
            string errorMsg = message;
            if (string.IsNullOrEmpty(errorMsg))
                errorMsg = e?.Message ?? string.Empty;

            return reader is IXmlLineInfo xmlLineInfo
                ? new WorkflowMarkupSerializationException(errorMsg, xmlLineInfo.LineNumber, xmlLineInfo.LinePosition)
                : new WorkflowMarkupSerializationException(errorMsg, 0, 0);
        }
    }
}
