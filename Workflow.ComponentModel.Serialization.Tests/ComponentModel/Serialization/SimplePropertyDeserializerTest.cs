using LogicBuilder.ComponentModel.Design.Serialization;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization
{
    public class SimplePropertyDeserializerTest
    {
        private readonly SimplePropertyDeserializer _deserializer;
        private readonly MarkupExtensionHelper _markupExtensionHelper;
        private readonly SerializationErrorHelper _serializationErrorHelper;
        private readonly WorkflowMarkupSerializationManager _serializationManager;
        private readonly DesignerSerializationManager _designerSerializationManager;

        public SimplePropertyDeserializerTest()
        {
            _markupExtensionHelper = new MarkupExtensionHelper();
            _serializationErrorHelper = new SerializationErrorHelper();
            _deserializer = new SimplePropertyDeserializer(_markupExtensionHelper, _serializationErrorHelper);
            _designerSerializationManager = new DesignerSerializationManager();
            _serializationManager = new WorkflowMarkupSerializationManager(_designerSerializationManager);
        }

        #region DeserializeSimpleProperty Tests

        [Fact]
        public void DeserializeSimpleProperty_WithWritableProperty_SetsPropertyValue()
        {
            // Arrange
            var testObject = new TestClass();
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StringProperty));
            var xmlReader = CreateXmlReader("<Property>TestValue</Property>");

            
            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                _serializationManager.Context.Push(property!);

                // Act
                _deserializer.DeserializeSimpleProperty(_serializationManager, xmlReader, testObject, "TestValue");

                // Assert
                Assert.Equal("TestValue", testObject.StringProperty);

                // Cleanup
                _serializationManager.Context.Pop();
            }
        }

        [Fact]
        public void DeserializeSimpleProperty_WithIntProperty_SetsIntValue()
        {
            // Arrange
            var testObject = new TestClass();
            var property = typeof(TestClass).GetProperty(nameof(TestClass.IntProperty));
            var xmlReader = CreateXmlReader("<Property>42</Property>");

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                _serializationManager.Context.Push(property!);

                // Act
                _deserializer.DeserializeSimpleProperty(_serializationManager, xmlReader, testObject, "42");

                // Assert
                Assert.Equal(42, testObject.IntProperty);

                // Cleanup
                _serializationManager.Context.Pop();
            }
        }

        [Fact]
        public void DeserializeSimpleProperty_WithBoolProperty_SetsBoolValue()
        {
            // Arrange
            var testObject = new TestClass();
            var property = typeof(TestClass).GetProperty(nameof(TestClass.BoolProperty));
            var xmlReader = CreateXmlReader("<Property>true</Property>");
            
            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                _serializationManager.Context.Push(property!);

                // Act
                _deserializer.DeserializeSimpleProperty(_serializationManager, xmlReader, testObject, "true");

                // Assert
                Assert.True(testObject.BoolProperty);

                // Cleanup
                _serializationManager.Context.Pop();
            }
        }

        [Fact]
        public void DeserializeSimpleProperty_WithReadOnlyNonCollectionProperty_ReportsError()
        {
            // Arrange
            var testObject = new TestClass();
            var property = typeof(TestClass).GetProperty(nameof(TestClass.ReadOnlyStringProperty));
            var xmlReader = CreateXmlReader("<Property>TestValue</Property>");

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                _serializationManager.Context.Push(property!);

                // Act
                _deserializer.DeserializeSimpleProperty(_serializationManager, xmlReader, testObject, "TestValue");

                // Assert
                Assert.True(_designerSerializationManager.Errors.Count > 0);

                // Cleanup
                _serializationManager.Context.Pop();
            }
        }

        [Fact]
        public void DeserializeSimpleProperty_WithReadOnlyCollectionProperty_AddsToCollection()
        {
            // Arrange
            var testObject = new TestClass();
            var property = typeof(TestClass).GetProperty(nameof(TestClass.ReadOnlyCollection));
            var xmlReader = CreateXmlReader("<Property>Item1,Item2,Item3</Property>");

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                _serializationManager.Context.Push(property!);

                // Act
                _deserializer.DeserializeSimpleProperty(_serializationManager, xmlReader, testObject, "Item1,Item2,Item3");

                // Assert
                Assert.Contains("Item1", testObject.ReadOnlyCollection);
                Assert.Contains("Item2", testObject.ReadOnlyCollection);
                Assert.Contains("Item3", testObject.ReadOnlyCollection);

                // Cleanup
                _serializationManager.Context.Pop();
            }
        }

        [Fact]
        public void DeserializeSimpleProperty_WithNullPropertyInContext_ReturnsWithoutError()
        {
            // Arrange
            System.Diagnostics.Trace.Listeners.Clear();
            var testObject = new TestClass();
            var xmlReader = CreateXmlReader("<Property>TestValue</Property>");

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Don't push anything to context, so Current will be null

                // Act - should return early without throwing
                _deserializer.DeserializeSimpleProperty(_serializationManager, xmlReader, testObject, "TestValue");

                // Assert - no exception thrown
                Assert.True(true);
            }
        }

        [Fact]
        public void DeserializeSimpleProperty_WithEmptyValue_SetsEmptyString()
        {
            // Arrange
            var testObject = new TestClass();
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StringProperty));
            var xmlReader = CreateXmlReader("<Property></Property>");

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                _serializationManager.Context.Push(property!);

                // Act
                _deserializer.DeserializeSimpleProperty(_serializationManager, xmlReader, testObject, string.Empty);

                // Assert
                Assert.Equal(string.Empty, testObject.StringProperty);

                // Cleanup
                _serializationManager.Context.Pop();
            }
        }

        [Fact]
        public void DeserializeSimpleProperty_WithPropertyThatThrowsOnSet_ReportsError()
        {
            // Arrange
            var testObject = new TestClassWithExceptionProperty();
            var property = typeof(TestClassWithExceptionProperty).GetProperty(nameof(TestClassWithExceptionProperty.ThrowingProperty));
            var xmlReader = CreateXmlReader("<Property>TestValue</Property>");

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                _serializationManager.Context.Push(property!);

                // Act
                _deserializer.DeserializeSimpleProperty(_serializationManager, xmlReader, testObject, "TestValue");

                // Assert
                Assert.True(_designerSerializationManager.Errors.Count > 0);

                // Cleanup
                _serializationManager.Context.Pop();
            }
        }

        [Fact]
        public void DeserializeSimpleProperty_WithInvalidIntValue_ReportsError()
        {
            // Arrange
            var testObject = new TestClass();
            var property = typeof(TestClass).GetProperty(nameof(TestClass.IntProperty));
            var xmlReader = CreateXmlReader("<Property>NotANumber</Property>");

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                _serializationManager.Context.Push(property!);

                // Act
                _deserializer.DeserializeSimpleProperty(_serializationManager, xmlReader, testObject, "NotANumber");

                // Assert
                Assert.True(_designerSerializationManager.Errors.Count > 0);

                // Cleanup
                _serializationManager.Context.Pop();
            }
        }

        [Fact]
        public void DeserializeSimpleProperty_WithDoubleProperty_SetsDoubleValue()
        {
            // Arrange
            var testObject = new TestClass();
            var property = typeof(TestClass).GetProperty(nameof(TestClass.DoubleProperty));
            var xmlReader = CreateXmlReader("<Property>3.14</Property>");

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                _serializationManager.Context.Push(property!);

                // Act
                _deserializer.DeserializeSimpleProperty(_serializationManager, xmlReader, testObject, "3.14");

                // Assert
                Assert.Equal(3.14, testObject.DoubleProperty);

                // Cleanup
                _serializationManager.Context.Pop();
            }
        }

        [Fact]
        public void DeserializeSimpleProperty_WithEnumProperty_SetsEnumValue()
        {
            // Arrange
            var testObject = new TestClass();
            var property = typeof(TestClass).GetProperty(nameof(TestClass.EnumProperty));
            var xmlReader = CreateXmlReader("<Property>Value2</Property>");

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                _serializationManager.Context.Push(property!);

                // Act
                _deserializer.DeserializeSimpleProperty(_serializationManager, xmlReader, testObject, "Value2");

                // Assert
                Assert.Equal(ValueType.Value2, testObject.EnumProperty);

                // Cleanup
                _serializationManager.Context.Pop();
            }
        }

        #endregion

        #region Helper Methods

        private static XmlReader CreateXmlReader(string xml)
        {
            var stringReader = new StringReader(xml);
            var xmlReader = XmlReader.Create(stringReader);
            xmlReader.Read(); // Move to first element
            return xmlReader;
        }

        #endregion

        #region Test Classes

        private class TestClass
        {
            public string StringProperty { get; set; } = "";
            public int IntProperty { get; set; } = 0;// NOSONAR
            public bool BoolProperty { get; set; } = false;// NOSONAR
            public double DoubleProperty { get; set; } = 0.0;
            public ValueType EnumProperty { get; set; } = ValueType.Value1;// NOSONAR

            public string ReadOnlyStringProperty { get; } = "ReadOnly";

            private readonly List<string> _readOnlyCollection = [];
            public ICollection<string> ReadOnlyCollection => _readOnlyCollection;
        }

        private class TestClassWithExceptionProperty
        {
            private readonly string _throwingProperty = "";
            
            public string ThrowingProperty
            {
                get => _throwingProperty;
                set => throw new InvalidOperationException("Property setter always throws");
            }
        }

        private enum ValueType
        {
            Value1,
            Value2,
            Value3
        }

        #endregion
    }
}