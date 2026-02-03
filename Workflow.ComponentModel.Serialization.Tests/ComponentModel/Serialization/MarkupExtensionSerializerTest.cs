using LogicBuilder.ComponentModel.Design.Serialization;
using LogicBuilder.Workflow.ComponentModel.Design;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Xunit;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization
{
    public class MarkupExtensionSerializerTest
    {
        private readonly TestableMarkupExtensionSerializer _serializer;
        private readonly WorkflowMarkupSerializationManager _serializationManager;

        public MarkupExtensionSerializerTest()
        {
            _serializer = new TestableMarkupExtensionSerializer();
            _serializationManager = new WorkflowMarkupSerializationManager(new DesignerSerializationManager());
        }

        #region CanSerializeToString Tests

        [Fact]
        public void CanSerializeToString_ReturnsTrue_WhenValueIsMarkupExtension()
        {
            // Arrange
            var markupExtension = new TestMarkupExtension();

            // Act
            bool result = _serializer.CanSerializeToString(_serializationManager, markupExtension);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanSerializeToString_ReturnsTrue_WhenValueIsString()
        {
            // Arrange
            var value = "test";

            // Act
            bool result = _serializer.CanSerializeToString(_serializationManager, value);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanSerializeToString_ReturnsTrue_WhenValueIsNull()
        {
            // Act
            bool result = _serializer.CanSerializeToString(_serializationManager, null);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region SerializeToString Tests

        [Fact]
        public void SerializeToString_ThrowsArgumentNullException_WhenSerializationManagerIsNull()
        {
            // Arrange
            var markupExtension = new TestMarkupExtension();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _serializer.SerializeToString(null, markupExtension));
        }

        [Fact]
        public void SerializeToString_ThrowsArgumentNullException_WhenXmlWriterIsNull()
        {
            // Arrange
            var markupExtension = new TestMarkupExtension();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _serializer.SerializeToString(_serializationManager, markupExtension));
        }

        [Fact]
        public void SerializeToString_ThrowsArgumentNullException_WhenValueIsNull()
        {
            // Arrange
            var sb = new StringBuilder();
            var writer = XmlWriter.Create(sb);
            _serializationManager.WorkflowMarkupStack.Push(writer);

            try
            {
                // Act & Assert
                Assert.Throws<ArgumentNullException>(() => _serializer.SerializeToString(_serializationManager, null));
            }
            finally
            {
                _serializationManager.WorkflowMarkupStack.Pop();
                writer.Dispose();
            }
        }

        private sealed class WellKnownTypeSerializationProvider : IDesignerSerializationProvider
        {
            #region IDesignerSerializationProvider Members
            object? IDesignerSerializationProvider.GetSerializer(IDesignerSerializationManager manager, object? currentSerializer, Type? objectType, Type serializerType)
            {
                if (serializerType == typeof(WorkflowMarkupSerializer) && objectType != null)
                {
                    if (typeof(ICollection<string>).IsAssignableFrom(objectType) && objectType.IsAssignableFrom(typeof(List<string>)) && !typeof(Array).IsAssignableFrom(objectType))
                        return new StringCollectionMarkupSerializer();
                    else if (typeof(Color).IsAssignableFrom(objectType))
                        return new ColorMarkupSerializer();
                    else if (typeof(Size).IsAssignableFrom(objectType))
                        return new SizeMarkupSerializer();
                    else if (typeof(Point).IsAssignableFrom(objectType))
                        return new PointMarkupSerializer();
                    else if (objectType == typeof(CodeTypeReference))
                        return new CodeTypeReferenceSerializer();
                }

                return null;
            }
            #endregion
        }

        //[Fact]
        //public void SerializeToString_SerializesSimpleMarkupExtension()
        //{
        //    // Arrange
        //    var markupExtension = new TestMarkupExtension();
        //    var sb = new StringBuilder();
        //    var settings = new XmlWriterSettings { OmitXmlDeclaration = true };
        //    var writer = XmlWriter.Create(sb, settings);


        //    _serializationManager.WorkflowMarkupStack.Push(writer);

        //    try
        //    {
        //        writer.WriteStartElement("Root");

                
        //        using (((DesignerSerializationManager)_serializationManager.SerializationManager).CreateSession())
        //        {
        //            // Act
        //            _serializer.SerializeToString(_serializationManager, markupExtension);
        //        }

        //        writer.WriteEndElement();
        //        writer.Flush();

        //        // Assert
        //        var result = sb.ToString();
        //        Assert.Contains("{", result);
        //        Assert.Contains("}", result);
        //    }
        //    finally
        //    {
        //        _serializationManager.WorkflowMarkupStack.Pop();
        //        writer.Dispose();
        //    }
        //}

        //[Fact]
        //public void SerializeToString_SerializesMarkupExtensionWithProperties()
        //{
        //    // Arrange
        //    var markupExtension = new TestMarkupExtensionWithProperties
        //    {
        //        StringProperty = "test",
        //        IntProperty = 42
        //    };
        //    var sb = new StringBuilder();
        //    var settings = new XmlWriterSettings { OmitXmlDeclaration = true };
        //    var writer = XmlWriter.Create(sb, settings);
        //    _serializationManager.WorkflowMarkupStack.Push(writer);

        //    try
        //    {
        //        writer.WriteStartElement("Root");

        //        // Act
        //        _serializer.SerializeToString(_serializationManager, markupExtension);

        //        writer.WriteEndElement();
        //        writer.Flush();

        //        // Assert
        //        var result = sb.ToString();
        //        Assert.Contains("{", result);
        //        Assert.Contains("}", result);
        //    }
        //    finally
        //    {
        //        _serializationManager.WorkflowMarkupStack.Pop();
        //        writer.Dispose();
        //    }
        //}

        //[Fact]
        //public void SerializeToString_EscapesSpecialCharacters()
        //{
        //    // Arrange
        //    var markupExtension = new TestMarkupExtensionWithProperties
        //    {
        //        StringProperty = "test=value,with{special}chars"
        //    };
        //    var sb = new StringBuilder();
        //    var settings = new XmlWriterSettings { OmitXmlDeclaration = true };
        //    var writer = XmlWriter.Create(sb, settings);
        //    _serializationManager.WorkflowMarkupStack.Push(writer);

        //    try
        //    {
        //        _serializer.Serialize(writer, markupExtension);
        //        writer.WriteStartElement("Root");

        //        // Act
        //        //MarkupExtensionSerializer markupExtensionSerializer = new MarkupExtensionSerializer();
        //        _serializer.SerializeToString(_serializationManager, markupExtension);

        //        writer.WriteEndElement();
        //        writer.Flush();

        //        // Assert
        //        var result = sb.ToString();
        //        Assert.Contains("\\", result); // Should contain escape characters
        //    }
        //    finally
        //    {
        //        _serializationManager.WorkflowMarkupStack.Pop();
        //        writer.Dispose();
        //    }
        //}

        #endregion

        #region GetInstanceDescriptor Tests

        [Fact]
        public void GetInstanceDescriptor_ThrowsArgumentException_WhenValueIsNotMarkupExtension()
        {
            // Arrange
            var value = "not a markup extension";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _serializer.TestGetInstanceDescriptor(_serializationManager, value));
            Assert.Contains("MarkupExtension", exception.Message);
        }

        [Fact]
        public void GetInstanceDescriptor_ReturnsInstanceDescriptor_WhenValueIsMarkupExtension()
        {
            // Arrange
            var markupExtension = new TestMarkupExtension();

            // Act
            var result = _serializer.TestGetInstanceDescriptor(_serializationManager, markupExtension);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<InstanceDescriptor>(result);
        }

        [Fact]
        public void GetInstanceDescriptor_ReturnsDefaultConstructorDescriptor()
        {
            // Arrange
            var markupExtension = new TestMarkupExtension();

            // Act
            var result = _serializer.TestGetInstanceDescriptor(_serializationManager, markupExtension);

            // Assert
            Assert.NotNull(result.MemberInfo);
            var ctorInfo = result.MemberInfo as ConstructorInfo;
            Assert.Empty(ctorInfo!.GetParameters());
        }

        #endregion

        #region CreateEscapedValue Tests

        [Fact]
        public void CreateEscapedValue_ThrowsArgumentNullException_WhenValueIsNull()
        {
            // Act & Assert
            var exception = Assert.Throws<System.Reflection.TargetInvocationException>(() => _serializer.TestCreateEscapedValue(null!));
            Assert.IsType<ArgumentNullException>(exception.InnerException);
        }

        [Fact]
        public void CreateEscapedValue_ReturnsOriginalString_WhenNoSpecialCharacters()
        {
            // Arrange
            var value = "simplestring";

            // Act
            var result = _serializer.TestCreateEscapedValue(value);

            // Assert
            Assert.Equal(value, result);
        }

        [Fact]
        public void CreateEscapedValue_EscapesEquals()
        {
            // Arrange
            var value = "test=value";

            // Act
            var result = _serializer.TestCreateEscapedValue(value);

            // Assert
            Assert.Contains("\\=", result);
        }

        [Fact]
        public void CreateEscapedValue_EscapesComma()
        {
            // Arrange
            var value = "test,value";

            // Act
            var result = _serializer.TestCreateEscapedValue(value);

            // Assert
            Assert.Contains("\\,", result);
        }

        [Fact]
        public void CreateEscapedValue_EscapesCurlyBraces()
        {
            // Arrange
            var value = "test{value}";

            // Act
            var result = _serializer.TestCreateEscapedValue(value);

            // Assert
            Assert.Contains("\\{", result);
            Assert.Contains("\\}", result);
        }

        [Fact]
        public void CreateEscapedValue_EscapesQuotes()
        {
            // Arrange
            var value = "test\"value'";

            // Act
            var result = _serializer.TestCreateEscapedValue(value);

            // Assert
            Assert.Contains("\\\"", result);
            Assert.Contains("\\'", result);
        }

        [Fact]
        public void CreateEscapedValue_EscapesBackslash()
        {
            // Arrange
            var value = "test\\value";

            // Act
            var result = _serializer.TestCreateEscapedValue(value);

            // Assert
            Assert.Contains("\\\\", result);
        }

        [Fact]
        public void CreateEscapedValue_EscapesMultipleCharacters()
        {
            // Arrange
            var value = "test=value,with{special}chars";

            // Act
            var result = _serializer.TestCreateEscapedValue(value);

            // Assert
            Assert.Contains("\\=", result);
            Assert.Contains("\\,", result);
            Assert.Contains("\\{", result);
            Assert.Contains("\\}", result);
        }

        [Fact]
        public void CreateEscapedValue_ReturnsEmptyString_WhenInputIsEmpty()
        {
            // Arrange
            var value = "";

            // Act
            var result = _serializer.TestCreateEscapedValue(value);

            // Assert
            Assert.Equal("", result);
        }

        #endregion

        #region Test Helper Classes

        // Testable wrapper to expose protected methods
        private class TestableMarkupExtensionSerializer : MarkupExtensionSerializer
        {
            public InstanceDescriptor TestGetInstanceDescriptor(WorkflowMarkupSerializationManager manager, object value)
            {
                return GetInstanceDescriptor(manager, value);
            }

            public string TestCreateEscapedValue(string value)
            {
                // Use reflection to access private method
                var method = typeof(MarkupExtensionSerializer).GetMethod("CreateEscapedValue",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                return (string)method!.Invoke(this, [value])!;
            }
        }

        // Test markup extension classes
        private class TestMarkupExtension : MarkupExtension
        {
            public TestMarkupExtension() { }

            public override object ProvideValue(IServiceProvider provider)
            {
                return this;
            }
        }

        private class TestMarkupExtensionWithProperties : MarkupExtension
        {
            public TestMarkupExtensionWithProperties() { }

            public string StringProperty { get; set; } = string.Empty;

            public int IntProperty { get; set; }

            public override object ProvideValue(IServiceProvider provider)
            {
                return this;
            }
        }

        private class TestMarkupExtensionWithConstructor(string value) : MarkupExtension
        {
            private readonly string _value = value;

            [ConstructorArgument("value")]
            public string Value
            {
                get { return _value; }
            }

            public override object ProvideValue(IServiceProvider provider)
            {
                return _value;
            }
        }

        #endregion
    }
}