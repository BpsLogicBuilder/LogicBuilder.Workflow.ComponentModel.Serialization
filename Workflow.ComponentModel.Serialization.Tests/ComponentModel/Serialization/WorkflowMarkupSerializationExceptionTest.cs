using LogicBuilder.Workflow.ComponentModel.Serialization;
using System;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization
{
    public class WorkflowMarkupSerializationExceptionTest
    {
        [Fact]
        public void Constructor_WithMessageLineAndColumn_SetsProperties()
        {
            var ex = new WorkflowMarkupSerializationException("Test message", 42, 7);

            Assert.Equal("Test message", ex.Message);
            Assert.Equal(42, ex.LineNumber);
            Assert.Equal(7, ex.LinePosition);
        }

        [Fact]
        public void Constructor_WithMessageInnerLineAndColumn_SetsProperties()
        {
            var inner = new Exception("Inner");
            var ex = new WorkflowMarkupSerializationException("Test message", inner, 10, 20);

            Assert.Equal("Test message", ex.Message);
            Assert.Equal(inner, ex.InnerException);
            Assert.Equal(10, ex.LineNumber);
            Assert.Equal(20, ex.LinePosition);
        }

        [Fact]
        public void Constructor_WithMessageAndInnerException_SetsProperties()
        {
            var inner = new Exception("Inner");
            var ex = new WorkflowMarkupSerializationException("Test message", inner);

            Assert.Equal("Test message", ex.Message);
            Assert.Equal(inner, ex.InnerException);
            Assert.Equal(-1, ex.LineNumber);
            Assert.Equal(-1, ex.LinePosition);
        }

        [Fact]
        public void Constructor_WithMessage_SetsProperties()
        {
            var ex = new WorkflowMarkupSerializationException("Test message");

            Assert.Equal("Test message", ex.Message);
            Assert.Equal(-1, ex.LineNumber);
            Assert.Equal(-1, ex.LinePosition);
        }

        [Fact]
        public void Constructor_Default_SetsDefaults()
        {
            var ex = new WorkflowMarkupSerializationException();

            Assert.Equal(-1, ex.LineNumber);
            Assert.Equal(-1, ex.LinePosition);
        }
    }
}