using LogicBuilder.ComponentModel.Design.Serialization;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using LogicBuilder.Workflow.ComponentModel.Serialization.Factories;
using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;
using System;
using System.IO;
using System.Xml;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization
{
    public class DeserializeFromStringHelperTest
    {
        private readonly IDeserializeFromStringHelper _helper;
        private readonly WorkflowMarkupSerializationManager _serializationManager;
        private readonly DesignerSerializationManager _designerSerializationManager;

        public DeserializeFromStringHelperTest()
        {
            _helper = DeserializeFromStringHelperFactory.Create(DependencyHelperFactory.Create());
            _designerSerializationManager = new DesignerSerializationManager();
            _serializationManager = new WorkflowMarkupSerializationManager(_designerSerializationManager);
        }

        #region DeserializeFromString Tests - Primitives

        [Fact]
        public void DeserializeFromString_WithStringType_ReturnsString()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>TestValue</Property>");
            var value = "TestValue";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(string), value);

                // Assert
                Assert.Equal(value, result);
            }
        }

        [Fact]
        public void DeserializeFromString_WithIntType_ReturnsInt()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>42</Property>");
            var value = "42";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(int), value);

                // Assert
                Assert.Equal(42, result);
            }
        }

        [Fact]
        public void DeserializeFromString_WithBoolType_ReturnsBool()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>true</Property>");
            var value = "true";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(bool), value);

                // Assert
                Assert.True((bool)result);
            }
        }

        [Fact]
        public void DeserializeFromString_WithDoubleType_ReturnsDouble()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>3.14</Property>");
            var value = "3.14";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(double), value);

                // Assert
                Assert.Equal(3.14, result);
            }
        }

        [Fact]
        public void DeserializeFromString_WithLongType_ReturnsLong()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>9223372036854775807</Property>");
            var value = "9223372036854775807";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(long), value);

                // Assert
                Assert.Equal(9223372036854775807L, result);
            }
        }

        [Fact]
        public void DeserializeFromString_WithFloatType_ReturnsFloat()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>2.5</Property>");
            var value = "2.5";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(float), value);

                // Assert
                Assert.Equal(2.5f, result);
            }
        }

        #endregion

        #region DeserializeFromString Tests - Enums

        [Fact]
        public void DeserializeFromString_WithEnumType_ReturnsEnumValue()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>Value2</Property>");
            var value = "Value2";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(ValueType), value);

                // Assert
                Assert.Equal(ValueType.Value2, result);
            }
        }

        [Fact]
        public void DeserializeFromString_WithEnumTypeCaseInsensitive_ReturnsEnumValue()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>value1</Property>");
            var value = "value1";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(ValueType), value);

                // Assert
                Assert.Equal(ValueType.Value1, result);
            }
        }

        #endregion

        #region DeserializeFromString Tests - Special Types

        [Fact]
        public void DeserializeFromString_WithTimeSpanType_ReturnsTimeSpan()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>00:05:30</Property>");
            var value = "00:05:30";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(TimeSpan), value);

                // Assert
                Assert.Equal(TimeSpan.FromMinutes(5).Add(TimeSpan.FromSeconds(30)), result);
            }
        }

        [Fact]
        public void DeserializeFromString_WithDateTimeType_ReturnsDateTime()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>2026-02-27T10:30:00</Property>");
            var value = "2026-02-27T10:30:00";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(DateTime), value);

                // Assert
                Assert.Equal(new DateTime(2026, 2, 27, 10, 30, 0, DateTimeKind.Unspecified), result);
            }
        }

        [Fact]
        public void DeserializeFromString_WithGuidType_ReturnsGuid()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>12345678-1234-1234-1234-123456789012</Property>");
            var value = "12345678-1234-1234-1234-123456789012";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(Guid), value);

                // Assert
                Assert.Equal(new Guid("12345678-1234-1234-1234-123456789012"), result);
            }
        }

        [Fact]
        public void DeserializeFromString_WithDelegateType_ReturnsMethodName()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>MethodName</Property>");
            var value = "MethodName";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(Action), value);

                // Assert
                Assert.Equal(value, result);
            }
        }

        #endregion

        #region DeserializeFromString Tests - Nullable Types

        [Fact]
        public void DeserializeFromString_WithNullableInt_ReturnsInt()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>42</Property>");
            var value = "42";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(int?), value);

                // Assert
                Assert.Equal(42, result);
            }
        }

        [Fact]
        public void DeserializeFromString_WithNullableBool_ReturnsBool()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>false</Property>");
            var value = "false";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(bool?), value);

                // Assert
                Assert.False((bool)result);
            }
        }

        [Fact]
        public void DeserializeFromString_WithNullableDouble_ReturnsDouble()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>99.99</Property>");
            var value = "99.99";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(double?), value);

                // Assert
                Assert.Equal(99.99, result);
            }
        }

        [Fact]
        public void DeserializeFromString_WithNullableEnum_ReturnsEnumValue()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>Value3</Property>");
            var value = "Value3";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(ValueType?), value);

                // Assert
                Assert.Equal(ValueType.Value3, result);
            }
        }

        #endregion

        #region DeserializeFromString Tests - Type Handling

        [Fact]
        public void DeserializeFromString_WithTypeOfPrimitive_ReturnsType()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>System.Int32</Property>");
            var value = typeof(int).AssemblyQualifiedName;

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(Type), value);

                // Assert
                Assert.Equal(typeof(int), result);
            }
        }

        [Fact]
        public void DeserializeFromString_WithTypeOfString_ReturnsType()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>System.String</Property>");
            var value = typeof(string).AssemblyQualifiedName;

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(Type), value);

                // Assert
                Assert.Equal(typeof(string), result);
            }
        }

        [Fact]
        public void DeserializeFromString_WithTypeOfEnum_ReturnsType()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>LogicBuilder.Workflow.Tests.ComponentModel.Serialization.DeserializeFromStringHelperTest+TestEnum</Property>");
            var value = typeof(ValueType).AssemblyQualifiedName;

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);
                
                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(Type), value);

                // Assert
                Assert.Equal(typeof(ValueType), result);
            }
        }

        [Fact]
        public void DeserializeFromString_WithTypeOfUnknownType_ReturnsString()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>Unknown.Type.Name</Property>");
            var value = "Unknown.Type.Name";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(Type), value);

                // Assert
                Assert.Equal(value, result);
            }
        }

        #endregion

        #region DeserializeFromString Tests - IConvertible

        [Fact]
        public void DeserializeFromString_WithDecimalType_ReturnsDecimal()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>123.45</Property>");
            var value = "123.45";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(decimal), value);

                // Assert
                Assert.Equal(123.45m, result);
            }
        }

        [Fact]
        public void DeserializeFromString_WithByteType_ReturnsByte()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>255</Property>");
            var value = "255";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(byte), value);

                // Assert
                Assert.Equal((byte)255, result);
            }
        }

        [Fact]
        public void DeserializeFromString_WithCharType_ReturnsChar()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>A</Property>");
            var value = "A";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(char), value);

                // Assert
                Assert.Equal('A', result);
            }
        }

        #endregion

        #region DeserializeFromString Tests - Escape Sequences

        [Fact]
        public void DeserializeFromString_WithEscapeSequence_RemovesEscapePrefix()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>{}</Property>");
            var value = "{}TestValue";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(string), value);

                // Assert
                Assert.Equal("TestValue", result);
            }
        }

        [Fact]
        public void DeserializeFromString_WithEscapeSequenceAndInt_ParsesIntCorrectly()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>42</Property>");
            var value = "{}42";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(int), value);

                // Assert
                Assert.Equal(42, result);
            }
        }

        #endregion

        #region DeserializeFromString Tests - Compact Format

        [Fact]
        public void DeserializeFromString_WithCompactFormat_DeserializesUsingCompactFormatDeserializer()
        {
            // Arrange
            var xml = "<Test xmlns:x='" + StandardXomlKeys.Definitions_XmlNs + "' />";
            var xmlReader = CreateXmlReader(xml);
            var value = "{x:Type System.String}";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(Type), value);

                // Assert
                Assert.Equal(typeof(TypeExtension), result.GetType());
            }
        }

        #endregion

        #region DeserializeFromString Tests - Validation

        [Fact]
        public void DeserializeFromString_WithNullSerializationManager_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _helper.DeserializeFromString(null!, typeof(string), "test"));
        }

        [Fact]
        public void DeserializeFromString_WithNullPropertyType_ThrowsArgumentNullException()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>Test</Property>");

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act & Assert
                Assert.Throws<ArgumentNullException>(() =>
                    _helper.DeserializeFromString(_serializationManager, null!, "test"));
            }
        }

        [Fact]
        public void DeserializeFromString_WithNullValue_ThrowsArgumentNullException()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>Test</Property>");

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act & Assert
                Assert.Throws<ArgumentNullException>(() =>
                    _helper.DeserializeFromString(_serializationManager, typeof(string), null!));
            }
        }

        [Fact]
        public void DeserializeFromString_WithNoXmlReader_ReturnsNull()
        {
            // Arrange
            System.Diagnostics.Trace.Listeners.Clear();
            var value = "TestValue";

            using (_designerSerializationManager.CreateSession())
            {
                // Don't push XmlReader to stack

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(string), value);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public void DeserializeFromString_WithEmptyString_ReturnsEmptyString()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property></Property>");
            var value = string.Empty;

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(string), value);

                // Assert
                Assert.Equal(string.Empty, result);
            }
        }

        #endregion

        #region DeserializeFromString Tests - Edge Cases

        [Fact]
        public void DeserializeFromString_WithNegativeInt_ReturnsNegativeInt()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>-42</Property>");
            var value = "-42";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(int), value);

                // Assert
                Assert.Equal(-42, result);
            }
        }

        [Fact]
        public void DeserializeFromString_WithZero_ReturnsZero()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>0</Property>");
            var value = "0";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(int), value);

                // Assert
                Assert.Equal(0, result);
            }
        }

        [Fact]
        public void DeserializeFromString_WithMaxInt_ReturnsMaxInt()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>2147483647</Property>");
            var value = "2147483647";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(int), value);

                // Assert
                Assert.Equal(int.MaxValue, result);
            }
        }

        [Fact]
        public void DeserializeFromString_WithMinInt_ReturnsMinInt()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>-2147483648</Property>");
            var value = "-2147483648";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(int), value);

                // Assert
                Assert.Equal(int.MinValue, result);
            }
        }

        [Fact]
        public void DeserializeFromString_WithWhitespace_PreservesWhitespace()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<Property>  spaces  </Property>");
            var value = "  spaces  ";

            using (_designerSerializationManager.CreateSession())
            {
                xmlReader.Read();
                _serializationManager.WorkflowMarkupStack.Push(xmlReader);

                // Act
                var result = _helper.DeserializeFromString(_serializationManager, typeof(string), value);

                // Assert
                Assert.Equal("  spaces  ", result);
            }
        }

        #endregion

        #region Helper Methods

        private static XmlReader CreateXmlReader(string xml)
        {
            var stringReader = new StringReader(xml);
            var xmlReader = XmlReader.Create(stringReader);
            return xmlReader;
        }

        #endregion

        #region Test Classes and Enums

        private enum ValueType
        {
            Value1,
            Value2,
            Value3
        }

        #endregion
    }
}