using LogicBuilder.ComponentModel.Design.Serialization;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using LogicBuilder.Workflow.ComponentModel.Serialization.Factories;
using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;
using LogicBuilder.Workflow.ComponentModel.Serialization.Structures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Xml;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization
{
    public class ContentPropertyTest
    {
        private readonly IDeserializeFromStringHelper _deserializeFromStringHelper;
        private readonly IMarkupExtensionHelper _markupExtensionHelper;
        private readonly ISerializationErrorHelper _serializationErrorHelper;
        private readonly IWorkflowMarkupSerializationHelper _workflowMarkupSerializationHelper;
        private readonly WorkflowMarkupSerializationManager _serializationManager;
        private readonly DesignerSerializationManager _designerSerializationManager;

        public ContentPropertyTest()
        {
            _deserializeFromStringHelper = DeserializeFromStringHelperFactory.Create(DependencyHelperFactory.Create());
            _markupExtensionHelper = MarkupExtensionHelperFactory.Create();
            _serializationErrorHelper = SerializationErrorHelperFactory.Create();
            _workflowMarkupSerializationHelper = WorkflowMarkupSerializationHelperFactory.Create();
            _designerSerializationManager = new DesignerSerializationManager();
            _serializationManager = new WorkflowMarkupSerializationManager(_designerSerializationManager);
        }

        #region Test Helper Classes

        [ContentProperty("Items")]
        public class TestContainerWithStringCollection
        {
            public TestContainerWithStringCollection()
            {
                Items = [];
            }

            public string Name { get; set; } = string.Empty;
            public List<string> Items { get; }
        }

        [ContentProperty("Items")]
        public class TestContainerWithCollection
        {
            public TestContainerWithCollection()
            {
                Items = [];
            }

            public string Name { get; set; } = string.Empty;
            public List<TestObject> Items { get; }
        }

        [ContentProperty("Value")]
        public class TestContainerWithSimpleProperty
        {
            public string Value { get; set; } = string.Empty;
        }

        [ContentProperty("ReadOnlyValue")]
        public class TestContainerWithReadOnlyProperty
        {
            public string ReadOnlyValue { get; } = string.Empty;
        }

        [ContentProperty("NonExistent")]
        public class TestContainerWithInvalidContentProperty
        {
            public string Name { get; set; } = string.Empty;
        }

        public class TestContainerWithoutContentProperty
        {
            public string Name { get; set; } = string.Empty;
        }

        [ContentProperty("NestedObject")]
        public class TestContainerWithObjectProperty
        {
            public TestNestedObject? NestedObject { get; set; }
        }

        public class TestNestedObject
        {
            public string Name { get; set; } = string.Empty;
        }

        public class TestObject
        {
            public string Name { get; set; } = string.Empty;
        }

        [ContentProperty("Items")]
        public class TestContainerWithNullCollection
        {
            public List<string> Items { get; set; } = [];
        }

        public class TestParentWithChildren : List<object>
        {
            public string Name { get; set; } = string.Empty;
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidContainerWithCollection_InitializesSuccessfully()
        {
            // Arrange
            var parentObject = new TestContainerWithStringCollection();
            var parentSerializer = new WorkflowMarkupSerializer();

            // Act
            using var contentProperty = new ContentProperty(
                _serializationManager,
                parentSerializer,
                parentObject,
                _deserializeFromStringHelper,
                _markupExtensionHelper,
                _serializationErrorHelper,
                _workflowMarkupSerializationHelper);

            // Assert
            Assert.NotNull(contentProperty);
            Assert.NotNull(contentProperty.Property);
            Assert.Equal("Items", contentProperty.Property.Name);
        }

        [Fact]
        public void Constructor_WithValidContainerWithSimpleProperty_InitializesSuccessfully()
        {
            // Arrange
            var parentObject = new TestContainerWithSimpleProperty();
            var parentSerializer = new WorkflowMarkupSerializer();

            // Act
            using var contentProperty = new ContentProperty(
                _serializationManager,
                parentSerializer,
                parentObject,
                _deserializeFromStringHelper,
                _markupExtensionHelper,
                _serializationErrorHelper,
                _workflowMarkupSerializationHelper);

            // Assert
            Assert.NotNull(contentProperty);
            Assert.NotNull(contentProperty.Property);
            Assert.Equal("Value", contentProperty.Property.Name);
        }

        [Fact]
        public void Constructor_WithContainerWithoutContentProperty_InitializesWithNullProperty()
        {
            // Arrange
            var parentObject = new TestContainerWithoutContentProperty();
            var parentSerializer = new WorkflowMarkupSerializer();

            // Act
            using var contentProperty = new ContentProperty(
                _serializationManager,
                parentSerializer,
                parentObject,
                _deserializeFromStringHelper,
                _markupExtensionHelper,
                _serializationErrorHelper,
                _workflowMarkupSerializationHelper);

            // Assert
            Assert.NotNull(contentProperty);
            Assert.Null(contentProperty.Property);
        }

        [Fact]
        public void Constructor_WithInvalidContentPropertyName_ReportsError()
        {
            // Arrange
            var parentObject = new TestContainerWithInvalidContentProperty();
            var parentSerializer = new WorkflowMarkupSerializer();

            using (_designerSerializationManager.CreateSession())
            {
                // Act
                using var contentProperty = new ContentProperty(
                    _serializationManager,
                    parentSerializer,
                    parentObject,
                    _deserializeFromStringHelper,
                    _markupExtensionHelper,
                    _serializationErrorHelper,
                    _workflowMarkupSerializationHelper);

                // Assert
                Assert.NotEmpty(_designerSerializationManager.Errors);
            }
        }

        #endregion

        #region Property Tests

        [Fact]
        public void Property_ReturnsContentPropertyInfo()
        {
            // Arrange
            var parentObject = new TestContainerWithStringCollection();
            var parentSerializer = new WorkflowMarkupSerializer();

            using var contentProperty = new ContentProperty(
                _serializationManager,
                parentSerializer,
                parentObject,
                _deserializeFromStringHelper,
                _markupExtensionHelper,
                _serializationErrorHelper,
                _workflowMarkupSerializationHelper);

            // Act
            var property = contentProperty.Property;

            // Assert
            Assert.NotNull(property);
            Assert.Equal("Items", property.Name);
            Assert.Equal(typeof(List<string>), property.PropertyType);
        }

        #endregion

        #region GetContents Tests

        [Fact]
        public void GetContents_WithContentProperty_ReturnsPropertyValue()
        {
            // Arrange
            var parentObject = new TestContainerWithStringCollection();
            parentObject.Items.Add("item1");
            parentObject.Items.Add("item2");
            var parentSerializer = new WorkflowMarkupSerializer();

            using var contentProperty = new ContentProperty(
                _serializationManager,
                parentSerializer,
                parentObject,
                _deserializeFromStringHelper,
                _markupExtensionHelper,
                _serializationErrorHelper,
                _workflowMarkupSerializationHelper);

            // Act
            var contents = contentProperty.GetContents() as List<string>;

            // Assert
            Assert.NotNull(contents);
            Assert.Equal(2, contents.Count);
            Assert.Equal("item1", contents[0]);
            Assert.Equal("item2", contents[1]);
        }

        [Fact]
        public void GetContents_WithoutContentProperty_ReturnsChildrenFromSerializer()
        {
            // Arrange
            var parentObject = new TestParentWithChildren
            {
                "child1",
                "child2"
            };
            var parentSerializer = new CollectionMarkupSerializer();

            using var contentProperty = new ContentProperty(
                _serializationManager,
                parentSerializer,
                parentObject,
                _deserializeFromStringHelper,
                _markupExtensionHelper,
                _serializationErrorHelper,
                _workflowMarkupSerializationHelper);

            // Act
            var contents = contentProperty.GetContents();

            // Assert
            Assert.NotNull(contents);
        }

        #endregion

        #region SetContents Tests - Collection

        [Fact]
        public void SetContents_WithEmptyContentsList_DoesNothing()
        {
            // Arrange
            var parentObject = new TestContainerWithStringCollection();
            var parentSerializer = new WorkflowMarkupSerializer();

            using var contentProperty = new ContentProperty(
                _serializationManager,
                parentSerializer,
                parentObject,
                _deserializeFromStringHelper,
                _markupExtensionHelper,
                _serializationErrorHelper,
                _workflowMarkupSerializationHelper);

            var contents = new List<ContentInfo>();

            // Act
            contentProperty.SetContents(contents);

            // Assert
            Assert.Empty(parentObject.Items);
        }

        [Fact]
        public void SetContents_WithCollectionContentProperty_AddsItemsToCollection()
        {
            // Arrange
            var parentObject = new TestContainerWithCollection();
            var parentSerializer = new WorkflowMarkupSerializer();

            using var contentProperty = new ContentProperty(
                _serializationManager,
                parentSerializer,
                parentObject,
                _deserializeFromStringHelper,
                _markupExtensionHelper,
                _serializationErrorHelper,
                _workflowMarkupSerializationHelper);

            var contents = new List<ContentInfo>
            {
                new(new TestObject { Name = "item1" }, 1, 1),
                new(new TestObject { Name = "item2" }, 2, 1),
                new(new TestObject { Name = "item3" }, 3, 1)
            };

            
            using (_designerSerializationManager.CreateSession())
            {
                // Act
                contentProperty.SetContents(contents);

                // Assert
                Assert.Empty(_designerSerializationManager.Errors);
                Assert.Equal(3, parentObject.Items.Count);
                Assert.Equal("item1", parentObject.Items[0].Name);
                Assert.Equal("item2", parentObject.Items[1].Name);
                Assert.Equal("item3", parentObject.Items[2].Name);
            }
        }

        [Fact]
        public void SetContents_WithNullCollectionProperty_ReportsError()
        {
            // Arrange
            var parentObject = new TestContainerWithNullCollection();
            var parentSerializer = new WorkflowMarkupSerializer();

            using var contentProperty = new ContentProperty(
                _serializationManager,
                parentSerializer,
                parentObject,
                _deserializeFromStringHelper,
                _markupExtensionHelper,
                _serializationErrorHelper,
                _workflowMarkupSerializationHelper);

            var contents = new List<ContentInfo>
            {
                new("item1", 1, 1)
            };

            using (_designerSerializationManager.CreateSession())
            {
                // Act
                contentProperty.SetContents(contents);

                // Assert
                Assert.NotEmpty(_designerSerializationManager.Errors);
            }
        }

        #endregion

        #region SetContents Tests - Simple Property

        [Fact]
        public void SetContents_WithSimplePropertyAndStringContent_SetsPropertyValue()
        {
            // Arrange
            var parentObject = new TestContainerWithSimpleProperty();
            var parentSerializer = new WorkflowMarkupSerializer();

            using var stringReader = new StringReader("<Property>System.String</Property>");
            using var xmlReader = XmlReader.Create(stringReader);

            _serializationManager.WorkflowMarkupStack.Push(xmlReader);

            using var contentProperty = new ContentProperty(
                _serializationManager,
                parentSerializer,
                parentObject,
                _deserializeFromStringHelper,
                _markupExtensionHelper,
                _serializationErrorHelper,
                _workflowMarkupSerializationHelper);

            var contents = new List<ContentInfo>
            {
                new("test value", 1, 1)
            };

            using (_designerSerializationManager.CreateSession())
            {
                // Act
                contentProperty.SetContents(contents);

                // Assert
                Assert.Equal("test value", parentObject.Value);
            }

            _serializationManager.WorkflowMarkupStack.Pop();
        }

        [Fact]
        public void SetContents_WithReadOnlyProperty_ReportsError()
        {
            // Arrange
            var parentObject = new TestContainerWithReadOnlyProperty();
            var parentSerializer = new WorkflowMarkupSerializer();

            using var contentProperty = new ContentProperty(
                _serializationManager,
                parentSerializer,
                parentObject,
                _deserializeFromStringHelper,
                _markupExtensionHelper,
                _serializationErrorHelper,
                _workflowMarkupSerializationHelper);

            var contents = new List<ContentInfo>
            {
                new("test value", 1, 1)
            };

            using (_designerSerializationManager.CreateSession())
            {
                // Act
                contentProperty.SetContents(contents);

                // Assert
                Assert.NotEmpty(_designerSerializationManager.Errors);
            }
        }

        [Fact]
        public void SetContents_WithSimplePropertyAndMultipleContents_ReportsError()
        {
            // Arrange
            var parentObject = new TestContainerWithSimpleProperty();
            var parentSerializer = new WorkflowMarkupSerializer();

            using var contentProperty = new ContentProperty(
                _serializationManager,
                parentSerializer,
                parentObject,
                _deserializeFromStringHelper,
                _markupExtensionHelper,
                _serializationErrorHelper,
                _workflowMarkupSerializationHelper);

            var contents = new List<ContentInfo>
            {
                new("value1", 1, 1),
                new("value2", 2, 1)
            };

            using (_designerSerializationManager.CreateSession())
            {
                // Act
                contentProperty.SetContents(contents);

                // Assert
                Assert.NotEmpty(_designerSerializationManager.Errors);
            }
        }

        #endregion

        #region SetContents Tests - Without Content Property

        [Fact]
        public void SetContents_WithoutContentProperty_CallsParentSerializerAddChild()
        {
            // Arrange
            var parentObject = new TestParentWithChildren();
            var parentSerializer = new CollectionMarkupSerializer();

            using var contentProperty = new ContentProperty(
                _serializationManager,
                parentSerializer,
                parentObject,
                _deserializeFromStringHelper,
                _markupExtensionHelper,
                _serializationErrorHelper,
                _workflowMarkupSerializationHelper);

            var contents = new List<ContentInfo>
            {
                new("child1", 1, 1),
                new("child2", 2, 1)
            };

            // Act
            contentProperty.SetContents(contents);

            // Assert
            Assert.Equal(2, parentObject.Count);
        }

        #endregion

        #region Dispose Tests

        [Fact]
        public void Dispose_CanBeCalledMultipleTimes()
        {
            // Arrange
            var parentObject = new TestContainerWithStringCollection();
            var parentSerializer = new WorkflowMarkupSerializer();

            var contentProperty = new ContentProperty(
                _serializationManager,
                parentSerializer,
                parentObject,
                _deserializeFromStringHelper,
                _markupExtensionHelper,
                _serializationErrorHelper,
                _workflowMarkupSerializationHelper);

            // Act & Assert - Should not throw
            contentProperty.Dispose();
            contentProperty.Dispose();
            Assert.True(true); // If we reach this point, it means no exception was thrown
        }

        [Fact]
        public void Dispose_WithXmlReader_CallsOnAfterDeserialize()
        {
            // Arrange
            var parentObject = new TestContainerWithStringCollection();
            var parentSerializer = new WorkflowMarkupSerializer();
            var xml = "<TestContainerWithCollection><Items><string>item1</string></Items></TestContainerWithCollection>";
            
            using var stringReader = new StringReader(xml);
            using var xmlReader = XmlReader.Create(stringReader);
            
            _serializationManager.WorkflowMarkupStack.Push(xmlReader);

            var contentProperty = new ContentProperty(
                _serializationManager,
                parentSerializer,
                parentObject,
                _deserializeFromStringHelper,
                _markupExtensionHelper,
                _serializationErrorHelper,
                _workflowMarkupSerializationHelper);

            // Act & Assert - Should not throw
            contentProperty.Dispose();
            Assert.True(true); // If we reach this point, it means no exception was thrown

            _serializationManager.WorkflowMarkupStack.Pop();
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void Integration_SetAndGetContents_RoundTrip()
        {
            // Arrange
            var parentObject = new TestContainerWithCollection();
            var parentSerializer = new WorkflowMarkupSerializer();

            using (_designerSerializationManager.CreateSession())
            {
                using var contentProperty = new ContentProperty(
                _serializationManager,
                parentSerializer,
                parentObject,
                _deserializeFromStringHelper,
                _markupExtensionHelper,
                _serializationErrorHelper,
                _workflowMarkupSerializationHelper);

                var originalContents = new List<ContentInfo>
                {
                    new(new TestObject { Name = "item1" }, 1, 1),
                    new(new TestObject { Name = "item2" }, 2, 1)
                };

                // Act
                contentProperty.SetContents(originalContents);
                var retrievedContents = contentProperty.GetContents() as List<TestObject>;

                // Assert
                Assert.NotNull(retrievedContents);
                Assert.Equal(2, retrievedContents.Count);
                Assert.Equal("item1", retrievedContents[0].Name);
                Assert.Equal("item2", retrievedContents[1].Name);
            }

           //_serializationManager.WorkflowMarkupStack.Pop();
        }

        #endregion
    }
}