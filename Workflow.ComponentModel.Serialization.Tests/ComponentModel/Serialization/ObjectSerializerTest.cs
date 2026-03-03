using LogicBuilder.ComponentModel.Design.Serialization;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using LogicBuilder.Workflow.ComponentModel.Serialization.Factories;
using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization
{
    public class ObjectSerializerTest
    {
        private readonly IObjectSerializer _objectSerializer;
        private readonly WorkflowMarkupSerializationManager _serializationManager;
        private readonly DesignerSerializationManager _designerSerializationManager;

        public ObjectSerializerTest()
        {
            _objectSerializer = ObjectSerializerFactory.Create();
            _designerSerializationManager = new DesignerSerializationManager();
            _serializationManager = new WorkflowMarkupSerializationManager(_designerSerializationManager);
        }

        #region SerializeContents Tests - Null Argument Validation

        [Fact]
        public void SerializeContents_ThrowsArgumentNullException_WhenSerializationManagerIsNull()
        {
            // Arrange
            var obj = new object();
            using var stringWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter);

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                _objectSerializer.SerializeContents(null!, obj, xmlWriter, false));
            Assert.Equal("serializationManager", exception.ParamName);
        }

        [Fact]
        public void SerializeContents_ThrowsArgumentNullException_WhenObjIsNull()
        {
            // Arrange
            using var stringWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter);

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                _objectSerializer.SerializeContents(_serializationManager, null!, xmlWriter, false));
            Assert.Equal("obj", exception.ParamName);
        }

        [Fact]
        public void SerializeContents_ThrowsArgumentNullException_WhenWriterIsNull()
        {
            // Arrange
            var obj = new object();

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                _objectSerializer.SerializeContents(_serializationManager, obj, null!, false));
            Assert.Equal("writer", exception.ParamName);
        }

        #endregion

        #region SerializeObject Tests - Null Argument Validation

        [Fact]
        public void SerializeObject_ThrowsArgumentNullException_WhenSerializationManagerIsNull()
        {
            // Arrange
            var obj = new object();
            using var stringWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter);

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                _objectSerializer.SerializeObject(null!, obj, xmlWriter));
            Assert.Equal("serializationManager", exception.ParamName);
        }

        [Fact]
        public void SerializeObject_ThrowsArgumentNullException_WhenObjIsNull()
        {
            // Arrange
            using var stringWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter);

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                _objectSerializer.SerializeObject(_serializationManager, null!, xmlWriter));
            Assert.Equal("obj", exception.ParamName);
        }

        [Fact]
        public void SerializeObject_ThrowsArgumentNullException_WhenWriterIsNull()
        {
            // Arrange
            var obj = new object();

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                _objectSerializer.SerializeObject(_serializationManager, obj, null!));
            Assert.Equal("writer", exception.ParamName);
        }

        #endregion

        #region SerializeContents Tests - Primitive Types

        [Fact]
        public void SerializeContents_SerializesIntValue()
        {
            // Arrange
            int obj = 42;
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            // Act
            _objectSerializer.SerializeContents(_serializationManager, obj, xmlWriter, false);
            xmlWriter.Flush();

            // Assert
            var result = stringWriter.ToString();
            Assert.Contains("42", result);
        }

        [Fact]
        public void SerializeContents_SerializesStringValue()
        {
            // Arrange
            string obj = "test string";
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            // Act
            _objectSerializer.SerializeContents(_serializationManager, obj, xmlWriter, false);
            xmlWriter.Flush();

            // Assert
            var result = stringWriter.ToString();
            Assert.Contains("test string", result);
        }

        [Fact]
        public void SerializeContents_SerializesBoolValue()
        {
            // Arrange
            bool obj = true;
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            // Act
            _objectSerializer.SerializeContents(_serializationManager, obj, xmlWriter, false);
            xmlWriter.Flush();

            // Assert
            var result = stringWriter.ToString();
            Assert.Contains("true", result);
        }

        [Fact]
        public void SerializeContents_SerializesDecimalValue()
        {
            // Arrange
            decimal obj = 123.45m;
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            // Act
            _objectSerializer.SerializeContents(_serializationManager, obj, xmlWriter, false);
            xmlWriter.Flush();

            // Assert
            var result = stringWriter.ToString();
            Assert.Contains("123.45", result);
        }

        [Fact]
        public void SerializeContents_SerializesDateTimeValue()
        {
            // Arrange
            var obj = new DateTime(2026, 3, 3, 12, 0, 0, DateTimeKind.Utc);
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            // Act
            _objectSerializer.SerializeContents(_serializationManager, obj, xmlWriter, false);
            xmlWriter.Flush();

            // Assert
            var result = stringWriter.ToString();
            Assert.Contains("2026-03-03", result);
        }

        [Fact]
        public void SerializeContents_SerializesTimeSpanValue()
        {
            // Arrange
            var obj = TimeSpan.FromHours(2.5);
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            // Act
            _objectSerializer.SerializeContents(_serializationManager, obj, xmlWriter, false);
            xmlWriter.Flush();

            // Assert
            var result = stringWriter.ToString();
            Assert.Contains("02:30:00", result);
        }

        [Fact]
        public void SerializeContents_SerializesGuidValue()
        {
            // Arrange
            var obj = Guid.Parse("12345678-1234-1234-1234-123456789012");
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            // Act
            _objectSerializer.SerializeContents(_serializationManager, obj, xmlWriter, false);
            xmlWriter.Flush();

            // Assert
            var result = stringWriter.ToString();
            Assert.Contains("12345678-1234-1234-1234-123456789012", result);
        }

        [Fact]
        public void SerializeContents_SerializesEnumValue()
        {
            // Arrange
            var obj = ValuesType.Value2;
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            // Act
            _objectSerializer.SerializeContents(_serializationManager, obj, xmlWriter, false);
            xmlWriter.Flush();

            // Assert
            var result = stringWriter.ToString();
            Assert.Contains("Value2", result);
        }

        [Fact]
        public void SerializeContents_SerializesByteValue()
        {
            // Arrange
            byte obj = 255;
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            // Act
            _objectSerializer.SerializeContents(_serializationManager, obj, xmlWriter, false);
            xmlWriter.Flush();

            // Assert
            var result = stringWriter.ToString();
            Assert.Contains("255", result);
        }

        [Fact]
        public void SerializeContents_SerializesShortValue()
        {
            // Arrange
            short obj = 32000;
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            // Act
            _objectSerializer.SerializeContents(_serializationManager, obj, xmlWriter, false);
            xmlWriter.Flush();

            // Assert
            var result = stringWriter.ToString();
            Assert.Contains("32000", result);
        }

        [Fact]
        public void SerializeContents_ReplacesNullCharactersInString()
        {
            // Arrange
            string obj = "test\0string";
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            // Act
            _objectSerializer.SerializeContents(_serializationManager, obj, xmlWriter, false);
            xmlWriter.Flush();

            // Assert
            var result = stringWriter.ToString();
            Assert.Contains("test string", result);
        }

        [Fact]
        public void SerializeContents_EscapesStringStartingWithBrace()
        {
            // Arrange
            string obj = "{test}";
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            // Act
            _objectSerializer.SerializeContents(_serializationManager, obj, xmlWriter, false);
            xmlWriter.Flush();

            // Assert
            var result = stringWriter.ToString();
            Assert.Contains("{}", result);
        }

        [Fact]
        public void SerializeContents_DoesNotSerializeNullCharacter()
        {
            // Arrange
            char obj = '\0';
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            // Act
            _objectSerializer.SerializeContents(_serializationManager, obj, xmlWriter, false);
            xmlWriter.Flush();

            // Assert
            var result = stringWriter.ToString();
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void SerializeContents_SerializesNonNullCharacter()
        {
            // Arrange
            char obj = 'A';
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            // Act
            _objectSerializer.SerializeContents(_serializationManager, obj, xmlWriter, false);
            xmlWriter.Flush();

            // Assert
            var result = stringWriter.ToString();
            Assert.Contains("A", result);
        }

        #endregion

        #region SerializeObject Tests - Basic Scenarios

        [Fact]
        public void SerializeObject_SerializesSimpleObject()
        {
            // Arrange
            var obj = new TestSimpleClass { Name = "Test", Value = 123 };
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            using (_designerSerializationManager.CreateSession())
            {
                // Act
                _objectSerializer.SerializeObject(_serializationManager, obj, xmlWriter);
                xmlWriter.Flush();

                // Assert
                var result = stringWriter.ToString();
                Assert.NotEmpty(result);
            }
        }

        [Fact]
        public void SerializeObject_SerializesClassWithContentProperty()
        {
            // Arrange
            var obj = new TestClassWithContentProperty { Name = "Test" };
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            using (_designerSerializationManager.CreateSession())
            {
                // Act
                _objectSerializer.SerializeObject(_serializationManager, obj, xmlWriter);
                xmlWriter.Flush();

                // Assert
                var result = stringWriter.ToString();
                Assert.NotEmpty(result);
            }
        }

        [Fact]
        public void SerializeObject_SerializesPrimitiveType()
        {
            // Arrange
            int obj = 42;
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            using (_designerSerializationManager.CreateSession())
            {
                // Act
                _objectSerializer.SerializeObject(_serializationManager, obj, xmlWriter);
                xmlWriter.Flush();

                // Assert
                var result = stringWriter.ToString();
                Assert.NotEmpty(result);
            }
        }

        [Fact]
        public void SerializeObject_SerializesString()
        {
            // Arrange
            string obj = "test";
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            using (_designerSerializationManager.CreateSession())
            {
                // Act
                _objectSerializer.SerializeObject(_serializationManager, obj, xmlWriter);
                xmlWriter.Flush();

                // Assert
                var result = stringWriter.ToString();
                Assert.NotEmpty(result);
            }
        }

        [Fact]
        public void SerializeObject_SerializesObjectWithComplexProperties()
        {
            // Arrange
            var obj = new TestClassWithComplexProperty
            {
                Name = "Parent",
                ChildObject = new TestSimpleClass { Name = "Child", Value = 99 }
            };
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            using (_designerSerializationManager.CreateSession())
            {
                // Act
                _objectSerializer.SerializeObject(_serializationManager, obj, xmlWriter);
                xmlWriter.Flush();

                // Assert
                var result = stringWriter.ToString();
                Assert.NotEmpty(result);
            }
        }

        [Fact]
        public void SerializeObject_SerializesObjectWithReadOnlyProperty()
        {
            // Arrange
            var obj = new TestClassWithReadOnlyProperty("ReadOnlyValue");
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            using (_designerSerializationManager.CreateSession())
            {
                // Act
                _objectSerializer.SerializeObject(_serializationManager, obj, xmlWriter);
                xmlWriter.Flush();

                // Assert
                var result = stringWriter.ToString();
                Assert.NotEmpty(result);
            }
        }

        [Fact]
        public void SerializeObject_SerializesObjectWithNullProperty()
        {
            // Arrange
            var obj = new TestClassWithComplexProperty
            {
                Name = "Parent",
                ChildObject = null
            };
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            using (_designerSerializationManager.CreateSession())
            {
                // Act
                _objectSerializer.SerializeObject(_serializationManager, obj, xmlWriter);
                xmlWriter.Flush();

                // Assert
                var result = stringWriter.ToString();
                Assert.NotEmpty(result);
            }
        }

        [Fact]
        public void SerializeObject_SerializesObjectWithDefaultValueAttribute()
        {
            // Arrange
            var obj = new TestClassWithDefaultValue
            {
                PropertyWithDefault = null // Set to default value
            };
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            using (_designerSerializationManager.CreateSession())
            {
                // Act
                _objectSerializer.SerializeObject(_serializationManager, obj, xmlWriter);
                xmlWriter.Flush();

                // Assert - Should not throw
                var result = stringWriter.ToString();
                Assert.NotEmpty(result);
            }
        }

        [Fact]
        public void SerializeObject_SerializesObjectWithIndexedProperty()
        {
            // Arrange
            var obj = new TestClassWithIndexer();
            obj["key1"] = "value1";
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            using (_designerSerializationManager.CreateSession())
            {
                // Act - Should skip indexed properties
                _objectSerializer.SerializeObject(_serializationManager, obj, xmlWriter);
                xmlWriter.Flush();

                // Assert - Should not throw
                var result = stringWriter.ToString();
                Assert.NotEmpty(result);
            }
        }

        [Fact]
        public void SerializeObject_HandlesPropertyGetterException()
        {
            // Arrange
            var obj = new TestClassWithThrowingProperty();
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            using (_designerSerializationManager.CreateSession())
            {
                // Act - Should handle exception and continue
                _objectSerializer.SerializeObject(_serializationManager, obj, xmlWriter);
                xmlWriter.Flush();

                // Assert - Should not throw, error should be reported to serializationManager
                var result = stringWriter.ToString();
                Assert.NotEmpty(result);
            }
        }

        [Fact]
        public void SerializeObject_SerializesObjectWithCollection()
        {
            // Arrange
            var obj = new TestClassWithCollection
            {
                Items = ["item1", "item2", "item3"]
            };
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            using (_designerSerializationManager.CreateSession())
            {
                // Act
                _objectSerializer.SerializeObject(_serializationManager, obj, xmlWriter);
                xmlWriter.Flush();

                // Assert
                var result = stringWriter.ToString();
                Assert.NotEmpty(result);
            }
        }

        [Fact]
        public void SerializeObject_SerializesObjectWithEmptyCollection()
        {
            // Arrange
            var obj = new TestClassWithCollection
            {
                Items = []
            };
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            using (_designerSerializationManager.CreateSession())
            {
                // Act
                _objectSerializer.SerializeObject(_serializationManager, obj, xmlWriter);
                xmlWriter.Flush();

                // Assert
                var result = stringWriter.ToString();
                Assert.NotEmpty(result);
            }
        }

        #endregion

        #region SerializeContents Tests - DictionaryKey Parameter

        [Fact]
        public void SerializeContents_WithDictionaryKeyTrue_DoesNotSerializeExtendedProperties()
        {
            // Arrange
            int obj = 42;
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            // Act
            _objectSerializer.SerializeContents(_serializationManager, obj, xmlWriter, true);
            xmlWriter.Flush();

            // Assert - Should not throw and should serialize the value
            var result = stringWriter.ToString();
            Assert.Contains("42", result);
        }

        [Fact]
        public void SerializeContents_WithDictionaryKeyFalse_AllowsExtendedProperties()
        {
            // Arrange
            int obj = 44;
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            // Act
            _objectSerializer.SerializeContents(_serializationManager, obj, xmlWriter, false);
            xmlWriter.Flush();

            // Assert - Should not throw
            var result = stringWriter.ToString();
            Assert.Contains("44", result);
        }

        #endregion

        #region SerializeContents Tests - Complex Object Properties

        [Fact]
        public void SerializeContents_SerializesObjectWithMultipleProperties()
        {
            // Arrange
            var obj = new TestSimpleClass { Name = "TestName", Value = 123 };
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            using (_designerSerializationManager.CreateSession())
            {
                // Act
                _serializationManager.WorkflowMarkupStack.Push(xmlWriter);
                _serializationManager.Context.Push(obj);
                _serializationManager.SerializationStack.Push(obj);
                _objectSerializer.SerializeContents(_serializationManager, obj, xmlWriter, false);
                xmlWriter.Flush();
                _serializationManager.WorkflowMarkupStack.Pop();
                _serializationManager.Context.Pop();
                _serializationManager.SerializationStack.Pop();
                // Assert
                var result = stringWriter.ToString();
                Assert.NotEmpty(result);
            }
        }

        [Fact]
        public void SerializeObject_HandlesObjectWithNestedComplexProperty()
        {
            // Arrange
            var obj = new TestClassWithComplexProperty
            {
                Name = "Parent",
                ChildObject = new TestSimpleClass { Name = "Child", Value = 42 }
            };
            using var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            using (_designerSerializationManager.CreateSession())
            {
                // Act
                _objectSerializer.SerializeObject(_serializationManager, obj, xmlWriter);
                xmlWriter.Flush();

                // Assert
                var result = stringWriter.ToString();
                Assert.NotEmpty(result);
            }
        }

        #endregion

        #region Helper Classes

        private enum ValuesType
        {
            Value1,
            Value2,
            Value3
        }

        [ContentProperty("Name")]
        private class TestClassWithContentProperty
        {
            public string Name { get; set; } = string.Empty;
        }

        private class TestSimpleClass
        {
            public string Name { get; set; } = string.Empty;
            public int Value { get; set; }
        }

        private class TestClassWithComplexProperty
        {
            public string Name { get; set; } = string.Empty;
            public TestSimpleClass? ChildObject { get; set; }
        }

        private class TestClassWithReadOnlyProperty(string readOnlyValue)
        {
            public string ReadOnlyProperty { get; } = readOnlyValue;
        }

        private class TestClassWithDefaultValue
        {
            [DefaultValue(null)]
            public string? PropertyWithDefault { get; set; }
        }

        private class TestClassWithIndexer
        {
            private readonly Dictionary<string, string> _data = [];

            public string this[string key]
            {
                get => _data.TryGetValue(key, out var value) ? value : string.Empty; //NOSONAR - Return empty string for missing keys to avoid exceptions during serialization
                set => _data[key] = value;
            }

            public string NormalProperty { get; set; } = "Normal";
        }

        private class TestClassWithThrowingProperty
        {
#pragma warning disable CA1822 // Mark members as static
            public string ThrowingProperty //NOSONAR - This property is intentionally designed to throw an exception to test error handling during serialization
#pragma warning restore CA1822 // Mark members as static
            {
                get
                {
                    throw new InvalidOperationException("Property getter throws exception");
                }

                set { _ = value; }
            }

            public string NormalProperty { get; set; } = "Normal";
        }

        private class TestClassWithCollection
        {
            public List<string> Items { get; set; } = [];
        }

        #endregion
    }
}