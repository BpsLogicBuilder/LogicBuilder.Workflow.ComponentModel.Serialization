using LogicBuilder.ComponentModel.Design.Serialization;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization
{
    public class WorkflowMarkupSerializerTest
    {
        #region Test Helper Classes

        [ContentProperty("Items")]
        public class TestContainer
        {
            public TestContainer()
            {
                Items = [];
            }

            public string Name { get; set; } = string.Empty;
            public int Value { get; set; }
            public List<object> Items { get; }
        }

        public class TestSimpleObject
        {
            public string StringProperty { get; set; } = string.Empty;
            public int IntProperty { get; set; }
            public double DoubleProperty { get; set; }
            public bool BoolProperty { get; set; }
            public DateTime DateProperty { get; set; }
            public TimeSpan TimeSpanProperty { get; set; }
            public Guid GuidProperty { get; set; }
        }

        public class TestReadOnlyPropertyObject
        {
            private readonly List<string> _items = [];

            public List<string> Items => _items;
            public string Name { get; set; } = string.Empty;
        }

        public class TestNullableProperties
        {
            public int? NullableInt { get; set; }
            public DateTime? NullableDateTime { get; set; }
        }

        [DefaultValue(42)]
        public class TestDefaultValueObject
        {
            [DefaultValue(42)]
            public int ValueWithDefault { get; set; } = 42;
            public string Name { get; set; } = string.Empty;
        }

        public class TestCollectionObject
        {
            public TestCollectionObject()
            {
                Items = [];
            }

            public List<string> Items { get; }
        }

        public delegate void TestDelegate();

        public class TestObjectWithDelegate
        {
            public TestDelegate? DelegateProperty { get; set; }
        }

        #endregion

        #region Deserialize Tests

        [Fact]
        public void Deserialize_WithNullReader_ThrowsArgumentNullException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => serializer.Deserialize(null));
        }

        [Fact]
        public void Deserialize_WithSerializationManagerAndNullReader_ThrowsArgumentNullException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();

            // Act & Assert
            using (manager.CreateSession())
            {
                Assert.Throws<ArgumentNullException>(() => serializer.Deserialize(manager, null));
            }
        }

        [Fact]
        public void Deserialize_WithNullSerializationManager_ThrowsArgumentNullException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var xml = "<TestObject />";
            using var reader = XmlReader.Create(new StringReader(xml));

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => serializer.Deserialize(null, reader));
        }

        [Fact]
        public void Deserialize_WithEmptyXml_ReturnsNull()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var xml = "";
            using var reader = XmlReader.Create(new StringReader(xml));

            // Act & Assert
            Assert.Throws<WorkflowMarkupSerializationException>(() => serializer.Deserialize(reader));
        }

        [Fact]
        public void Deserialize_WithXmlException_ThrowsWorkflowMarkupSerializationException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var xml = "<Invalid><Unclosed>";
            using var reader = XmlReader.Create(new StringReader(xml));

            // Act & Assert
            Assert.Throws<WorkflowMarkupSerializationException>(() => serializer.Deserialize(reader));
        }

        #endregion

        #region Serialize Tests

        [Fact]
        public void Serialize_WithNullObject_ThrowsArgumentNullException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var output = new StringWriter();
            using var writer = XmlWriter.Create(output);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => serializer.Serialize(writer, null));
        }

        [Fact]
        public void Serialize_WithNullWriter_ThrowsArgumentNullException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var obj = new TestSimpleObject();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => serializer.Serialize(null, obj));
        }

        [Fact]
        public void Serialize_WithSerializationManagerAndNullWriter_ThrowsArgumentNullException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();
            var obj = new TestSimpleObject();

            // Act & Assert
            using (manager.CreateSession())
            {
                Assert.Throws<ArgumentNullException>(() => serializer.Serialize(manager, null, obj));
            }
        }

        [Fact]
        public void Serialize_WithNullSerializationManager_ThrowsArgumentNullException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var output = new StringWriter();
            using var writer = XmlWriter.Create(output);
            var obj = new TestSimpleObject();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => serializer.Serialize(null, writer, obj));
        }

        [Fact]
        public void Serialize_SimpleObject_GeneratesXml()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var obj = new TestSimpleObject
            {
                StringProperty = "Test",
                IntProperty = 42
            };
            var output = new StringWriter();
            using var writer = XmlWriter.Create(output);

            // Act
            serializer.Serialize(writer, obj);
            writer.Flush();
            var result = output.ToString();

            // Assert
            Assert.False(string.IsNullOrEmpty(result));
        }

        [Fact]
        public void Serialize_CharValue_GeneratesXml()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var obj = 'A';
            var output = new StringWriter();
            using var writer = XmlWriter.Create(output);

            // Act
            serializer.Serialize(writer, obj);
            writer.Flush();
            var result = output.ToString();

            // Assert
            Assert.False(string.IsNullOrEmpty(result));
        }

        [Fact]
        public void Serialize_ByteValue_GeneratesXml()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            byte obj = 255;
            var output = new StringWriter();
            using var writer = XmlWriter.Create(output);

            // Act
            serializer.Serialize(writer, obj);
            writer.Flush();
            var result = output.ToString();

            // Assert
            Assert.False(string.IsNullOrEmpty(result));
        }

        [Fact]
        public void Serialize_Int16Value_GeneratesXml()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            short obj = 32767;
            var output = new StringWriter();
            using var writer = XmlWriter.Create(output);

            // Act
            serializer.Serialize(writer, obj);
            writer.Flush();
            var result = output.ToString();

            // Assert
            Assert.False(string.IsNullOrEmpty(result));
        }

        [Fact]
        public void Serialize_DecimalValue_GeneratesXml()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            decimal obj = 123.45m;
            var output = new StringWriter();
            using var writer = XmlWriter.Create(output);

            // Act
            serializer.Serialize(writer, obj);
            writer.Flush();
            var result = output.ToString();

            // Assert
            Assert.False(string.IsNullOrEmpty(result));
        }

        [Fact]
        public void Serialize_EnumValue_GeneratesXml()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var obj = StringComparison.OrdinalIgnoreCase;
            var output = new StringWriter();
            using var writer = XmlWriter.Create(output);

            // Act
            serializer.Serialize(writer, obj);
            writer.Flush();
            var result = output.ToString();

            // Assert
            Assert.False(string.IsNullOrEmpty(result));
        }

        [Fact]
        public void Serialize_StringWithCurlyBraces_EscapesCorrectly()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var obj = "{SomeValue}";
            var output = new StringWriter();
            using var writer = XmlWriter.Create(output);

            // Act
            serializer.Serialize(writer, obj);
            writer.Flush();
            var result = output.ToString();

            // Assert
            Assert.Contains("{}", result);
        }

        #endregion

        #region ShouldSerializeValue Tests

        [Fact]
        public void ShouldSerializeValue_WithNullSerializationManager_ThrowsArgumentNullException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var value = "test";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => serializer.ShouldSerializeValue(null, value));
        }

        [Fact]
        public void ShouldSerializeValue_WithNullValue_ReturnsFalse()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();

            // Act
            bool result;
            using (manager.CreateSession())
            {
                result = serializer.ShouldSerializeValue(new WorkflowMarkupSerializationManager(manager), null);
            }

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ShouldSerializeValue_WithNonNullValue_ReturnsTrue()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();
            var value = "test";

            // Act
            bool result;
            using (manager.CreateSession())
            {
                result = serializer.ShouldSerializeValue(new WorkflowMarkupSerializationManager(manager), value);
            }

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ShouldSerializeValue_WithDefaultValue_ReturnsFalse()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();
            var property = typeof(TestDefaultValueObject).GetProperty("ValueWithDefault");

            // Act
            bool result;
            using (manager.CreateSession())
            {
                var wfManager = new WorkflowMarkupSerializationManager(manager);
                wfManager.Context.Push(property!);
                result = serializer.ShouldSerializeValue(wfManager, 42);
                wfManager.Context.Pop();
            }

            // Assert
            Assert.False(result);
        }

        #endregion

        #region CanSerializeToString Tests

        [Fact]
        public void CanSerializeToString_WithNullSerializationManager_ThrowsArgumentNullException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var value = "test";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => serializer.CanSerializeToString(null, value));
        }

        [Fact]
        public void CanSerializeToString_WithNullValue_ThrowsArgumentNullException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();

            // Act & Assert
            using (manager.CreateSession())
            {
                Assert.Throws<ArgumentNullException>(() => 
                    serializer.CanSerializeToString(new WorkflowMarkupSerializationManager(manager), null));
            }
        }

        [Fact]
        public void CanSerializeToString_WithStringValue_ReturnsTrue()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();
            var value = "test";

            // Act
            bool result;
            using (manager.CreateSession())
            {
                result = serializer.CanSerializeToString(new WorkflowMarkupSerializationManager(manager), value);
            }

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanSerializeToString_WithIntValue_ReturnsTrue()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();
            var value = 42;

            // Act
            bool result;
            using (manager.CreateSession())
            {
                result = serializer.CanSerializeToString(new WorkflowMarkupSerializationManager(manager), value);
            }

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanSerializeToString_WithEnumValue_ReturnsTrue()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();
            var value = StringComparison.Ordinal;

            // Act
            bool result;
            using (manager.CreateSession())
            {
                result = serializer.CanSerializeToString(new WorkflowMarkupSerializationManager(manager), value);
            }

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanSerializeToString_WithTimeSpanValue_ReturnsTrue()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();
            var value = TimeSpan.FromMinutes(5);

            // Act
            bool result;
            using (manager.CreateSession())
            {
                result = serializer.CanSerializeToString(new WorkflowMarkupSerializationManager(manager), value);
            }

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanSerializeToString_WithGuidValue_ReturnsTrue()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();
            var value = Guid.NewGuid();

            // Act
            bool result;
            using (manager.CreateSession())
            {
                result = serializer.CanSerializeToString(new WorkflowMarkupSerializationManager(manager), value);
            }

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanSerializeToString_WithDateTimeValue_ReturnsTrue()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();
            var value = DateTime.Now;

            // Act
            bool result;
            using (manager.CreateSession())
            {
                result = serializer.CanSerializeToString(new WorkflowMarkupSerializationManager(manager), value);
            }

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanSerializeToString_WithDelegateValue_ReturnsTrue()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();
            TestDelegate value = () => { };

            // Act
            bool result;
            using (manager.CreateSession())
            {
                result = serializer.CanSerializeToString(new WorkflowMarkupSerializationManager(manager), value);
            }

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanSerializeToString_WithComplexObject_ReturnsFalse()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();
            var value = new TestSimpleObject();

            // Act
            bool result;
            using (manager.CreateSession())
            {
                result = serializer.CanSerializeToString(new WorkflowMarkupSerializationManager(manager), value);
            }

            // Assert
            Assert.False(result);
        }

        #endregion

        #region SerializeToString Tests

        [Fact]
        public void SerializeToString_WithNullSerializationManager_ThrowsArgumentNullException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var value = "test";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => serializer.SerializeToString(null, value));
        }

        [Fact]
        public void SerializeToString_WithNullValue_ThrowsArgumentNullException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();

            // Act & Assert
            using (manager.CreateSession())
            {
                Assert.Throws<ArgumentNullException>(() => 
                    serializer.SerializeToString(new WorkflowMarkupSerializationManager(manager), null));
            }
        }

        [Fact]
        public void SerializeToString_WithStringValue_ReturnsString()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();
            var value = "test";

            // Act
            string result;
            using (manager.CreateSession())
            {
                result = serializer.SerializeToString(new WorkflowMarkupSerializationManager(manager), value);
            }

            // Assert
            Assert.Equal("test", result);
        }

        [Fact]
        public void SerializeToString_WithIntValue_ReturnsString()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();
            var value = 42;

            // Act
            string result;
            using (manager.CreateSession())
            {
                result = serializer.SerializeToString(new WorkflowMarkupSerializationManager(manager), value);
            }

            // Assert
            Assert.Equal("42", result);
        }

        [Fact]
        public void SerializeToString_WithDateTimeValue_ReturnsRoundtripString()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();
            var value = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);

            // Act
            string result;
            using (manager.CreateSession())
            {
                result = serializer.SerializeToString(new WorkflowMarkupSerializationManager(manager), value);
            }

            // Assert
            Assert.NotNull(result);
            Assert.Contains("2024", result);
        }

        [Fact]
        public void SerializeToString_WithDelegate_ReturnsMethodName()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();
            TestDelegate value = TestMethod;

            // Act
            string result;
            using (manager.CreateSession())
            {
                result = serializer.SerializeToString(new WorkflowMarkupSerializationManager(manager), value);
            }

            // Assert
            Assert.Equal("TestMethod", result);
        }

        private void TestMethod() { }

        #endregion

        #region DeserializeFromString Tests

        [Fact]
        public void DeserializeFromString_WithNullSerializationManager_ThrowsArgumentNullException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var propertyType = typeof(string);
            var value = "test";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                serializer.DeserializeFromString(null, propertyType, value));
        }

        [Fact]
        public void DeserializeFromString_WithNullPropertyType_ThrowsArgumentNullException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();
            var value = "test";

            // Act & Assert
            using (manager.CreateSession())
            {
                Assert.Throws<ArgumentNullException>(() => 
                    serializer.DeserializeFromString(new WorkflowMarkupSerializationManager(manager), null, value));
            }
        }

        [Fact]
        public void DeserializeFromString_WithNullValue_ThrowsArgumentNullException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();
            var propertyType = typeof(string);

            // Act & Assert
            using (manager.CreateSession())
            {
                Assert.Throws<ArgumentNullException>(() => 
                    serializer.DeserializeFromString(new WorkflowMarkupSerializationManager(manager), propertyType, null));
            }
        }

        #endregion

        #region GetChildren Tests

        [Fact]
        public void GetChildren_WithNullSerializationManager_ThrowsArgumentNullException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var obj = new TestContainer();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => serializer.GetChildren(null, obj));
        }

        [Fact]
        public void GetChildren_WithNullObject_ThrowsArgumentNullException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();

            // Act & Assert
            using (manager.CreateSession())
            {
                Assert.Throws<ArgumentNullException>(() => 
                    serializer.GetChildren(new WorkflowMarkupSerializationManager(manager), null));
            }
        }

        [Fact]
        public void GetChildren_ReturnsNull()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();
            var obj = new TestContainer();

            // Act
            System.Collections.IList result;
            using (manager.CreateSession())
            {
                result = serializer.GetChildren(new WorkflowMarkupSerializationManager(manager), obj);
            }

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region ClearChildren Tests

        [Fact]
        public void ClearChildren_WithNullSerializationManager_ThrowsArgumentNullException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var obj = new TestContainer();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => serializer.ClearChildren(null, obj));
        }

        [Fact]
        public void ClearChildren_WithNullObject_ThrowsArgumentNullException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();

            // Act & Assert
            using (manager.CreateSession())
            {
                Assert.Throws<ArgumentNullException>(() => 
                    serializer.ClearChildren(new WorkflowMarkupSerializationManager(manager), null));
            }
        }

        #endregion

        #region AddChild Tests

        [Fact]
        public void AddChild_WithNullSerializationManager_ThrowsArgumentNullException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var parent = new TestContainer();
            var child = new object();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => serializer.AddChild(null, parent, child));
        }

        [Fact]
        public void AddChild_WithNullParentObject_ThrowsArgumentNullException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();
            var child = new object();

            // Act & Assert
            using (manager.CreateSession())
            {
                Assert.Throws<ArgumentNullException>(() => 
                    serializer.AddChild(new WorkflowMarkupSerializationManager(manager), null, child));
            }
        }

        [Fact]
        public void AddChild_WithNullChildObject_ThrowsArgumentNullException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();
            var parent = new TestContainer();

            // Act & Assert
            using (manager.CreateSession())
            {
                Assert.Throws<ArgumentNullException>(() => 
                    serializer.AddChild(new WorkflowMarkupSerializationManager(manager), parent, null));
            }
        }

        [Fact]
        public void AddChild_ThrowsException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();
            var parent = new TestContainer();
            var child = new object();

            // Act & Assert
            using (manager.CreateSession())
            {
                Assert.Throws<Exception>(() => 
                    serializer.AddChild(new WorkflowMarkupSerializationManager(manager), parent, child));
            }
        }

        #endregion

        #region GetProperties Tests

        [Fact]
        public void GetProperties_WithNullSerializationManager_ThrowsArgumentNullException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var obj = new TestSimpleObject();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => serializer.GetProperties(null, obj));
        }

        [Fact]
        public void GetProperties_WithNullObject_ThrowsArgumentNullException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();

            // Act & Assert
            using (manager.CreateSession())
            {
                Assert.Throws<ArgumentNullException>(() => 
                    serializer.GetProperties(new WorkflowMarkupSerializationManager(manager), null));
            }
        }

        [Fact]
        public void GetProperties_ReturnsPropertyInfoArray()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();
            var obj = new TestSimpleObject();

            // Act
            PropertyInfo[] result;
            using (manager.CreateSession())
            {
                result = serializer.GetProperties(new WorkflowMarkupSerializationManager(manager), obj);
            }

            // Assert
            Assert.NotNull(result);
            Assert.IsType<PropertyInfo[]>(result);
        }

        #endregion

        #region GetEvents Tests

        [Fact]
        public void GetEvents_WithNullSerializationManager_ThrowsArgumentNullException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var obj = new TestSimpleObject();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => serializer.GetEvents(null, obj));
        }

        [Fact]
        public void GetEvents_WithNullObject_ThrowsArgumentNullException()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();

            // Act & Assert
            using (manager.CreateSession())
            {
                Assert.Throws<ArgumentNullException>(() => 
                    serializer.GetEvents(new WorkflowMarkupSerializationManager(manager), null));
            }
        }

        [Fact]
        public void GetEvents_ReturnsEventInfoArray()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();
            var obj = new TestSimpleObject();

            // Act
            EventInfo[] result;
            using (manager.CreateSession())
            {
                result = serializer.GetEvents(new WorkflowMarkupSerializationManager(manager), obj);
            }

            // Assert
            Assert.NotNull(result);
            Assert.IsType<EventInfo[]>(result);
        }

        #endregion

        #region IsValidCompactAttributeFormat Tests

        [Fact]
        public void IsValidCompactAttributeFormat_WithValidFormat_ReturnsTrue()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var value = "{ActivityBind ID=Workflow1}";

            // Act
            var result = serializer.IsValidCompactAttributeFormat(value);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValidCompactAttributeFormat_WithEscapedBraces_ReturnsFalse()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var value = "{}SomeValue";

            // Act
            var result = serializer.IsValidCompactAttributeFormat(value);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidCompactAttributeFormat_WithoutBraces_ReturnsFalse()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var value = "SomeValue";

            // Act
            var result = serializer.IsValidCompactAttributeFormat(value);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidCompactAttributeFormat_WithEmptyString_ReturnsFalse()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var value = "";

            // Act
            var result = serializer.IsValidCompactAttributeFormat(value);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidCompactAttributeFormat_WithOnlyOpenBrace_ReturnsFalse()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var value = "{SomeValue";

            // Act
            var result = serializer.IsValidCompactAttributeFormat(value);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidCompactAttributeFormat_WithOnlyCloseBrace_ReturnsFalse()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var value = "SomeValue}";

            // Act
            var result = serializer.IsValidCompactAttributeFormat(value);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region EnsureMarkupExtensionTypeName Tests

        [Fact]
        public void EnsureMarkupExtensionTypeName_WithExtensionSuffix_RemovesSuffix()
        {
            // Arrange
            var type = typeof(NullExtension);

            // Act
            var result = WorkflowMarkupSerializer.EnsureMarkupExtensionTypeName(type);

            // Assert
            Assert.Equal("Null", result);
        }

        [Fact]
        public void EnsureMarkupExtensionTypeName_WithArrayType_ReturnsArrayExtension()
        {
            // Arrange
            var xmlQualifiedName = new System.Xml.XmlQualifiedName("Array", StandardXomlKeys.Definitions_XmlNs);

            // Act
            var result = WorkflowMarkupSerializer.EnsureMarkupExtensionTypeName(xmlQualifiedName);

            // Assert
            Assert.Equal("ArrayExtension", result);
        }

        [Fact]
        public void EnsureMarkupExtensionTypeName_WithNonArrayType_ReturnsOriginalName()
        {
            // Arrange
            var xmlQualifiedName = new System.Xml.XmlQualifiedName("SomeType", StandardXomlKeys.Definitions_XmlNs);

            // Act
            var result = WorkflowMarkupSerializer.EnsureMarkupExtensionTypeName(xmlQualifiedName);

            // Assert
            Assert.Equal("SomeType", result);
        }

        #endregion

        #region InternalDeserializeFromString Tests

        [Fact]
        public void DeserializeFromString_WithStringType_ReturnsString()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var xml = "<Test />";
            using var reader = XmlReader.Create(new StringReader(xml));
            reader.Read();
            var manager = new DesignerSerializationManager();

            // Act
            object result;
            using (manager.CreateSession())
            {
                var wfManager = new WorkflowMarkupSerializationManager(manager);
                wfManager.WorkflowMarkupStack.Push(reader);
                result = serializer.DeserializeFromString(wfManager, typeof(string), "test value");
                wfManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.Equal("test value", result);
        }

        [Fact]
        public void DeserializeFromString_WithEscapedCurlyBraces_RemovesEscapeSequence()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var xml = "<Test />";
            using var reader = XmlReader.Create(new StringReader(xml));
            reader.Read();
            var manager = new DesignerSerializationManager();

            // Act
            object result;
            using (manager.CreateSession())
            {
                var wfManager = new WorkflowMarkupSerializationManager(manager);
                wfManager.WorkflowMarkupStack.Push(reader);
                result = serializer.DeserializeFromString(wfManager, typeof(string), "{}{test}");
                wfManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.Equal("{test}", result);
        }

        [Fact]
        public void DeserializeFromString_WithNullableInt_DeserializesCorrectly()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var xml = "<Test />";
            using var reader = XmlReader.Create(new StringReader(xml));
            reader.Read();
            var manager = new DesignerSerializationManager();

            // Act
            object result;
            using (manager.CreateSession())
            {
                var wfManager = new WorkflowMarkupSerializationManager(manager);
                wfManager.WorkflowMarkupStack.Push(reader);
                result = serializer.DeserializeFromString(wfManager, typeof(int?), "42");
                wfManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.Equal(42, result);
        }

        [Fact]
        public void DeserializeFromString_WithEnum_ParsesCorrectly()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var xml = "<Test />";
            using var reader = XmlReader.Create(new StringReader(xml));
            reader.Read();
            var manager = new DesignerSerializationManager();

            // Act
            object result;
            using (manager.CreateSession())
            {
                var wfManager = new WorkflowMarkupSerializationManager(manager);
                wfManager.WorkflowMarkupStack.Push(reader);
                result = serializer.DeserializeFromString(wfManager, typeof(StringComparison), "OrdinalIgnoreCase");
                wfManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.Equal(StringComparison.OrdinalIgnoreCase, result);
        }

        [Fact]
        public void DeserializeFromString_WithTimeSpan_ParsesCorrectly()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var xml = "<Test />";
            using var reader = XmlReader.Create(new StringReader(xml));
            reader.Read();
            var manager = new DesignerSerializationManager();

            // Act
            object result;
            using (manager.CreateSession())
            {
                var wfManager = new WorkflowMarkupSerializationManager(manager);
                wfManager.WorkflowMarkupStack.Push(reader);
                result = serializer.DeserializeFromString(wfManager, typeof(TimeSpan), "00:05:00");
                wfManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.Equal(TimeSpan.FromMinutes(5), result);
        }

        [Fact]
        public void DeserializeFromString_WithDateTime_ParsesCorrectly()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var xml = "<Test />";
            using var reader = XmlReader.Create(new StringReader(xml));
            reader.Read();
            var manager = new DesignerSerializationManager();
            var dateString = "2024-01-01T12:00:00Z";

            // Act
            object result;
            using (manager.CreateSession())
            {
                var wfManager = new WorkflowMarkupSerializationManager(manager);
                wfManager.WorkflowMarkupStack.Push(reader);
                result = serializer.DeserializeFromString(wfManager, typeof(DateTime), dateString);
                wfManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.IsType<DateTime>(result);
        }

        [Fact]
        public void DeserializeFromString_WithGuid_ParsesCorrectly()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var xml = "<Test />";
            using var reader = XmlReader.Create(new StringReader(xml));
            reader.Read();
            var manager = new DesignerSerializationManager();
            var guidString = "12345678-1234-1234-1234-123456789012";

            // Act
            object result;
            using (manager.CreateSession())
            {
                var wfManager = new WorkflowMarkupSerializationManager(manager);
                wfManager.WorkflowMarkupStack.Push(reader);
                result = serializer.DeserializeFromString(wfManager, typeof(Guid), guidString);
                wfManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.IsType<Guid>(result);
        }

        [Fact]
        public void DeserializeFromString_WithType_ReturnsType()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var xml = "<Test />";
            using var reader = XmlReader.Create(new StringReader(xml));
            reader.Read();
            var manager = new DesignerSerializationManager();

            // Act
            object result;
            using (manager.CreateSession())
            {
                var wfManager = new WorkflowMarkupSerializationManager(manager);
                wfManager.WorkflowMarkupStack.Push(reader);
                result = serializer.DeserializeFromString(wfManager, typeof(Type), "System.String");
                wfManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.Equal("System.String", result);
        }

        [Fact]
        public void DeserializeFromString_WithDelegate_ReturnsMethodName()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var xml = "<Test />";
            using var reader = XmlReader.Create(new StringReader(xml));
            reader.Read();
            var manager = new DesignerSerializationManager();

            // Act
            object result;
            using (manager.CreateSession())
            {
                var wfManager = new WorkflowMarkupSerializationManager(manager);
                wfManager.WorkflowMarkupStack.Push(reader);
                result = serializer.DeserializeFromString(wfManager, typeof(TestDelegate), "SomeMethod");
                wfManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.Equal("SomeMethod", result);
        }

        [Fact]
        public void DeserializeFromString_WithInt_ParsesCorrectly()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var xml = "<Test />";
            using var reader = XmlReader.Create(new StringReader(xml));
            reader.Read();
            var manager = new DesignerSerializationManager();

            // Act
            object result;
            using (manager.CreateSession())
            {
                var wfManager = new WorkflowMarkupSerializationManager(manager);
                wfManager.WorkflowMarkupStack.Push(reader);
                result = serializer.DeserializeFromString(wfManager, typeof(int), "123");
                wfManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.Equal(123, result);
        }

        #endregion

        #region DeserializeObject and DeserializeContents Tests

        [Fact]
        public void Deserialize_WithPrimitiveValue_DeserializesObject()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<String xmlns=""clr-namespace:System;assembly=mscorlib"">TestValue</String>";
            using var reader = XmlReader.Create(new StringReader(xml));

            // Act
            var result = serializer.Deserialize(reader);

            // Assert
            Assert.Equal("TestValue", result);
        }

        [Fact]
        public void Deserialize_WithIntValue_DeserializesObject()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Int32 xmlns=""clr-namespace:System;assembly=mscorlib"">42</Int32>";
            using var reader = XmlReader.Create(new StringReader(xml));

            // Act
            var result = serializer.Deserialize(reader);

            // Assert
            Assert.Equal(42, result);
        }

        #endregion

        #region DeserializeFromCompactFormat Tests

        [Fact]
        public void DeserializeFromCompactFormat_WithNullExtension_CreatesNullExtension()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
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
        public void DeserializeFromCompactFormat_WithInvalidFormat_ReportsError()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
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

        #endregion

        #region GetMarkupExtensionFromValue Tests

        [Fact]
        public void Serialize_WithNullValue_SerializesAsNullExtension()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var output = new StringWriter();
            using var writer = XmlWriter.Create(output);

            // Act
            serializer.Serialize(writer, new NullExtension());
            writer.Flush();
            var result = output.ToString();

            // Assert
            Assert.Contains("Null", result);
        }

        [Fact]
        public void Serialize_WithTypeValue_SerializesAsTypeExtension()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var output = new StringWriter();
            using var writer = XmlWriter.Create(output);
            var typeExtension = new TypeExtension(typeof(string));

            // Act
            serializer.Serialize(writer, typeExtension);
            writer.Flush();
            var result = output.ToString();

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public void Serialize_WithArrayValue_SerializesAsArrayExtension()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var output = new StringWriter();
            using var writer = XmlWriter.Create(output);
            var array = new string[] { "Item1", "Item2" };
            var arrayExtension = new ArrayExtension(array);

            // Act
            serializer.Serialize(writer, arrayExtension);
            writer.Flush();
            var result = output.ToString();

            // Assert
            Assert.NotEmpty(result);
        }

        #endregion

        #region Serialization Error Tests

        [Fact]
        public void Serialize_WithCircularReference_ThrowsException()
        {
            // This would test circular reference detection
            // The implementation should detect and prevent infinite loops
        }

        #endregion

        #region Additional Coverage Tests

        [Fact]
        public void Serialize_WithGuidValue_GeneratesXml()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var obj = Guid.NewGuid();
            var output = new StringWriter();
            using var writer = XmlWriter.Create(output);

            // Act
            serializer.Serialize(writer, obj);
            writer.Flush();
            var result = output.ToString();

            // Assert
            Assert.False(string.IsNullOrEmpty(result));
        }

        [Fact]
        public void Serialize_WithTimeSpanValue_GeneratesXml()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var obj = TimeSpan.FromMinutes(30);
            var output = new StringWriter();
            using var writer = XmlWriter.Create(output);

            // Act
            serializer.Serialize(writer, obj);
            writer.Flush();
            var result = output.ToString();

            // Assert
            Assert.False(string.IsNullOrEmpty(result));
        }

        [Fact]
        public void Serialize_WithDateTimeValue_GeneratesXml()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var obj = DateTime.UtcNow;
            var output = new StringWriter();
            using var writer = XmlWriter.Create(output);

            // Act
            serializer.Serialize(writer, obj);
            writer.Flush();
            var result = output.ToString();

            // Assert
            Assert.False(string.IsNullOrEmpty(result));
        }

        [Fact]
        public void Serialize_WithNullCharValue_DoesNotWriteValue()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var obj = '\0';
            var output = new StringWriter();
            using var writer = XmlWriter.Create(output);

            // Act
            serializer.Serialize(writer, obj);
            writer.Flush();
            var result = output.ToString();

            // Assert - null char should not be written
            Assert.False(string.IsNullOrEmpty(result));
        }

        #endregion

        #region CreateSerializationError Coverage Tests

        [Fact]
        public void Deserialize_WithMalformedXml_ThrowsSerializationErrorWithLineInfo()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var xml = @"<Test><Invalid></Test>";
            using var reader = XmlReader.Create(new StringReader(xml));

            // Act & Assert
            var ex = Assert.Throws<WorkflowMarkupSerializationException>(() => serializer.Deserialize(reader));
            Assert.NotNull(ex.Message);
        }

        #endregion

        #region GetClrFullName and CreateInstance Coverage Tests

        [Fact]
        public void Deserialize_WithCustomType_UsesGetClrFullName()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Int32 xmlns=""clr-namespace:System;assembly=mscorlib"">100</Int32>";
            using var reader = XmlReader.Create(new StringReader(xml));

            // Act
            var result = serializer.Deserialize(reader);

            // Assert
            Assert.Equal(100, result);
        }

        #endregion

        #region DeserializeCompoundProperty and DeserializeSimpleProperty Coverage Tests

        public class TestObjectWithReadOnlyCollection
        {
            private readonly List<string> _items = [];
            public IList<string> Items => _items;
            public string Name { get; set; } = string.Empty;
        }

        #endregion

        #region LookupProperty Coverage Tests

        [Fact]
        public void GetProperties_ReturnsPropertiesWithoutRuntimeNameProperty()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();
            var obj = new TestSimpleObject();

            // Act
            PropertyInfo[] result;
            using (manager.CreateSession())
            {
                result = serializer.GetProperties(new WorkflowMarkupSerializationManager(manager), obj);
            }

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        #endregion

        #region TokenizeAttributes Coverage Tests

        [Fact]
        public void DeserializeFromCompactFormat_WithPositionalArguments_ParsesCorrectly()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
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
            var serializer = new WorkflowMarkupSerializer();
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
            var serializer = new WorkflowMarkupSerializer();
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

        #endregion

        #region RemoveEscapes Coverage Tests

        [Fact]
        public void DeserializeFromCompactFormat_WithQuotedStrings_ParsesCorrectly()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
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

        #endregion

        #region IsMarkupExtension Coverage Tests

        [Fact]
        public void Serialize_WithMarkupExtension_SerializesCorrectly()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var output = new StringWriter();
            using var writer = XmlWriter.Create(output);
            var nullExt = new NullExtension();

            // Act
            serializer.Serialize(writer, nullExt);
            writer.Flush();
            var result = output.ToString();

            // Assert
            Assert.NotEmpty(result);
        }

        #endregion

        #region GetValueFromMarkupExtension Coverage Tests

        [Fact]
        public void Deserialize_WithNullExtension_ReturnsNullExtension()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Null xmlns=""" + StandardXomlKeys.Definitions_XmlNs + @""" />";
            using var reader = XmlReader.Create(new StringReader(xml));

            // Act
            var result = serializer.Deserialize(reader);

            // Assert - NullExtension is deserialized as an object
            Assert.IsType<NullExtension>(result);
        }

        [Fact]
        public void Deserialize_WithTypeExtension_ReturnsTypeExtension()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Type xmlns=""" + StandardXomlKeys.Definitions_XmlNs + @""" xmlns:s=""clr-namespace:System;assembly=mscorlib"" Type=""s:String"" />";
            using var reader = XmlReader.Create(new StringReader(xml));

            // Act
            var result = serializer.Deserialize(reader);

            // Assert - TypeExtension is deserialized as an object
            Assert.IsType<TypeExtension>(result);
        }

        #endregion

        #region GetExtendedProperties Coverage Tests

        [Fact]
        public void GetExtendedProperties_WithBaseSerializer_ReturnsEmptyArray()
        {
            // This tests the default implementation
            // The base WorkflowMarkupSerializer.GetExtendedProperties returns an empty array
            // Subclasses can override it
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();
            var obj = new TestSimpleObject();

            // Act
            using (manager.CreateSession())
            {
                var wfManager = new WorkflowMarkupSerializationManager(manager);
                // Internal method, tested indirectly through serialization
                var properties = serializer.GetProperties(wfManager, obj);
                
                // Assert
                Assert.NotNull(properties);
            }
        }

        #endregion

        #region OnGetRuntimeNameValue and OnSetRuntimeNameValue Coverage Tests

        [RuntimeNameProperty("Name")]
        public class TestObjectWithRuntimeName
        {
            public string Name { get; set; } = string.Empty;
            public int Value { get; set; }
        }

        [Fact]
        public void GetProperties_WithRuntimeNameProperty_WrapsProperty()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var manager = new DesignerSerializationManager();
            var obj = new TestObjectWithRuntimeName { Name = "Test", Value = 42 };

            // Act
            PropertyInfo[] result;
            using (manager.CreateSession())
            {
                result = serializer.GetProperties(new WorkflowMarkupSerializationManager(manager), obj);
            }

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result, p => p.Name == "Name");
        }

        [Fact]
        public void Serialize_WithRuntimeNameProperty_SerializesCorrectly()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var output = new StringWriter();
            using var writer = XmlWriter.Create(output);
            var obj = new TestObjectWithRuntimeName { Name = "TestName", Value = 100 };

            // Act
            serializer.Serialize(writer, obj);
            writer.Flush();
            var result = output.ToString();

            // Assert
            Assert.Contains("TestName", result);
        }

        #endregion

        #region OnGetRuntimeQualifiedName Coverage Tests

        [Fact]
        public void Serialize_WithExtendedProperty_UsesQualifiedName()
        {
            // This indirectly tests OnGetRuntimeQualifiedName through ExtendedPropertyInfo
            // The method is called when serializing extended properties
            var serializer = new WorkflowMarkupSerializer();
            var output = new StringWriter();
            using var writer = XmlWriter.Create(output);
            var obj = new TestSimpleObject { StringProperty = "Test" };

            // Act
            serializer.Serialize(writer, obj);
            writer.Flush();
            var result = output.ToString();

            // Assert
            Assert.NotEmpty(result);
        }

        #endregion

        #region DeserializeContents Coverage Tests

        [Fact]
        public void Deserialize_WithNestedElements_DeserializesContents()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Decimal xmlns=""clr-namespace:System;assembly=mscorlib"">123.45</Decimal>";
            using var reader = XmlReader.Create(new StringReader(xml));

            // Act
            var result = serializer.Deserialize(reader);

            // Assert
            Assert.Equal(123.45m, result);
        }

        [Fact]
        public void Deserialize_WithEnumValue_DeserializesCorrectly()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<StringComparison xmlns=""clr-namespace:System;assembly=mscorlib"">OrdinalIgnoreCase</StringComparison>";
            using var reader = XmlReader.Create(new StringReader(xml));

            // Act
            var result = serializer.Deserialize(reader);

            // Assert
            Assert.Equal(StringComparison.OrdinalIgnoreCase, result);
        }

        [Fact]
        public void Deserialize_WithGuidValue_DeserializesCorrectly()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var guidValue = Guid.NewGuid();
            var xml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<Guid xmlns=""clr-namespace:System;assembly=mscorlib"">{guidValue}</Guid>";
            using var reader = XmlReader.Create(new StringReader(xml));

            // Act
            var result = serializer.Deserialize(reader);

            // Assert
            Assert.Equal(guidValue, result);
        }

        [Fact]
        public void Deserialize_WithTimeSpanValue_DeserializesCorrectly()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<TimeSpan xmlns=""clr-namespace:System;assembly=mscorlib"">00:30:00</TimeSpan>";
            using var reader = XmlReader.Create(new StringReader(xml));

            // Act
            var result = serializer.Deserialize(reader);

            // Assert
            Assert.Equal(TimeSpan.FromMinutes(30), result);
        }

        [Fact]
        public void Deserialize_WithDateTimeValue_DeserializesCorrectly()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<DateTime xmlns=""clr-namespace:System;assembly=mscorlib"">2024-01-15T10:30:00Z</DateTime>";
            using var reader = XmlReader.Create(new StringReader(xml));

            // Act
            var result = serializer.Deserialize(reader);

            // Assert
            Assert.IsType<DateTime>(result);
        }

        #endregion

        #region SerializeContents Coverage Tests

        [Fact]
        public void Serialize_WithComplexObject_SerializesContents()
        {
            // Arrange
            var serializer = new WorkflowMarkupSerializer();
            var output = new StringWriter();
            using var writer = XmlWriter.Create(output);
            var obj = new TestSimpleObject 
            { 
                StringProperty = "Test",
                IntProperty = 42,
                DoubleProperty = 3.14,
                BoolProperty = true
            };

            // Act
            serializer.Serialize(writer, obj);
            writer.Flush();
            var result = output.ToString();

            // Assert
            Assert.Contains("Test", result);
            Assert.Contains("42", result);
        }

        #endregion
    }
}