namespace LogicBuilder.Workflow.ComponentModel.Serialization
{
    using System;
    using System.Runtime.Serialization;

    #region Class WorkflowMarkupSerializationException
    [Serializable()]
    public class WorkflowMarkupSerializationException : Exception
    {
        private readonly int lineNumber = -1;
        private readonly int columnNumber = -1;

        public WorkflowMarkupSerializationException(string message, int lineNumber, int columnNumber)
            : base(message)
        {
            this.lineNumber = lineNumber;
            this.columnNumber = columnNumber;
        }

        public WorkflowMarkupSerializationException(string message, Exception innerException, int lineNumber, int columnNumber)
            : base(message, innerException)
        {
            this.lineNumber = lineNumber;
            this.columnNumber = columnNumber;
        }

        public WorkflowMarkupSerializationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public WorkflowMarkupSerializationException(string message)
            : base(message)
        {
        }

        public WorkflowMarkupSerializationException()
            : base()
        {
        }

        public int LineNumber
        {
            get
            {
                return this.lineNumber;
            }
        }

        public int LinePosition
        {
            get
            {
                return this.columnNumber;
            }
        }
    }
    #endregion
}
