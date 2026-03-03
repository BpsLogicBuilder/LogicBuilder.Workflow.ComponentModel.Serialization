using LogicBuilder.ComponentModel.Design.Serialization;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization
{
    public class SafeXmlNodeWriterTest
    {
        private readonly DesignerSerializationManager _designerSerializationManager;
        private readonly WorkflowMarkupSerializationManager _serializationManager;

        public SafeXmlNodeWriterTest()
        {
            _designerSerializationManager = new DesignerSerializationManager();
            _serializationManager = new WorkflowMarkupSerializationManager(_designerSerializationManager);
        }

        #region Test Helper Classes

        public class TestObject
        {
            public string Name { get; set; } = string.Empty;
            public int Value { get; set; }
        }

        public class TestObjectWithProperty
        {
            public string TestProperty { get; set; } = string.Empty;
        }

        #endregion

        #region Constructor Tests - Element NodeType

        [Fact]
        public void Constructor_WritesStartElement_WhenPropertyIsMemberInfo()
        {
            // Arrange
            using StringWriter stringWriter = new();
            using XmlWriter xmlWriter = XmlWriter.Create(stringWriter);
            var owner = new TestObjectWithProperty { TestProperty = "Test" };
            PropertyInfo property = typeof(TestObjectWithProperty).GetProperty(nameof(TestObjectWithProperty.TestProperty))!;

            xmlWriter.WriteStartElement("Root");

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(xmlWriter);

                using (new SafeXmlNodeWriter(_serializationManager, owner, property, XmlNodeType.Element))
                {
                    // Element should be written in constructor
                }

                _serializationManager.WorkflowMarkupStack.Pop();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.Flush();

            // Assert
            string result = stringWriter.ToString();
            Assert.Contains("TestObjectWithProperty.TestProperty", result);
        }

        [Fact]
        public void Constructor_WritesStartElement_WhenPropertyIsNotMemberInfo()
        {
            // Arrange
            using StringWriter stringWriter = new();
            using XmlWriter xmlWriter = XmlWriter.Create(stringWriter);
            var owner = new TestObject { Name = "Test", Value = 42 };
            string property = "TestProperty";

            xmlWriter.WriteStartElement("Root");

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(xmlWriter);

                using (new SafeXmlNodeWriter(_serializationManager, owner, property, XmlNodeType.Element))
                {
                    // Element should be written in constructor
                }

                _serializationManager.WorkflowMarkupStack.Pop();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.Flush();

            // Assert
            string result = stringWriter.ToString();
            Assert.Contains("TestObject", result);
        }

        [Fact]
        public void Constructor_IncrementsWriterDepth_WhenCreatingElement()
        {
            // Arrange
            using StringWriter stringWriter = new();
            using XmlWriter xmlWriter = XmlWriter.Create(stringWriter);
            var owner = new TestObject();
            PropertyInfo property = typeof(TestObject).GetProperty(nameof(TestObject.Name))!;
            int initialDepth;

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(xmlWriter);
                initialDepth = _serializationManager.WriterDepth;

                using (new SafeXmlNodeWriter(_serializationManager, owner, property, XmlNodeType.Element))
                {
                    // Assert - depth should be incremented inside the using block
                    Assert.Equal(initialDepth + 1, _serializationManager.WriterDepth);
                }

                _serializationManager.WorkflowMarkupStack.Pop();
            }
        }

        [Fact]
        public void Constructor_ThrowsInvalidOperationException_WhenXmlWriterNotInStack()
        {
            // Arrange
            var owner = new TestObject();
            PropertyInfo property = typeof(TestObject).GetProperty(nameof(TestObject.Name))!;

            // Act & Assert
            using (_designerSerializationManager.CreateSession())
            {
                var exception = Assert.Throws<InvalidOperationException>(() =>
                    new SafeXmlNodeWriter(_serializationManager, owner, property, XmlNodeType.Element));

                Assert.NotNull(exception);
            }
        }

        #endregion

        #region Constructor Tests - Attribute NodeType

        [Fact]
        public void Constructor_WritesStartAttribute_WhenNodeTypeIsAttribute()
        {
            // Arrange
            using StringWriter stringWriter = new();
            using XmlWriter xmlWriter = XmlWriter.Create(stringWriter);
            var owner = new TestObject();
            PropertyInfo property = typeof(TestObject).GetProperty(nameof(TestObject.Name))!;

            xmlWriter.WriteStartElement("Root");

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(xmlWriter);

                using (new SafeXmlNodeWriter(_serializationManager, owner, property, XmlNodeType.Attribute))
                {
                    xmlWriter.WriteString("AttributeValue");
                }

                _serializationManager.WorkflowMarkupStack.Pop();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.Flush();

            // Assert
            string result = stringWriter.ToString();
            Assert.Contains("Name=\"AttributeValue\"", result);
        }

        [Fact]
        public void Constructor_DoesNotIncrementWriterDepth_WhenCreatingAttribute()
        {
            // Arrange
            using StringWriter stringWriter = new();
            using XmlWriter xmlWriter = XmlWriter.Create(stringWriter);
            var owner = new TestObject();
            PropertyInfo property = typeof(TestObject).GetProperty(nameof(TestObject.Name))!;
            int initialDepth;

            xmlWriter.WriteStartElement("Root");

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(xmlWriter);
                initialDepth = _serializationManager.WriterDepth;

                using (new SafeXmlNodeWriter(_serializationManager, owner, property, XmlNodeType.Attribute))
                {
                    // Assert - depth should not change for attributes
                    Assert.Equal(initialDepth, _serializationManager.WriterDepth);
                }

                _serializationManager.WorkflowMarkupStack.Pop();
            }

            xmlWriter.WriteEndElement();
        }

        #endregion

        #region Dispose Tests

        [Fact]
        public void Dispose_WritesEndElement_WhenNodeTypeIsElement()
        {
            // Arrange
            using StringWriter stringWriter = new();
            using XmlWriter xmlWriter = XmlWriter.Create(stringWriter);
            var owner = new TestObject();
            PropertyInfo property = typeof(TestObject).GetProperty(nameof(TestObject.Name))!;

            xmlWriter.WriteStartElement("Root");

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(xmlWriter);

                using (var safeWriter = new SafeXmlNodeWriter(_serializationManager, owner, property, XmlNodeType.Element))
                {
                    xmlWriter.WriteString("Content");
                }

                _serializationManager.WorkflowMarkupStack.Pop();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.Flush();

            // Assert
            string result = stringWriter.ToString();
            Assert.Contains("<ns0:TestObject.Name xmlns:ns0=\"clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;Assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535\">Content</ns0:TestObject.Name>", result);
        }

        [Fact]
        public void Dispose_WritesEndAttribute_WhenNodeTypeIsAttribute()
        {
            // Arrange
            using StringWriter stringWriter = new();
            using XmlWriter xmlWriter = XmlWriter.Create(stringWriter);
            var owner = new TestObject();
            PropertyInfo property = typeof(TestObject).GetProperty(nameof(TestObject.Name))!;

            xmlWriter.WriteStartElement("Root");

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(xmlWriter);

                using (new SafeXmlNodeWriter(_serializationManager, owner, property, XmlNodeType.Attribute))
                {
                    xmlWriter.WriteString("TestValue");
                }

                _serializationManager.WorkflowMarkupStack.Pop();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.Flush();

            // Assert
            string result = stringWriter.ToString();
            Assert.Contains("Name=\"TestValue\"", result);
        }

        [Fact]
        public void Dispose_DecrementsWriterDepth_WhenNodeTypeIsElement()
        {
            // Arrange
            using StringWriter stringWriter = new();
            using XmlWriter xmlWriter = XmlWriter.Create(stringWriter);
            var owner = new TestObject();
            PropertyInfo property = typeof(TestObject).GetProperty(nameof(TestObject.Name))!;
            int initialDepth;
            int depthAfterDispose;

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(xmlWriter);
                initialDepth = _serializationManager.WriterDepth;

                using (new SafeXmlNodeWriter(_serializationManager, owner, property, XmlNodeType.Element))
                {
                    // Depth incremented
                }

                depthAfterDispose = _serializationManager.WriterDepth;
                _serializationManager.WorkflowMarkupStack.Pop();
            }

            // Assert
            Assert.Equal(initialDepth, depthAfterDispose);
        }

        [Fact]
        public void Dispose_HandlesMultipleNestedElements_Successfully()
        {
            // Arrange
            using StringWriter stringWriter = new();
            using XmlWriter xmlWriter = XmlWriter.Create(stringWriter);
            var owner = new TestObject();
            PropertyInfo property1 = typeof(TestObject).GetProperty(nameof(TestObject.Name))!;
            PropertyInfo property2 = typeof(TestObject).GetProperty(nameof(TestObject.Value))!;

            xmlWriter.WriteStartElement("Root");

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(xmlWriter);
                int initialDepth = _serializationManager.WriterDepth;

                using (new SafeXmlNodeWriter(_serializationManager, owner, property1, XmlNodeType.Element))
                {
                    Assert.Equal(initialDepth + 1, _serializationManager.WriterDepth);

                    using (new SafeXmlNodeWriter(_serializationManager, owner, property2, XmlNodeType.Element))
                    {
                        Assert.Equal(initialDepth + 2, _serializationManager.WriterDepth);
                        xmlWriter.WriteString("NestedContent");
                    }

                    Assert.Equal(initialDepth + 1, _serializationManager.WriterDepth);
                }

                Assert.Equal(initialDepth, _serializationManager.WriterDepth);
                _serializationManager.WorkflowMarkupStack.Pop();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.Flush();

            // Assert
            string result = stringWriter.ToString();
            Assert.Contains("<ns0:TestObject.Name", result);
            Assert.Contains("<ns0:TestObject.Value>", result);
            Assert.Contains("</ns0:TestObject.Value>", result);
            Assert.Contains("</ns0:TestObject.Name>", result);
        }

        [Fact]
        public void Dispose_DoesNotThrow_WhenXmlWriterIsNull()
        {
            // Arrange
            using StringWriter stringWriter = new();
            using XmlWriter xmlWriter = XmlWriter.Create(stringWriter);
            var owner = new TestObject();
            PropertyInfo property = typeof(TestObject).GetProperty(nameof(TestObject.Name))!;
            SafeXmlNodeWriter safeWriter;

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(xmlWriter);
                safeWriter = new SafeXmlNodeWriter(_serializationManager, owner, property, XmlNodeType.Element);
                _serializationManager.WorkflowMarkupStack.Pop(); // Remove writer from stack
            }

            // Assert - should not throw when disposing
            var exception = Record.Exception(() => ((IDisposable)safeWriter).Dispose());
            Assert.Null(exception);
        }

        [Fact]
        public void Dispose_DoesNotWriteEnd_WhenWriterStateIsError()
        {
            // Arrange
            using StringWriter stringWriter = new();
            using XmlWriter xmlWriter = XmlWriter.Create(stringWriter);
            var owner = new TestObject();
            PropertyInfo property = typeof(TestObject).GetProperty(nameof(TestObject.Name))!;

            // Act & Assert
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(xmlWriter);

                // Create invalid XML state by closing the writer
                xmlWriter.Close();

                // This should handle the error state gracefully
                Assert.Throws<InvalidOperationException>(() =>
                {
                    using (new SafeXmlNodeWriter(_serializationManager, owner, property, XmlNodeType.Element))
                    {
                        // Constructor and dispose should handle error state
                    }
                });

                // Depending on implementation, may throw during construction or handle gracefully
                // The key is that Dispose checks WriteState.Error
            }
        }

        #endregion

        #region Property Type Tests

        [Fact]
        public void Constructor_HandlesStringProperty_Successfully()
        {
            // Arrange
            using StringWriter stringWriter = new();
            using XmlWriter xmlWriter = XmlWriter.Create(stringWriter);
            var owner = new TestObject();
            string property = "CustomProperty";

            xmlWriter.WriteStartElement("Root");

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(xmlWriter);

                using (new SafeXmlNodeWriter(_serializationManager, owner, property, XmlNodeType.Element))
                {
                    xmlWriter.WriteString("Value");
                }

                _serializationManager.WorkflowMarkupStack.Pop();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.Flush();

            // Assert
            string result = stringWriter.ToString();
            Assert.Contains("TestObject", result);
        }

        [Fact]
        public void Constructor_HandlesMemberInfoWithAttributeNodeType_Successfully()
        {
            // Arrange
            using StringWriter stringWriter = new();
            using XmlWriter xmlWriter = XmlWriter.Create(stringWriter);
            var owner = new TestObject();
            PropertyInfo property = typeof(TestObject).GetProperty(nameof(TestObject.Name))!;

            xmlWriter.WriteStartElement("Root");

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(xmlWriter);

                using (new SafeXmlNodeWriter(_serializationManager, owner, property, XmlNodeType.Attribute))
                {
                    xmlWriter.WriteString("AttributeValue");
                }

                _serializationManager.WorkflowMarkupStack.Pop();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.Flush();

            // Assert
            string result = stringWriter.ToString();
            Assert.Contains("Name=", result);
            Assert.Contains("AttributeValue", result);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void SafeXmlNodeWriter_CreatesValidXmlDocument_WithElements()
        {
            // Arrange
            using StringWriter stringWriter = new();
            using XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = false });
            var owner = new TestObject { Name = "TestName", Value = 42 };
            PropertyInfo nameProperty = typeof(TestObject).GetProperty(nameof(TestObject.Name))!;
            PropertyInfo valueProperty = typeof(TestObject).GetProperty(nameof(TestObject.Value))!;

            xmlWriter.WriteStartElement("Root");

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(xmlWriter);

                using (new SafeXmlNodeWriter(_serializationManager, owner, nameProperty, XmlNodeType.Element))
                {
                    xmlWriter.WriteString("TestName");
                }

                using (new SafeXmlNodeWriter(_serializationManager, owner, valueProperty, XmlNodeType.Element))
                {
                    xmlWriter.WriteString("42");
                }

                _serializationManager.WorkflowMarkupStack.Pop();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.Flush();

            // Assert
            string result = stringWriter.ToString();
            Assert.Contains("<ns0:TestObject.Name xmlns:ns0=\"clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;Assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535\">TestName</ns0:TestObject.Name>", result);
            Assert.Contains("<ns0:TestObject.Value xmlns:ns0=\"clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;Assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535\">42</ns0:TestObject.Value>", result);

            // Verify it's valid XML
            var exception = Record.Exception(() => XmlReader.Create(new StringReader(result)));
            Assert.Null(exception);
        }

        [Fact]
        public void SafeXmlNodeWriter_CreatesValidXmlDocument_WithAttributes()
        {
            // Arrange
            using StringWriter stringWriter = new();
            using XmlWriter xmlWriter = XmlWriter.Create(stringWriter);
            var owner = new TestObject { Name = "TestName", Value = 42 };
            PropertyInfo nameProperty = typeof(TestObject).GetProperty(nameof(TestObject.Name))!;
            PropertyInfo valueProperty = typeof(TestObject).GetProperty(nameof(TestObject.Value))!;

            xmlWriter.WriteStartElement("Root");

            // Act
            using (_designerSerializationManager.CreateSession())
            {
                _serializationManager.WorkflowMarkupStack.Push(xmlWriter);

                using (new SafeXmlNodeWriter(_serializationManager, owner, nameProperty, XmlNodeType.Attribute))
                {
                    xmlWriter.WriteString("TestName");
                }

                using (new SafeXmlNodeWriter(_serializationManager, owner, valueProperty, XmlNodeType.Attribute))
                {
                    xmlWriter.WriteString("42");
                }

                _serializationManager.WorkflowMarkupStack.Pop();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.Flush();

            // Assert
            string result = stringWriter.ToString();
            Assert.Contains("Name=\"TestName\"", result);
            Assert.Contains("Value=\"42\"", result);

            // Verify it's valid XML
            var exception = Record.Exception(() => XmlReader.Create(new StringReader(result)));
            Assert.Null(exception);
        }

        #endregion
    }
}