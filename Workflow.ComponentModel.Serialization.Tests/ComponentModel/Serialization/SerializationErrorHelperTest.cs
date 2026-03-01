using LogicBuilder.Workflow.ComponentModel.Serialization;
using System;
using System.IO;
using System.Xml;
using Xunit;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization
{
    public class SerializationErrorHelperTest
    {
        private readonly SerializationErrorHelper _helper;

        public SerializationErrorHelperTest()
        {
            _helper = new SerializationErrorHelper();
        }

        #region CreateSerializationError(Exception, XmlReader)

        [Fact]
        public void CreateSerializationError_WithException_AndXmlReaderWithLineInfo_ReturnsExceptionWithLineInfo()
        {
            // Arrange
            var exception = new InvalidOperationException("Test exception message");
            var xmlReader = CreateXmlReaderWithLineInfo("<root />", 5, 10);

            // Act
            var result = _helper.CreateSerializationError(exception, xmlReader);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test exception message", result.Message);
            Assert.Equal(5, result.LineNumber);
            Assert.Equal(10, result.LinePosition);
        }

        [Fact]
        public void CreateSerializationError_WithException_AndXmlReaderWithoutLineInfo_ReturnsExceptionWithZeroLineInfo()
        {
            // Arrange
            var exception = new InvalidOperationException("Test exception message");
            var xmlReader = CreateXmlReaderWithoutLineInfo("<root />");

            // Act
            var result = _helper.CreateSerializationError(exception, xmlReader);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test exception message", result.Message);
            Assert.Equal(0, result.LineNumber);
            Assert.Equal(0, result.LinePosition);
        }

        [Fact]
        public void CreateSerializationError_WithNullException_AndXmlReader_ReturnsExceptionWithEmptyMessage()
        {
            // Arrange
            Exception? exception = null;
            var xmlReader = CreateXmlReaderWithLineInfo("<root />", 1, 1);

            // Act
            var result = _helper.CreateSerializationError(exception, xmlReader);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Message);
            Assert.Equal(1, result.LineNumber);
            Assert.Equal(1, result.LinePosition);
        }

        #endregion

        #region CreateSerializationError(string, XmlReader)

        [Fact]
        public void CreateSerializationError_WithMessage_AndXmlReaderWithLineInfo_ReturnsExceptionWithLineInfo()
        {
            // Arrange
            var message = "Custom error message";
            var xmlReader = CreateXmlReaderWithLineInfo("<root />", 3, 7);

            // Act
            var result = _helper.CreateSerializationError(message, xmlReader);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Custom error message", result.Message);
            Assert.Equal(3, result.LineNumber);
            Assert.Equal(7, result.LinePosition);
        }

        [Fact]
        public void CreateSerializationError_WithMessage_AndXmlReaderWithoutLineInfo_ReturnsExceptionWithZeroLineInfo()
        {
            // Arrange
            var message = "Custom error message";
            var xmlReader = CreateXmlReaderWithoutLineInfo("<root />");

            // Act
            var result = _helper.CreateSerializationError(message, xmlReader);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Custom error message", result.Message);
            Assert.Equal(0, result.LineNumber);
            Assert.Equal(0, result.LinePosition);
        }

        [Fact]
        public void CreateSerializationError_WithNullMessage_AndXmlReader_ReturnsExceptionWithEmptyMessage()
        {
            // Arrange
            string? message = null;
            var xmlReader = CreateXmlReaderWithLineInfo("<root />", 2, 5);

            // Act
            var result = _helper.CreateSerializationError(message, xmlReader);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Message);
            Assert.Equal(2, result.LineNumber);
            Assert.Equal(5, result.LinePosition);
        }

        [Fact]
        public void CreateSerializationError_WithEmptyMessage_AndXmlReader_ReturnsExceptionWithEmptyMessage()
        {
            // Arrange
            var message = string.Empty;
            var xmlReader = CreateXmlReaderWithLineInfo("<root />", 4, 2);

            // Act
            var result = _helper.CreateSerializationError(message, xmlReader);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Message);
            Assert.Equal(4, result.LineNumber);
            Assert.Equal(2, result.LinePosition);
        }

        #endregion

        #region CreateSerializationError(string, Exception, XmlReader)

        [Fact]
        public void CreateSerializationError_WithMessageAndException_AndXmlReaderWithLineInfo_UsesMessage()
        {
            // Arrange
            var message = "Custom error message";
            var exception = new InvalidOperationException("Exception message");
            var xmlReader = CreateXmlReaderWithLineInfo("<root />", 6, 12);

            // Act
            var result = _helper.CreateSerializationError(message, exception, xmlReader);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Custom error message", result.Message);
            Assert.Equal(6, result.LineNumber);
            Assert.Equal(12, result.LinePosition);
        }

        [Fact]
        public void CreateSerializationError_WithNullMessageAndException_AndXmlReader_UsesExceptionMessage()
        {
            // Arrange
            string? message = null;
            var exception = new InvalidOperationException("Exception message");
            var xmlReader = CreateXmlReaderWithLineInfo("<root />", 8, 3);

            // Act
            var result = _helper.CreateSerializationError(message, exception, xmlReader);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Exception message", result.Message);
            Assert.Equal(8, result.LineNumber);
            Assert.Equal(3, result.LinePosition);
        }

        [Fact]
        public void CreateSerializationError_WithEmptyMessageAndException_AndXmlReader_UsesExceptionMessage()
        {
            // Arrange
            var message = string.Empty;
            var exception = new InvalidOperationException("Exception message");
            var xmlReader = CreateXmlReaderWithLineInfo("<root />", 7, 9);

            // Act
            var result = _helper.CreateSerializationError(message, exception, xmlReader);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Exception message", result.Message);
            Assert.Equal(7, result.LineNumber);
            Assert.Equal(9, result.LinePosition);
        }

        [Fact]
        public void CreateSerializationError_WithNullMessageAndNullException_AndXmlReader_ReturnsEmptyMessage()
        {
            // Arrange
            string? message = null;
            Exception? exception = null;
            var xmlReader = CreateXmlReaderWithLineInfo("<root />", 10, 15);

            // Act
            var result = _helper.CreateSerializationError(message, exception, xmlReader);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Message);
            Assert.Equal(10, result.LineNumber);
            Assert.Equal(15, result.LinePosition);
        }

        [Fact]
        public void CreateSerializationError_WithMessageExceptionAndXmlReaderWithoutLineInfo_UsesMessage()
        {
            // Arrange
            var message = "Custom error message";
            var exception = new InvalidOperationException("Exception message");
            var xmlReader = CreateXmlReaderWithoutLineInfo("<root />");

            // Act
            var result = _helper.CreateSerializationError(message, exception, xmlReader);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Custom error message", result.Message);
            Assert.Equal(0, result.LineNumber);
            Assert.Equal(0, result.LinePosition);
        }

        #endregion

        #region Helper Methods

        private static MockXmlReaderWithLineInfo CreateXmlReaderWithLineInfo(string xml, int lineNumber, int linePosition)
        {
            var stringReader = new StringReader(xml);
            var xmlReader = XmlReader.Create(stringReader);
            xmlReader.Read(); // Move to first element

            // Create a mock XmlReader with IXmlLineInfo
            return new MockXmlReaderWithLineInfo(xmlReader, lineNumber, linePosition);
        }

        private static MockXmlReaderWithoutLineInfo CreateXmlReaderWithoutLineInfo(string xml)
        {
            var stringReader = new StringReader(xml);
            var settings = new XmlReaderSettings
            {
                IgnoreWhitespace = true
            };
            var xmlReader = XmlReader.Create(stringReader, settings);
            xmlReader.Read(); // Move to first element

            // Wrap in a reader that doesn't implement IXmlLineInfo
            return new MockXmlReaderWithoutLineInfo(xmlReader);
        }

        #endregion

        #region Mock Classes

        private class MockXmlReaderWithLineInfo(XmlReader innerReader, int lineNumber, int linePosition) : XmlReader, IXmlLineInfo
        {
            private readonly XmlReader _innerReader = innerReader;
            private readonly int _lineNumber = lineNumber;
            private readonly int _linePosition = linePosition;

            public int LineNumber => _lineNumber;
            public int LinePosition => _linePosition;
            public bool HasLineInfo() => true;

            public override XmlNodeType NodeType => _innerReader.NodeType;
            public override string LocalName => _innerReader.LocalName;
            public override string NamespaceURI => _innerReader.NamespaceURI;
            public override string Prefix => _innerReader.Prefix;
            public override string Value => _innerReader.Value;
            public override int Depth => _innerReader.Depth;
            public override string BaseURI => _innerReader.BaseURI;
            public override bool IsEmptyElement => _innerReader.IsEmptyElement;
            public override int AttributeCount => _innerReader.AttributeCount;
            public override bool EOF => _innerReader.EOF;
            public override ReadState ReadState => _innerReader.ReadState;
            public override XmlNameTable NameTable => _innerReader.NameTable;

            public override string GetAttribute(string name) => _innerReader.GetAttribute(name)!;
            public override string GetAttribute(string name, string? namespaceURI) => _innerReader.GetAttribute(name, namespaceURI)!;
            public override string GetAttribute(int i) => _innerReader.GetAttribute(i);
            public override bool MoveToAttribute(string name) => _innerReader.MoveToAttribute(name);
            public override bool MoveToAttribute(string name, string? ns) => _innerReader.MoveToAttribute(name, ns);
            public override bool MoveToFirstAttribute() => _innerReader.MoveToFirstAttribute();
            public override bool MoveToNextAttribute() => _innerReader.MoveToNextAttribute();
            public override bool MoveToElement() => _innerReader.MoveToElement();
            public override bool Read() => _innerReader.Read();
            public override bool ReadAttributeValue() => _innerReader.ReadAttributeValue();
            public override string LookupNamespace(string prefix) => _innerReader.LookupNamespace(prefix)!;
            public override void ResolveEntity() => _innerReader.ResolveEntity();
        }

        private class MockXmlReaderWithoutLineInfo(XmlReader innerReader) : XmlReader
        {
            private readonly XmlReader _innerReader = innerReader;

            public override XmlNodeType NodeType => _innerReader.NodeType;
            public override string LocalName => _innerReader.LocalName;
            public override string NamespaceURI => _innerReader.NamespaceURI;
            public override string Prefix => _innerReader.Prefix;
            public override string Value => _innerReader.Value;
            public override int Depth => _innerReader.Depth;
            public override string BaseURI => _innerReader.BaseURI;
            public override bool IsEmptyElement => _innerReader.IsEmptyElement;
            public override int AttributeCount => _innerReader.AttributeCount;
            public override bool EOF => _innerReader.EOF;
            public override ReadState ReadState => _innerReader.ReadState;
            public override XmlNameTable NameTable => _innerReader.NameTable;

            public override string GetAttribute(string name) => _innerReader.GetAttribute(name)!;
            public override string GetAttribute(string name, string? namespaceURI) => _innerReader.GetAttribute(name, namespaceURI)!;
            public override string GetAttribute(int i) => _innerReader.GetAttribute(i);
            public override bool MoveToAttribute(string name) => _innerReader.MoveToAttribute(name);
            public override bool MoveToAttribute(string name, string? ns) => _innerReader.MoveToAttribute(name, ns);
            public override bool MoveToFirstAttribute() => _innerReader.MoveToFirstAttribute();
            public override bool MoveToNextAttribute() => _innerReader.MoveToNextAttribute();
            public override bool MoveToElement() => _innerReader.MoveToElement();
            public override bool Read() => _innerReader.Read();
            public override bool ReadAttributeValue() => _innerReader.ReadAttributeValue();
            public override string LookupNamespace(string prefix) => _innerReader.LookupNamespace(prefix)!;
            public override void ResolveEntity() => _innerReader.ResolveEntity();
        }

        #endregion
    }
}