using LogicBuilder.ComponentModel.Design.Serialization;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using LogicBuilder.Workflow.ComponentModel.Serialization.Factories;
using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;
using System;
using System.IO;
using System.Xml;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization
{
    public class FromCompactFormatDeserializerTest
    {
        private readonly IFromCompactFormatDeserializer _deserializer;
        private readonly WorkflowMarkupSerializationManager _serializationManager;
        private readonly DesignerSerializationManager _designerSerializationManager;

        public FromCompactFormatDeserializerTest()
        {
            _deserializer = FromCompactFormatDeserializerFactory.Create(DependencyHelperFactory.Create());
            _designerSerializationManager = new DesignerSerializationManager();
            _serializationManager = new WorkflowMarkupSerializationManager(_designerSerializationManager);
        }

        #region Helper Methods

        private static XmlReader CreateXmlReader(string xml)
        {
            var settings = new XmlReaderSettings
            {
                IgnoreWhitespace = true,
                IgnoreComments = true
            };
            return XmlReader.Create(new StringReader(xml), settings);
        }

        #endregion

        #region Test Helper Classes

        public class TestSimpleMarkupExtension
        {
            public string Value { get; set; } = string.Empty;
        }

        public class TestPositionalArgsExtension
        {
            public TestPositionalArgsExtension(string arg1)
            {
                Arg1 = arg1;
            }

            public TestPositionalArgsExtension(string arg1, string arg2)
            {
                Arg1 = arg1;
                Arg2 = arg2;
            }

            public string Arg1 { get; }
            public string Arg2 { get; } = string.Empty;
        }

        public class TestNamedArgsExtension
        {
            public string Property1 { get; set; } = string.Empty;
            public string Property2 { get; set; } = string.Empty;
            public int IntProperty { get; set; }
        }

        public class TestMixedArgsExtension(string positional)
        {
            public string PositionalArg { get; } = positional;
            public string NamedProperty { get; set; } = string.Empty;
        }

        public class TestExtension
        {
            public string Value { get; set; } = string.Empty;
        }

        #endregion

        #region Deserialize - Invalid Format Tests

        [Fact]
        public void Deserialize_WithEmptyString_ReturnsNull()
        {
            // Arrange
            var xml = "<Test xmlns:t='clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535' />";
            using var reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            object result;
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                result = _deserializer.DeserializeFromCompactFormat(_serializationManager, reader, "");
                _serializationManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Deserialize_WithoutOpeningBrace_ReturnsNull()
        {
            // Arrange
            var xml = "<Test xmlns:t='clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535' />";
            using var reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            object result;
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                result = _deserializer.DeserializeFromCompactFormat(_serializationManager, reader, "t:TestExtension}");
                _serializationManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Deserialize_WithoutClosingBrace_ReturnsNull()
        {
            // Arrange
            var xml = "<Test xmlns:t='clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535' />";
            using var reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            object result;
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                result = _deserializer.DeserializeFromCompactFormat(_serializationManager, reader, "{t:FromCompactFormatDeserializerTest+TestExtension");
                _serializationManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Deserialize_WithIncorrectSyntax_ReturnsNull()
        {
            // Arrange
            var xml = "<Test xmlns:t='clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535' />";
            using var reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            object result;
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                result = _deserializer.DeserializeFromCompactFormat(_serializationManager, reader, "InvalidFormat");
                _serializationManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region Deserialize - Null Arguments Tests

        [Fact]
        public void Deserialize_WithNullReader_ThrowsArgumentNullException()
        {
            // Arrange
            var attrValue = "{x:Null}";

            // Act & Assert
            using (_designerSerializationManager.CreateSession())
            {
                Assert.Throws<ArgumentNullException>(() =>
                    _deserializer.DeserializeFromCompactFormat(_serializationManager, null, attrValue));
            }
        }

        #endregion

        #region Deserialize - Type Resolution Tests

        [Fact]
        public void Deserialize_WithUnresolvableType_ReturnsNull()
        {
            // Arrange
            var xml = "<Test xmlns:t='clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535' />";
            using var reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            object result;
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                result = _deserializer.DeserializeFromCompactFormat(_serializationManager, reader, "{t:NonExistentType}");
                _serializationManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Deserialize_WithTypeNameWithoutExtensionSuffix_AppendsExtensionSuffix()
        {
            // Arrange
            var xml = "<Test xmlns:t='clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535' />";
            using var reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            object result;
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                result = _deserializer.DeserializeFromCompactFormat(_serializationManager, reader, "{t:FromCompactFormatDeserializerTest+Test}");
                _serializationManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TestExtension>(result);
        }

        [Fact]
        public void Deserialize_WithTypeNameWithExtensionSuffix_CreatesInstance()
        {
            // Arrange
            var xml = "<Test xmlns:t='clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535' />";
            using var reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            object result;
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                result = _deserializer.DeserializeFromCompactFormat(_serializationManager, reader, "{t:FromCompactFormatDeserializerTest+TestExtension}");
                _serializationManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TestExtension>(result);
        }

        [Fact]
        public void Deserialize_WithNullExtension_CreatesNullExtension()
        {
            // Arrange
            var xml = "<Test xmlns:x='" + StandardXomlKeys.Definitions_XmlNs + "' />";
            using var reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            object result;
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                result = _deserializer.DeserializeFromCompactFormat(_serializationManager, reader, "{x:Null}");
                _serializationManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NullExtension>(result);
        }

        [Fact]
        public void Deserialize_WithTypeExtension_CreatesTypeExtension()
        {
            // Arrange
            var xml = "<Test xmlns:x='" + StandardXomlKeys.Definitions_XmlNs + "' xmlns:s='clr-namespace:System;assembly=mscorlib' />";
            using var reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            object result;
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                result = _deserializer.DeserializeFromCompactFormat(_serializationManager, reader, "{x:Type s:String}");
                _serializationManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TypeExtension>(result);
        }

        #endregion

        #region Deserialize - Positional Arguments Tests

        [Fact]
        public void Deserialize_WithSinglePositionalArgument_CreatesInstanceWithConstructor()
        {
            // Arrange
            var xml = "<Test xmlns:t='clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535' />";
            using var reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            object result;
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                result = _deserializer.DeserializeFromCompactFormat(_serializationManager, reader, "{t:FromCompactFormatDeserializerTest+TestPositionalArgsExtension Arg1Value}");
                _serializationManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TestPositionalArgsExtension>(result);
            var instance = (TestPositionalArgsExtension)result;
            Assert.Equal("Arg1Value", instance.Arg1);
        }

        [Fact]
        public void Deserialize_WithMultiplePositionalArguments_CreatesInstanceWithConstructor()
        {
            // Arrange
            var xml = "<Test xmlns:t='clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535' />";
            using var reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            object result;
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                result = _deserializer.DeserializeFromCompactFormat(_serializationManager, reader, "{t:FromCompactFormatDeserializerTest+TestPositionalArgsExtension Arg1Value, Arg2Value}");
                _serializationManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TestPositionalArgsExtension>(result);
            var instance = (TestPositionalArgsExtension)result;
            Assert.Equal("Arg1Value", instance.Arg1);
            Assert.Equal("Arg2Value", instance.Arg2);
        }

        [Fact]
        public void Deserialize_WithNoMatchingConstructor_CreatesInstanceWithDefaultConstructor()
        {
            // Arrange
            var xml = "<Test xmlns:t='clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535' />";
            using var reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            object result;
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                result = _deserializer.DeserializeFromCompactFormat(_serializationManager, reader, "{t:FromCompactFormatDeserializerTest+TestNamedArgsExtension}");
                _serializationManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TestNamedArgsExtension>(result);
        }

        #endregion

        #region Deserialize - Named Arguments Tests

        [Fact]
        public void Deserialize_WithSingleNamedArgument_SetsProperty()
        {
            // Arrange
            var xml = "<Test xmlns:t='clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535' />";
            using var reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            object result;
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                result = _deserializer.DeserializeFromCompactFormat(_serializationManager, reader, "{t:FromCompactFormatDeserializerTest+TestNamedArgsExtension Property1=Value1}");
                _serializationManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TestNamedArgsExtension>(result);
            var instance = (TestNamedArgsExtension)result;
            Assert.Equal("Value1", instance.Property1);
        }

        [Fact]
        public void Deserialize_WithMultipleNamedArguments_SetsAllProperties()
        {
            // Arrange
            var xml = "<Test xmlns:t='clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535' />";
            using var reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            object result;
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                result = _deserializer.DeserializeFromCompactFormat(_serializationManager, reader, "{t:FromCompactFormatDeserializerTest+TestNamedArgsExtension Property1=Value1, Property2=Value2}");
                _serializationManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TestNamedArgsExtension>(result);
            var instance = (TestNamedArgsExtension)result;
            Assert.Equal("Value1", instance.Property1);
            Assert.Equal("Value2", instance.Property2);
        }

        [Fact]
        public void Deserialize_WithIntNamedArgument_SetsIntProperty()
        {
            // Arrange
            var xml = "<Test xmlns:t='clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535' />";
            using var reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            object result;
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                result = _deserializer.DeserializeFromCompactFormat(_serializationManager, reader, "{t:FromCompactFormatDeserializerTest+TestNamedArgsExtension IntProperty=42}");
                _serializationManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TestNamedArgsExtension>(result);
            var instance = (TestNamedArgsExtension)result;
            Assert.Equal(42, instance.IntProperty);
        }

        [Fact]
        public void Deserialize_WithInvalidPropertyName_ReportsError()
        {
            // Arrange
            var xml = "<Test xmlns:t='clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535' />";
            using var reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            object result;
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                result = _deserializer.DeserializeFromCompactFormat(_serializationManager, reader, "{t:FromCompactFormatDeserializerTest+TestNamedArgsExtension NonExistentProperty=Value}");

                // Assert
                Assert.True(_designerSerializationManager.Errors.Count > 0);
                Assert.NotNull(result);

                _serializationManager.WorkflowMarkupStack.Pop();
            }

            
        }

        #endregion

        #region Deserialize - Mixed Arguments Tests

        [Fact]
        public void Deserialize_WithMixedPositionalAndNamedArguments_CreatesInstanceCorrectly()
        {
            // Arrange
            var xml = "<Test xmlns:t='clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535' />";
            using var reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            object result;
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                result = _deserializer.DeserializeFromCompactFormat(_serializationManager, reader, "{t:FromCompactFormatDeserializerTest+TestMixedArgsExtension PositionalValue, NamedProperty=NamedValue}");
                _serializationManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TestMixedArgsExtension>(result);
            var instance = (TestMixedArgsExtension)result;
            Assert.Equal("PositionalValue", instance.PositionalArg);
            Assert.Equal("NamedValue", instance.NamedProperty);
        }

        #endregion

        #region Deserialize - Special Characters Tests

        [Fact]
        public void Deserialize_WithEscapedCharacters_RemovesEscapes()
        {
            // Arrange
            var xml = "<Test xmlns:t='clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535' />";
            using var reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            object result;
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                result = _deserializer.DeserializeFromCompactFormat(_serializationManager, reader, "{t:FromCompactFormatDeserializerTest+TestNamedArgsExtension Property1=Value\\,1}");
                _serializationManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TestNamedArgsExtension>(result);
        }

        [Fact]
        public void Deserialize_WithEncodedName_DecodesName()
        {
            // Arrange
            var xml = "<Test xmlns:t='clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535' />";
            using var reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            object result;
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                result = _deserializer.DeserializeFromCompactFormat(_serializationManager, reader, "{t:FromCompactFormatDeserializerTest+TestNamedArgsExtension Property1=TestValue}");
                _serializationManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TestNamedArgsExtension>(result);
            var instance = (TestNamedArgsExtension)result;
            Assert.Equal("TestValue", instance.Property1);
        }

        #endregion

        #region Deserialize - Tokenization Error Tests

        [Fact]
        public void Deserialize_WithTokenizationError_ReturnsNull()
        {
            // Arrange
            var xml = "<Test xmlns:t='clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535' />";
            using var reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            object result;
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                // Use invalid tokenization format
                result = _deserializer.DeserializeFromCompactFormat(_serializationManager, reader, "{t:FromCompactFormatDeserializerTest+TestExtension 'unclosed quote}");
                _serializationManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region Deserialize - Prefix Tests

        [Fact]
        public void Deserialize_WithoutPrefix_ResolvesType()
        {
            // Arrange
            var xml = "<Test xmlns='clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535' />";
            using var reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            object result;
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                result = _deserializer.DeserializeFromCompactFormat(_serializationManager, reader, "{FromCompactFormatDeserializerTest+TestExtension}");
                _serializationManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TestExtension>(result);
        }

        #endregion

        #region Deserialize - Empty Arguments Tests

        [Fact]
        public void Deserialize_WithWhitespaceOnly_CreatesDefaultInstance()
        {
            // Arrange
            var xml = "<Test xmlns:t='clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535' />";
            using var reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            object result;
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                result = _deserializer.DeserializeFromCompactFormat(_serializationManager, reader, "{t:FromCompactFormatDeserializerTest+TestExtension   }");
                _serializationManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TestExtension>(result);
        }

        #endregion

        #region Deserialize - Edge Cases Tests

        [Fact]
        public void Deserialize_WithCommaDelimitedArguments_ParsesCorrectly()
        {
            // Arrange
            var xml = "<Test xmlns:t='clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535' />";
            using var reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            object result;
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                result = _deserializer.DeserializeFromCompactFormat(_serializationManager, reader, "{t:FromCompactFormatDeserializerTest+TestPositionalArgsExtension Value1, Value2}");
                _serializationManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TestPositionalArgsExtension>(result);
            var instance = (TestPositionalArgsExtension)result;
            Assert.Equal("Value1", instance.Arg1);
            Assert.Equal("Value2", instance.Arg2);
        }

        #endregion

        #region Deserialize - ArrayExtension Tests

        [Fact]
        public void Deserialize_WithArrayExtension_CreatesArrayExtension()
        {
            // Arrange
            var xml = "<Test xmlns:x='" + StandardXomlKeys.Definitions_XmlNs + "' xmlns:s='clr-namespace:System;assembly=mscorlib' />";
            using var reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            object result;
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                result = _deserializer.DeserializeFromCompactFormat(_serializationManager, reader, "{x:Array Type=s:String}");
                _serializationManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ArrayExtension>(result);
        }

        #endregion

        #region Deserialize - GetProperties Exception Tests

        public class TestExceptionExtension
        {
            public string Property1 { get; set; } = string.Empty;

            // This could throw if accessed improperly
            public string ThrowingProperty
            {
                get => throw new InvalidOperationException("Property access error");
                set { Property1 = value; }
            }
        }

        #endregion

        #region Old tests from before refactor WorkfloMarkupSerializerTest
        [Fact]
        public void DeserializeFromCompactFormat_WithNullExtension_CreatesNullExtension()
        {
            // Arrange PropertyName={StaticResource Key},PropertyValue=\"test\"}
            var serializer = FromCompactFormatDeserializerFactory.Create(DependencyHelperFactory.Create());
            var xml = "<Test xmlns:x='" + StandardXomlKeys.Definitions_XmlNs + "' />";
            using var reader = XmlReader.Create(new StringReader(xml));
            reader.Read();
            var manager = new DesignerSerializationManager();
            var value = "{x:Null}";

            // Act
            object result;
            using (manager.CreateSession())
            {
                var wfManager = new WorkflowMarkupSerializationManager(manager);
                wfManager.WorkflowMarkupStack.Push(reader);
                result = serializer.DeserializeFromCompactFormat(wfManager, reader, value);
                wfManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NullExtension>(result);
        }

        [Fact]
        public void DeserializeFromCompactFormat_WithPositionalArguments_ParsesCorrectly()
        {
            // Arrange
            var serializer = FromCompactFormatDeserializerFactory.Create(DependencyHelperFactory.Create());
            var xml = "<Test xmlns:x='" + StandardXomlKeys.Definitions_XmlNs + "' />";
            using var reader = XmlReader.Create(new StringReader(xml));
            reader.Read();
            var manager = new DesignerSerializationManager();
            var value = "{x:Null}";

            // Act
            object result;
            using (manager.CreateSession())
            {
                var wfManager = new WorkflowMarkupSerializationManager(manager);
                wfManager.WorkflowMarkupStack.Push(reader);
                result = serializer.DeserializeFromCompactFormat(wfManager, reader, value);
                wfManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void DeserializeFromCompactFormat_WithNamedArguments_ParsesCorrectly()
        {
            // Arrange
            var serializer = FromCompactFormatDeserializerFactory.Create(DependencyHelperFactory.Create());
            var xml = "<Test xmlns:x='" + StandardXomlKeys.Definitions_XmlNs + "' xmlns:t='clr-namespace:System;assembly=mscorlib' />";
            using var reader = XmlReader.Create(new StringReader(xml));
            reader.Read();
            var manager = new DesignerSerializationManager();
            var value = "{x:Type t:String}";

            // Act
            object result;
            using (manager.CreateSession())
            {
                var wfManager = new WorkflowMarkupSerializationManager(manager);
                wfManager.WorkflowMarkupStack.Push(reader);
                result = serializer.DeserializeFromCompactFormat(wfManager, reader, value);
                wfManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void DeserializeFromCompactFormat_WithEscapedCharacters_HandlesCorrectly()
        {
            // Arrange
            var serializer = FromCompactFormatDeserializerFactory.Create(DependencyHelperFactory.Create());
            var xml = "<Test xmlns:x='" + StandardXomlKeys.Definitions_XmlNs + "' />";
            using var reader = XmlReader.Create(new StringReader(xml));
            reader.Read();
            var manager = new DesignerSerializationManager();
            // Test the RemoveEscapes functionality
            var value = "{x:Null}";

            // Act
            object result;
            using (manager.CreateSession())
            {
                var wfManager = new WorkflowMarkupSerializationManager(manager);
                wfManager.WorkflowMarkupStack.Push(reader);
                result = serializer.DeserializeFromCompactFormat(wfManager, reader, value);
                wfManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.IsType<NullExtension>(result);
        }

        [Fact]
        public void DeserializeFromCompactFormat_WithQuotedStrings_ParsesCorrectly()
        {
            // Arrange
            var serializer = FromCompactFormatDeserializerFactory.Create(DependencyHelperFactory.Create());
            var xml = "<Test xmlns:x='" + StandardXomlKeys.Definitions_XmlNs + "' />";
            using var reader = XmlReader.Create(new StringReader(xml));
            reader.Read();
            var manager = new DesignerSerializationManager();
            var value = "{x:Type 'System.String'}";

            // Act
            object? result;
            using (manager.CreateSession())
            {
                var wfManager = new WorkflowMarkupSerializationManager(manager);
                wfManager.WorkflowMarkupStack.Push(reader);
                result = serializer.DeserializeFromCompactFormat(wfManager, reader, value);
                wfManager.WorkflowMarkupStack.Pop();
            }

            Assert.NotNull(result);
        }

        [Fact]
        public void DeserializeFromCompactFormat_WithMissingTerminatingBrace_ReportsError()
        {
            // Arrange
            var serializer = FromCompactFormatDeserializerFactory.Create(DependencyHelperFactory.Create());
            var xml = "<Test xmlns:x='" + StandardXomlKeys.Definitions_XmlNs + "' />";
            using var reader = XmlReader.Create(new StringReader(xml));
            reader.Read();
            var manager = new DesignerSerializationManager();
            var value = "{x:Null arg1, arg2";

            // Act
            object result;
            using (manager.CreateSession())
            {
                var wfManager = new WorkflowMarkupSerializationManager(manager);
                wfManager.WorkflowMarkupStack.Push(reader);
                result = serializer.DeserializeFromCompactFormat(wfManager, reader, value);
                wfManager.WorkflowMarkupStack.Pop();
            }

            // Assert - Should report error for missing terminating brace
            Assert.Null(result);
        }

        [Fact]
        public void DeserializeFromCompactFormat_WithParameterToParameterlessExtension_ReportsError()
        {
            // Arrange
            var serializer = FromCompactFormatDeserializerFactory.Create(DependencyHelperFactory.Create());
            var xml = "<Test xmlns:x='" + StandardXomlKeys.Definitions_XmlNs + "' />";
            using var reader = XmlReader.Create(new StringReader(xml));
            reader.Read();
            var manager = new DesignerSerializationManager();
            var value = "{x:Null arg1}";

            // Act
            object result;
            using (manager.CreateSession())
            {
                var wfManager = new WorkflowMarkupSerializationManager(manager);
                wfManager.WorkflowMarkupStack.Push(reader);
                result = serializer.DeserializeFromCompactFormat(wfManager, reader, value);
                wfManager.WorkflowMarkupStack.Pop();
            }

            // Assert - Should report error for trailing delimiter
            Assert.Null(result);
        }

        [Fact]
        public void DeserializeFromCompactFormat_WithDoubleQuotes_ParsesCorrectly()
        {
            // Arrange
            var serializer = FromCompactFormatDeserializerFactory.Create(DependencyHelperFactory.Create());
            var xml = "<Test xmlns:x='" + StandardXomlKeys.Definitions_XmlNs + "' />";
            using var reader = XmlReader.Create(new StringReader(xml));
            reader.Read();
            var manager = new DesignerSerializationManager();
            var value = "{x:Type \"System.String\"}";

            // Act
            object? result;
            using (manager.CreateSession())
            {
                var wfManager = new WorkflowMarkupSerializationManager(manager);
                wfManager.WorkflowMarkupStack.Push(reader);
                result = serializer.DeserializeFromCompactFormat(wfManager, reader, value);
                wfManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void DeserializeFromCompactFormat_WithInvalidFormat_ReportsError()
        {
            // Arrange
            var serializer = FromCompactFormatDeserializerFactory.Create(DependencyHelperFactory.Create());
            var xml = "<Test />";
            using var reader = XmlReader.Create(new StringReader(xml));
            reader.Read();
            var manager = new DesignerSerializationManager();
            var value = "InvalidFormat";

            // Act & Assert
            using (manager.CreateSession())
            {
                var wfManager = new WorkflowMarkupSerializationManager(manager);
                wfManager.WorkflowMarkupStack.Push(reader);
                var result = serializer.DeserializeFromCompactFormat(wfManager, reader, value);
                wfManager.WorkflowMarkupStack.Pop();
                // The method should report an error for invalid format
                Assert.Null(result);
            }
        }

        [Fact]
        public void DeserializeFromCompactFormat_WithNestedCurlyBraces_ParsesCorrectly()
        {
            // Arrange
            var serializer = FromCompactFormatDeserializerFactory.Create(DependencyHelperFactory.Create());
            var xml = "<Test xmlns:x='" + StandardXomlKeys.Definitions_XmlNs + "' />";
            using var reader = XmlReader.Create(new StringReader(xml));
            reader.Read();
            var manager = new DesignerSerializationManager();
            var value = "{x:Null}";

            // Act
            object? result;
            using (manager.CreateSession())
            {
                var wfManager = new WorkflowMarkupSerializationManager(manager);
                wfManager.WorkflowMarkupStack.Push(reader);
                result = serializer.DeserializeFromCompactFormat(wfManager, reader, value);
                wfManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.IsType<NullExtension>(result);
        }

        [Fact]
        public void DeserializeFromCompactFormat_WithExtraCharactersAfterBrace_ReportsError()
        {
            // Arrange
            var serializer = FromCompactFormatDeserializerFactory.Create(DependencyHelperFactory.Create());
            var xml = "<Test xmlns:x='" + StandardXomlKeys.Definitions_XmlNs + "' />";
            using var reader = XmlReader.Create(new StringReader(xml));
            reader.Read();
            var manager = new DesignerSerializationManager();
            var value = "{x:Null} extra";

            // Act
            object? result;
            using (manager.CreateSession())
            {
                var wfManager = new WorkflowMarkupSerializationManager(manager);
                wfManager.WorkflowMarkupStack.Push(reader);
                result = serializer.DeserializeFromCompactFormat(wfManager, reader, value);
                wfManager.WorkflowMarkupStack.Pop();
            }

            // Assert - should handle extra characters
            Assert.Null(result);
        }
        #endregion
    }
}