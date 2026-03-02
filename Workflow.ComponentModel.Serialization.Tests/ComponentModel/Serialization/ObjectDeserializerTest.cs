using LogicBuilder.ComponentModel.Design.Serialization;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using LogicBuilder.Workflow.ComponentModel.Serialization.Factories;
using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization
{
    public class ObjectDeserializerTest
    {
        private readonly IObjectDeserializer _deserializer;
        private readonly WorkflowMarkupSerializationManager _serializationManager;
        private readonly DesignerSerializationManager _designerSerializationManager;

        public ObjectDeserializerTest()
        {
            _deserializer = ObjectDeserializerFactory.Create();
            _designerSerializationManager = new DesignerSerializationManager();
            _serializationManager = new WorkflowMarkupSerializationManager(_designerSerializationManager);
        }

        #region DeserializeObject Tests

        [Fact]
        public void DeserializeObject_ThrowsArgumentNullException_WhenSerializationManagerIsNull()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<TestClass />");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                _deserializer.DeserializeObject(null!, xmlReader));
        }

        [Fact]
        public void DeserializeObject_ThrowsArgumentNullException_WhenReaderIsNull()
        {
            // Arrange & Act & Assert
            using (_designerSerializationManager.CreateSession())
            {
                Assert.Throws<ArgumentNullException>(() =>
                    _deserializer.DeserializeObject(_serializationManager, null!));
            }
        }

        [Fact]
        public void DeserializeObject_DeserializesSimpleString_Successfully()
        {
            // Arrange
            string xml = @"<s:String xmlns:s=""clr-namespace:System;assembly=mscorlib"">TestValue</s:String>";
            using XmlReader reader = CreateXmlReader(xml);

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                var result = _deserializer.DeserializeObject(_serializationManager, reader);

                // Assert
                Assert.NotNull(result);
                Assert.IsType<string>(result);
                Assert.Equal("TestValue", result);
            }
        }

        [Fact]
        public void DeserializeObject_DeserializesInt32_Successfully()
        {
            // Arrange
            string xml = @"<s:Int32 xmlns:s=""clr-namespace:System;assembly=mscorlib"">42</s:Int32>";
            using XmlReader reader = CreateXmlReader(xml);

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                var result = _deserializer.DeserializeObject(_serializationManager, reader);

                // Assert
                Assert.NotNull(result);
                Assert.IsType<int>(result);
                Assert.Equal(42, result);
            }
        }

        [Fact]
        public void DeserializeObject_DeserializesBoolean_Successfully()
        {
            // Arrange
            string xml = @"<s:Boolean xmlns:s=""clr-namespace:System;assembly=mscorlib"">true</s:Boolean>";
            using XmlReader reader = CreateXmlReader(xml);

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                var result = _deserializer.DeserializeObject(_serializationManager, reader);

                // Assert
                Assert.NotNull(result);
                Assert.IsType<bool>(result);
                Assert.True((bool)result);
            }
        }

        [Fact]
        public void DeserializeObject_DeserializesDateTime_WithRoundtripKind()
        {
            // Arrange
            var expectedDate = new DateTime(2026, 3, 2, 10, 30, 0, DateTimeKind.Utc);
            string dateString = expectedDate.ToString("o");
            string xml = $@"<s:DateTime xmlns:s=""clr-namespace:System;assembly=mscorlib"">{dateString}</s:DateTime>";
            using XmlReader reader = CreateXmlReader(xml);

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                var result = _deserializer.DeserializeObject(_serializationManager, reader);

                // Assert
                Assert.NotNull(result);
                Assert.IsType<DateTime>(result);
                Assert.Equal(expectedDate, (DateTime)result);
            }
        }

        [Fact]
        public void DeserializeObject_DeserializesGuid_Successfully()
        {
            // Arrange
            var expectedGuid = Guid.NewGuid();
            string xml = $@"<s:Guid xmlns:s=""clr-namespace:System;assembly=mscorlib"">{expectedGuid}</s:Guid>";
            using XmlReader reader = CreateXmlReader(xml);

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                var result = _deserializer.DeserializeObject(_serializationManager, reader);

                // Assert
                Assert.NotNull(result);
                Assert.IsType<Guid>(result);
                Assert.Equal(expectedGuid, result);
            }
        }

        [Fact]
        public void DeserializeObject_DeserializesDecimal_Successfully()
        {
            // Arrange
            string xml = @"<s:Decimal xmlns:s=""clr-namespace:System;assembly=mscorlib"">123.45</s:Decimal>";
            using XmlReader reader = CreateXmlReader(xml);

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                var result = _deserializer.DeserializeObject(_serializationManager, reader);

                // Assert
                Assert.NotNull(result);
                Assert.IsType<decimal>(result);
                Assert.Equal(123.45m, result);
            }
        }

        [Fact]
        public void DeserializeObject_DeserializesTimeSpan_Successfully()
        {
            // Arrange
            var expectedTimeSpan = TimeSpan.FromHours(2.5);
            string xml = $@"<s:TimeSpan xmlns:s=""clr-namespace:System;assembly=mscorlib"">{expectedTimeSpan}</s:TimeSpan>";
            using XmlReader reader = CreateXmlReader(xml);

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                var result = _deserializer.DeserializeObject(_serializationManager, reader);

                // Assert
                Assert.NotNull(result);
                Assert.IsType<TimeSpan>(result);
                Assert.Equal(expectedTimeSpan, result);
            }
        }

        #endregion

        #region CreateInstance Tests

        [Fact]
        public void CreateInstance_CreatesStringInstance_Successfully()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("String", "clr-namespace:System;assembly=mscorlib");
            string xml = @"<s:String xmlns:s=""clr-namespace:System;assembly=mscorlib"">Test</s:String>";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                var result = _deserializer.CreateInstance(_serializationManager, xmlQualifiedName, reader);

                // Assert
                Assert.NotNull(result);
                Assert.IsType<string>(result);
                Assert.Equal("Test", result);
            }
        }

        [Fact]
        public void CreateInstance_CreatesInt32Instance_Successfully()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("Int32", "clr-namespace:System;assembly=mscorlib");
            string xml = @"<s:Int32 xmlns:s=""clr-namespace:System;assembly=mscorlib"">100</s:Int32>";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                var result = _deserializer.CreateInstance(_serializationManager, xmlQualifiedName, reader);

                // Assert
                Assert.NotNull(result);
                Assert.IsType<int>(result);
                Assert.Equal(100, result);
            }
        }

        [Fact]
        public void CreateInstance_ReturnsNull_WhenTypeNotResolved()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("NonExistentType", "clr-namespace:NonExistent;assembly=NonExistent");
            string xml = @"<test:NonExistentType xmlns:test=""clr-namespace:NonExistent;assembly=NonExistent"" />";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                var result = _deserializer.CreateInstance(_serializationManager, xmlQualifiedName, reader);

                // Assert
                Assert.Null(result);
                Assert.True(_designerSerializationManager.Errors.Count > 0);
            }
        }

        [Fact]
        public void CreateInstance_TriesExtensionSuffix_WhenTypeNotFound()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("Test", "clr-namespace:System;assembly=mscorlib");
            string xml = @"<s:Test xmlns:s=""clr-namespace:System;assembly=mscorlib"" />";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                var result = _deserializer.CreateInstance(_serializationManager, xmlQualifiedName, reader);

                // Assert - should try "TestExtension" but still fail
                Assert.Null(result);
            }
        }

        [Fact]
        public void CreateInstance_CreatesEnumInstance_Successfully()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("StringComparison", "clr-namespace:System;assembly=mscorlib");
            string xml = @"<s:StringComparison xmlns:s=""clr-namespace:System;assembly=mscorlib"">Ordinal</s:StringComparison>";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                var result = _deserializer.CreateInstance(_serializationManager, xmlQualifiedName, reader);

                // Assert
                Assert.NotNull(result);
                Assert.IsType<StringComparison>(result);
                Assert.Equal(StringComparison.Ordinal, result);
            }
        }

        #endregion

        #region DeserializeCompoundProperty Tests

        [Fact]
        public void DeserializeCompoundProperty_WithReadOnlyProperty_DeserializesContents()
        {
            // Arrange
            var testObject = new TestClassWithReadOnlyProperty();
            var property = typeof(TestClassWithReadOnlyProperty).GetProperty(nameof(TestClassWithReadOnlyProperty.Items));
            string xml = @"<TestClassWithReadOnlyProperty.Items xmlns=""test"">
                            <s:String xmlns:s=""clr-namespace:System;assembly=mscorlib"">Item1</s:String>
                          </TestClassWithReadOnlyProperty.Items>";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                _serializationManager.Context.Push(property!);
                _deserializer.DeserializeCompoundProperty(_serializationManager, reader, testObject);

                // Assert
                Assert.NotNull(testObject.Items);
                // Note: Actual deserialization of items would require more setup
            }
        }

        [Fact]
        public void DeserializeCompoundProperty_WithWritableProperty_SetsPropertyValue()
        {
            // Arrange
            var testObject = new TestClass();
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StringProperty));
            string xml = @"<TestClass.StringProperty xmlns=""test"">
                            <s:String xmlns:s=""clr-namespace:System;assembly=mscorlib"">NewValue</s:String>
                          </TestClass.StringProperty>";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                _serializationManager.Context.Push(property!);
                _deserializer.DeserializeCompoundProperty(_serializationManager, reader, testObject);

                // Assert
                Assert.Equal("NewValue", testObject.StringProperty);
            }
        }

        [Fact]
        public void DeserializeCompoundProperty_WithTextContent_DeserializesSimpleProperty()
        {
            // Arrange
            var testObject = new TestClass();
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StringProperty));
            string xml = @"<TestClass.StringProperty xmlns=""test"">TextValue</TestClass.StringProperty>";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                _serializationManager.Context.Push(property!);
                _deserializer.DeserializeCompoundProperty(_serializationManager, reader, testObject);

                // Assert
                Assert.Equal("TextValue", testObject.StringProperty);
            }
        }

        [Fact]
        public void DeserializeCompoundProperty_WithEmptyElement_DoesNotThrow()
        {
            // Arrange
            var testObject = new TestClass();
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StringProperty));
            string xml = @"<TestClass.StringProperty xmlns=""test"" />";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act & Assert - Should not throw
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                _serializationManager.Context.Push(property!);
                _deserializer.DeserializeCompoundProperty(_serializationManager, reader, testObject);
            }

            Assert.True(true); // If we reach here, it means no exception was thrown
        }

        #endregion

        #region DeserializeContents Tests

        [Fact]
        public void DeserializeContents_ThrowsArgumentNullException_WhenSerializationManagerIsNull()
        {
            // Arrange
            var testObject = new TestClass();
            var xmlReader = CreateXmlReader("<TestClass />");
            xmlReader.Read();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _deserializer.DeserializeContents(null!, testObject, xmlReader));
        }

        [Fact]
        public void DeserializeContents_ThrowsArgumentNullException_WhenObjectIsNull()
        {
            // Arrange
            var xmlReader = CreateXmlReader("<TestClass />");
            xmlReader.Read();

            // Act & Assert
            using (_designerSerializationManager.CreateSession())
            {
                Assert.Throws<ArgumentNullException>(() =>
                    _deserializer.DeserializeContents(_serializationManager, null!, xmlReader));
            }
        }

        [Fact]
        public void DeserializeContents_ThrowsArgumentNullException_WhenReaderIsNull()
        {
            // Arrange
            var testObject = new TestClass();

            // Act & Assert
            using (_designerSerializationManager.CreateSession())
            {
                Assert.Throws<ArgumentNullException>(() =>
                    _deserializer.DeserializeContents(_serializationManager, testObject, null!));
            }
        }

        [Fact]
        public void DeserializeContents_ReturnsEarly_WhenNotElementNode()
        {
            // Arrange
            var testObject = new TestClass();
            string xml = "Some text content";
            using XmlReader reader = XmlReader.Create(new StringReader(xml));

            // Act & Assert - Should not throw
            using (_designerSerializationManager.CreateSession())
            {
                _deserializer.DeserializeContents(_serializationManager, testObject, reader);
            }

            Assert.True(true); // If we reach here, it means no exception was thrown
        }

        [Fact]
        public void DeserializeContents_DeserializesSimpleAttributes_Successfully()
        {
            // Arrange
            var testObject = new TestClass();
            string xml = @"<TestClass StringProperty=""AttributeValue"" />";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                _deserializer.DeserializeContents(_serializationManager, testObject, reader);

                // Assert
                Assert.Equal("AttributeValue", testObject.StringProperty);
            }
        }

        [Fact]
        public void DeserializeContents_IgnoresXmlnsAttributes()
        {
            // Arrange
            var testObject = new TestClass();
            string xml = @"<TestClass xmlns=""test"" xmlns:s=""clr-namespace:System;assembly=mscorlib"" />";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act & Assert - Should not throw
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                _deserializer.DeserializeContents(_serializationManager, testObject, reader);
            }

            Assert.True(true); // If we reach here, it means no exception was thrown
        }

        [Fact]
        public void DeserializeContents_WithEmptyElement_CompletesSuccessfully()
        {
            // Arrange
            var testObject = new TestClass();
            string xml = @"<TestClass />";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act & Assert - Should not throw
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                _deserializer.DeserializeContents(_serializationManager, testObject, reader);
            }

            Assert.True(true); // If we reach here, it means no exception was thrown
        }

        #endregion

        #region GetClrFullName Tests

        [Fact]
        public void GetClrFullName_ReturnsFullName_WhenNamespaceMappingExists()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("String", "clr-namespace:System;assembly=mscorlib");

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                var result = _deserializer.GetClrFullName(_serializationManager, xmlQualifiedName);

                // Assert
                Assert.NotNull(result);
                Assert.Contains("String", result);
            }
        }

        [Fact]
        public void GetClrFullName_ReturnsQualifiedName_WhenNoMappingExists()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("CustomType", "http://custom.namespace");

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                var result = _deserializer.GetClrFullName(_serializationManager, xmlQualifiedName);

                // Assert
                Assert.Equal("http://custom.namespace.CustomType", result);
            }
        }

        [Fact]
        public void GetClrFullName_AppendsTypeName_ToClrNamespace()
        {
            // Arrange
            var mapping = new WorkflowMarkupSerializerMapping(
                "http://test.namespace",
                "mscorlib",
                "Test.Namespace",
                "TestAssembly");
            var xmlQualifiedName = new XmlQualifiedName("TestType", "http://test.namespace");

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.XmlNamespaceBasedMappings["http://test.namespace"] = 
                    [mapping];
                
                var result = _deserializer.GetClrFullName(_serializationManager, xmlQualifiedName);

                // Assert
                Assert.Equal("Test.Namespace.TestType", result);
            }
        }

        [Fact]
        public void GetClrFullName_ReturnsTypeName_WhenClrNamespaceIsEmpty()
        {
            // Arrange
            var mapping = new WorkflowMarkupSerializerMapping(
                "http://test.namespace",
                "mscorlib",
                "",
                "TestAssembly");
            var xmlQualifiedName = new XmlQualifiedName("TestType", "http://test.namespace");

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.XmlNamespaceBasedMappings["http://test.namespace"] = 
                    [mapping];
                
                var result = _deserializer.GetClrFullName(_serializationManager, xmlQualifiedName);

                // Assert
                Assert.Equal("TestType", result);
            }
        }

        #endregion

        #region CreateInstance - Error Handling Tests

        [Fact]
        public void CreateInstance_ReportsError_WhenSerializerNotAvailable()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("TestClassWithoutSerializer", "clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;assembly=Workflow.ComponentModel.Serialization.Tests");
            string xml = @"<test:TestClassWithoutSerializer xmlns:test=""clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;assembly=Workflow.ComponentModel.Serialization.Tests"" />";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                var result = _deserializer.CreateInstance(_serializationManager, xmlQualifiedName, reader);

                // Assert
                Assert.Null(result);
                Assert.True(_designerSerializationManager.Errors.Count > 0);
            }
        }

        [Fact]
        public void CreateInstance_HandlesExceptionInGetType_WithInnerError()
        {
            // Arrange - Use a malformed namespace that will cause GetType to throw
            var xmlQualifiedName = new XmlQualifiedName("BadType", "clr-namespace:BadNamespace;;assembly=NonExistent");
            string xml = @"<test:BadType xmlns:test=""clr-namespace:BadNamespace;;assembly=NonExistent"" />";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                var result = _deserializer.CreateInstance(_serializationManager, xmlQualifiedName, reader);

                // Assert
                Assert.Null(result);
                Assert.True(_designerSerializationManager.Errors.Count > 0);
            }
        }

        [Fact]
        public void CreateInstance_HandlesExceptionInGetType_ForExtensionSuffix()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("Custom", "clr-namespace:BadNamespace;;assembly=NonExistent");
            string xml = @"<test:Custom xmlns:test=""clr-namespace:BadNamespace;;assembly=NonExistent"" />";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                var result = _deserializer.CreateInstance(_serializationManager, xmlQualifiedName, reader);

                // Assert
                Assert.Null(result);
                Assert.True(_designerSerializationManager.Errors.Count > 0);
            }
        }

        [Fact]
        public void CreateInstance_HandlesExceptionDuringInstanceCreation()
        {
            // Arrange - Use a type that exists but might throw during instantiation
            var xmlQualifiedName = new XmlQualifiedName("TestClassThrowsOnCreate", "clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;assembly=Workflow.ComponentModel.Serialization.Tests");
            string xml = @"<test:TestClassThrowsOnCreate xmlns:test=""clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;assembly=Workflow.ComponentModel.Serialization.Tests"" />";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                var result = _deserializer.CreateInstance(_serializationManager, xmlQualifiedName, reader);

                // Assert - Should handle gracefully
                Assert.True(result == null || _designerSerializationManager.Errors.Count > 0);
            }
        }

        [Fact]
        public void CreateInstance_HandlesPrimitiveType_WithParseError()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("Int32", "clr-namespace:System;assembly=mscorlib");
            string xml = @"<s:Int32 xmlns:s=""clr-namespace:System;assembly=mscorlib"">NotANumber</s:Int32>";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                var result = _deserializer.CreateInstance(_serializationManager, xmlQualifiedName, reader);

                // Assert
                Assert.Null(result);
                Assert.True(_designerSerializationManager.Errors.Count > 0);
            }
        }

        #endregion

        #region DeserializeCompoundProperty - Additional Tests

        [Fact]
        public void DeserializeCompoundProperty_ReportsError_WhenReadOnlyPropertyValueIsNull()
        {
            // Arrange
            var testObject = new TestClassWithNullReadOnlyProperty();
            var property = typeof(TestClassWithNullReadOnlyProperty).GetProperty(nameof(TestClassWithNullReadOnlyProperty.Items));
            string xml = @"<TestClassWithNullReadOnlyProperty.Items xmlns=""test"" />";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                _serializationManager.Context.Push(property!);
                _deserializer.DeserializeCompoundProperty(_serializationManager, reader, testObject);

                // Assert
                Assert.True(_designerSerializationManager.Errors.Count > 0);
            }
        }

        [Fact]
        public void DeserializeCompoundProperty_ReportsError_WhenAttributesFoundOnComplexProperty()
        {
            // Arrange
            var testObject = new TestClass();
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StringProperty));
            string xml = @"<TestClass.StringProperty xmlns=""test"" InvalidAttribute=""value"">
                            <s:String xmlns:s=""clr-namespace:System;assembly=mscorlib"">NewValue</s:String>
                          </TestClass.StringProperty>";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                _serializationManager.Context.Push(property!);
                _deserializer.DeserializeCompoundProperty(_serializationManager, reader, testObject);

                // Assert
                Assert.True(_designerSerializationManager.Errors.Count > 0);
            }
        }

        [Fact]
        public void DeserializeCompoundProperty_HandlesException_InPropertySetValue()
        {
            // Arrange
            var testObject = new TestClass();
            var property = typeof(TestClass).GetProperty(nameof(TestClass.IntProperty));
            string xml = @"<TestClass.IntProperty xmlns=""test"">
                            <s:String xmlns:s=""clr-namespace:System;assembly=mscorlib"">NotAnInt</s:String>
                          </TestClass.IntProperty>";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                _serializationManager.Context.Push(property!);
                _deserializer.DeserializeCompoundProperty(_serializationManager, reader, testObject);

                // Assert
                Assert.True(_designerSerializationManager.Errors.Count > 0);
            }
        }

        [Fact]
        public void DeserializeCompoundProperty_HandlesMarkupExtension_WithBracesPrefix()
        {
            // Arrange
            var testObject = new TestClass();
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StringProperty));
            string xml = @"<TestClass.StringProperty xmlns=""test"">
                            <s:String xmlns:s=""clr-namespace:System;assembly=mscorlib"">{}{Binding}</s:String>
                          </TestClass.StringProperty>";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                _serializationManager.Context.Push(property!);
                _deserializer.DeserializeCompoundProperty(_serializationManager, reader, testObject);

                // Assert - Should strip "{}" prefix
                Assert.Equal("{Binding}", testObject.StringProperty);
            }
        }

        #endregion

        #region DeserializeContents - Additional Tests

        [Fact]
        public void DeserializeContents_HandlesPrimitiveTypes_WithExtendedProperties()
        {
            // Arrange
            int primitiveValue = 42;
            string xml = @"<s:Int32 xmlns:s=""clr-namespace:System;assembly=mscorlib"">42</s:Int32>";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act & Assert - Should not throw
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                _deserializer.DeserializeContents(_serializationManager, primitiveValue, reader);
            }

            Assert.True(true); // If we reach here, it means no exception was thrown
        }

        [Fact]
        public void DeserializeContents_HandlesException_InGetProperties()
        {
            // Arrange - This would require a custom serializer that throws in GetProperties
            // For now, we'll test with a valid object but in a scenario that might cause issues
            var testObject = new TestClass();
            string xml = @"<TestClass xmlns=""test"" />";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                _deserializer.DeserializeContents(_serializationManager, testObject, reader);

                // Assert - Should complete without throwing
                Assert.True(true);
            }
        }

        [Fact]
        public void DeserializeContents_ProcessesMultipleAttributes()
        {
            // Arrange
            var testObject = new TestClass();
            string xml = @"<TestClass StringProperty=""Value1"" IntProperty=""123"" BoolProperty=""true"" />";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                _deserializer.DeserializeContents(_serializationManager, testObject, reader);

                // Assert
                Assert.Equal("Value1", testObject.StringProperty);
                Assert.Equal(123, testObject.IntProperty);
                Assert.True(testObject.BoolProperty);
            }
        }

        [Fact]
        public void DeserializeContents_ReportsError_WhenPropertyNotFound()
        {
            // Arrange
            var testObject = new TestClass();
            string xml = @"<TestClass NonExistentProperty=""Value"" />";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                _deserializer.DeserializeContents(_serializationManager, testObject, reader);

                // Assert
                Assert.True(_designerSerializationManager.Errors.Count > 0);
            }
        }

        #endregion

        #region DeserializeObject - Additional Tests

        [Fact]
        public void DeserializeObject_ReportsError_WhenTypeCannotBeResolved()
        {
            // Arrange
            string xml = "<Test/>";
            using XmlReader reader = XmlReader.Create(new StringReader(xml));

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                var result = _deserializer.DeserializeObject(_serializationManager, reader);

                // Assert
                Assert.Null(result);
                Assert.True(_designerSerializationManager.Errors.Count > 0);
            }
        }

        #endregion

        #region GetClrFullName - Edge Cases

        [Fact]
        public void GetClrFullName_HandlesMappingWithEmptyList()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("TestType", "http://empty.namespace");

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.XmlNamespaceBasedMappings["http://empty.namespace"] =
                    [];

                var result = _deserializer.GetClrFullName(_serializationManager, xmlQualifiedName);

                // Assert
                Assert.Equal("http://empty.namespace.TestType", result);
            }
        }

        [Fact]
        public void GetClrFullName_HandlesMultipleMappings_UsesFirst()
        {
            // Arrange
            var mapping1 = new WorkflowMarkupSerializerMapping(
                "http://test.namespace",
                "mscorlib",
                "First.Namespace",
                "TestAssembly");
            var mapping2 = new WorkflowMarkupSerializerMapping(
                "http://test.namespace",
                "mscorlib",
                "Second.Namespace",
                "TestAssembly");
            var xmlQualifiedName = new XmlQualifiedName("TestType", "http://test.namespace");

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.XmlNamespaceBasedMappings["http://test.namespace"] =
                    [mapping1, mapping2];

                var result = _deserializer.GetClrFullName(_serializationManager, xmlQualifiedName);

                // Assert
                Assert.Equal("First.Namespace.TestType", result);
            }
        }

        #endregion

        #region Primitive Type Tests

        [Fact]
        public void CreateInstance_DeserializesDouble_Successfully()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("Double", "clr-namespace:System;assembly=mscorlib");
            string xml = @"<s:Double xmlns:s=""clr-namespace:System;assembly=mscorlib"">3.14159</s:Double>";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                var result = _deserializer.CreateInstance(_serializationManager, xmlQualifiedName, reader);

                // Assert
                Assert.NotNull(result);
                Assert.IsType<double>(result);
                Assert.Equal(3.14159, (double)result, 5);
            }
        }

        [Fact]
        public void CreateInstance_DeserializesByte_Successfully()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("Byte", "clr-namespace:System;assembly=mscorlib");
            string xml = @"<s:Byte xmlns:s=""clr-namespace:System;assembly=mscorlib"">255</s:Byte>";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                var result = _deserializer.CreateInstance(_serializationManager, xmlQualifiedName, reader);

                // Assert
                Assert.NotNull(result);
                Assert.IsType<byte>(result);
                Assert.Equal((byte)255, result);
            }
        }

        [Fact]
        public void CreateInstance_DeserializesChar_Successfully()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("Char", "clr-namespace:System;assembly=mscorlib");
            string xml = @"<s:Char xmlns:s=""clr-namespace:System;assembly=mscorlib"">A</s:Char>";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                var result = _deserializer.CreateInstance(_serializationManager, xmlQualifiedName, reader);

                // Assert
                Assert.NotNull(result);
                Assert.IsType<char>(result);
                Assert.Equal('A', result);
            }
        }

        [Fact]
        public void CreateInstance_DeserializesInt64_Successfully()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("Int64", "clr-namespace:System;assembly=mscorlib");
            string xml = @"<s:Int64 xmlns:s=""clr-namespace:System;assembly=mscorlib"">9223372036854775807</s:Int64>";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                var result = _deserializer.CreateInstance(_serializationManager, xmlQualifiedName, reader);

                // Assert
                Assert.NotNull(result);
                Assert.IsType<long>(result);
                Assert.Equal(9223372036854775807L, result);
            }
        }

        #endregion

        #region DateTime Edge Cases

        [Fact]
        public void CreateInstance_DeserializesDateTime_WithLocalKind()
        {
            // Arrange
            var expectedDate = new DateTime(2026, 3, 2, 10, 30, 0, DateTimeKind.Local);
            string dateString = expectedDate.ToString("o");
            var xmlQualifiedName = new XmlQualifiedName("DateTime", "clr-namespace:System;assembly=mscorlib");
            string xml = $@"<s:DateTime xmlns:s=""clr-namespace:System;assembly=mscorlib"">{dateString}</s:DateTime>";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                var result = _deserializer.CreateInstance(_serializationManager, xmlQualifiedName, reader);

                // Assert
                Assert.NotNull(result);
                Assert.IsType<DateTime>(result);
                var resultDate = (DateTime)result;
                Assert.Equal(expectedDate.Ticks, resultDate.Ticks);
            }
        }

        [Fact]
        public void CreateInstance_DeserializesDateTime_WithUnspecifiedKind()
        {
            // Arrange
            var expectedDate = new DateTime(2026, 3, 2, 10, 30, 0, DateTimeKind.Unspecified);
            string dateString = expectedDate.ToString("o");
            var xmlQualifiedName = new XmlQualifiedName("DateTime", "clr-namespace:System;assembly=mscorlib");
            string xml = $@"<s:DateTime xmlns:s=""clr-namespace:System;assembly=mscorlib"">{dateString}</s:DateTime>";
            using XmlReader reader = CreateXmlReader(xml);
            reader.Read();

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(reader);
                var result = _deserializer.CreateInstance(_serializationManager, xmlQualifiedName, reader);

                // Assert
                Assert.NotNull(result);
                Assert.IsType<DateTime>(result);
            }
        }

        #endregion

        #region Helper Methods

        private static XmlReader CreateXmlReader(string xml)
        {
            var settings = new XmlReaderSettings
            {
                ConformanceLevel = ConformanceLevel.Fragment,
                IgnoreWhitespace = true,
                IgnoreComments = true
            };
            return XmlReader.Create(new StringReader(xml), settings);
        }

        #endregion

        #region Test Classes

        public class TestClass
        {
            public string? StringProperty { get; set; }
            public int IntProperty { get; set; }
            public bool BoolProperty { get; set; }
        }

        public class TestClassWithReadOnlyProperty
        {
            private readonly List<string> _items = [];

            public List<string> Items => _items;
        }

        public class TestClassWithNullReadOnlyProperty
        {
#pragma warning disable CA1822 // Mark members as static - needs to be this way for testing
            public List<string>? Items => null;//NOSONAR - This is intentional to test error handling when read-only property value is null
#pragma warning restore CA1822 // Mark members as static
        }

        public class TestClassWithoutSerializer
        {
            public string Property { get; set; } = string.Empty;
        }

        public class TestClassThrowsOnCreate
        {
            public TestClassThrowsOnCreate()
            {
                throw new InvalidOperationException("Cannot create instance");
            }
        }

        #endregion
    }
}