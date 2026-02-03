using LogicBuilder.Workflow.ComponentModel.Serialization;
using System;
using System.IO;
using System.Xml;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization
{
    public class WorkflowMarkupElementEventArgsTest
    {
        [Fact]
        public void Constructor_SetsXmlReaderProperty()
        {
            // Arrange
            using var stringReader = new StringReader("<root><child /></root>");
            using var xmlReader = XmlReader.Create(stringReader);
            // Act
            var args = new WorkflowMarkupElementEventArgs(xmlReader);

            // Assert
            Assert.Same(xmlReader, args.XmlReader);
        }

        [Fact]
        public void XmlReader_ReturnsNull_WhenConstructedWithNull()
        {
            // Act
            var args = new WorkflowMarkupElementEventArgs(null);

            // Assert
            Assert.Null(args.XmlReader);
        }

        [Fact]
        public void WorkflowMarkupElementEventArgs_IsSealed()
        {
            // Assert
            Assert.True(typeof(WorkflowMarkupElementEventArgs).IsSealed);
        }

        [Fact]
        public void WorkflowMarkupElementEventArgs_InheritsEventArgs()
        {
            // Assert
            Assert.True(typeof(EventArgs).IsAssignableFrom(typeof(WorkflowMarkupElementEventArgs)));
        }
    }
}