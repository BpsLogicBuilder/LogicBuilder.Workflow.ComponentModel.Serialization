namespace LogicBuilder.Workflow.ComponentModel.Serialization.Structures
{
    internal readonly struct ContentInfo(object content, int lineNumber, int linePosition)
    {
        public readonly int LineNumber = lineNumber;
        public readonly int LinePosition = linePosition;
        public readonly object Content = content;
    }
}
