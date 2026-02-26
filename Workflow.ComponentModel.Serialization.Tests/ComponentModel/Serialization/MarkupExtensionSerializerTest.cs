using LogicBuilder.ComponentModel.Design.Serialization;
using LogicBuilder.Workflow.ComponentModel.Design;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Xml;

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
            Assert.Throws<System.InvalidOperationException>(() => _serializer.SerializeToString(_serializationManager, markupExtension));
        }

        [Fact]
        public void SerializeToString_ThrowsArgumentNullException_WhenValueIsNull()
        {
            // Arrange
            var sb = new StringBuilder();
            using var writer = XmlWriter.Create(sb);
            _serializationManager.WorkflowMarkupStack.Push(writer);

            try
            {
                // Act & Assert
                Assert.Throws<ArgumentNullException>(() => _serializer.SerializeToString(_serializationManager, null));
            }
            finally
            {
                _serializationManager.WorkflowMarkupStack.Pop();
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

        [Fact]
        public void SerializeToString_SerializesSimpleMarkupExtension()
        {
            // Arrange
            var markupExtension = new TestMarkupExtension();
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true };
            using var writer = XmlWriter.Create(sb, settings);

            _serializationManager.WorkflowMarkupStack.Push(writer);

            try
            {
                writer.WriteStartElement("", "Root", "http://schemas.microsoft.com/winfx/2006/xaml");
                writer.WriteAttributeString("xmlns", "ns0", null, "clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;Assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535");

                using (((DesignerSerializationManager)_serializationManager.SerializationManager).CreateSession())
                {
                    // Act
                    _serializer.SerializeToString(_serializationManager, markupExtension);
                }

                writer.WriteEndElement();
                writer.Flush();

                // Assert
                var result = sb.ToString();
                Assert.Contains("{", result);
                Assert.Contains("}", result);
            }
            finally
            {
                _serializationManager.WorkflowMarkupStack.Pop();
                writer.Dispose();
            }
        }

        [Fact]
        public void SerializeToString_SerializesMarkupExtensionWithProperties()
        {
            // Arrange
            var designerSerializationManager = new DesignerSerializationManager();
            var manager = new WorkflowMarkupSerializationManager(designerSerializationManager);
            
            var markupExtension = new TestMarkupExtensionWithProperties
            {
                StringProperty = "test",
                IntProperty = 42
            };
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true };
            using var writer = XmlWriter.Create(sb, settings);
            manager.WorkflowMarkupStack.Push(writer);

            try
            {
                writer.WriteStartElement("", "Root", "http://schemas.microsoft.com/winfx/2006/xaml");
                writer.WriteAttributeString("xmlns", "ns0", null, "clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;Assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535");

                using (designerSerializationManager.CreateSession())
                {
                    // Act
                    _serializer.SerializeToString(manager, markupExtension);
                }

                writer.WriteEndElement();
                writer.Flush();

                // Assert
                var result = sb.ToString();
                Assert.Contains("{", result);
                Assert.Contains("}", result);
                Assert.Contains("StringProperty", result);
            }
            finally
            {
                manager.WorkflowMarkupStack.Pop();
                writer.Dispose();
            }
        }

        [Fact]
        public void SerializeToString_EscapesSpecialCharactersInPropertyValues()
        {
            // Arrange
            var designerSerializationManager = new DesignerSerializationManager();
            var manager = new WorkflowMarkupSerializationManager(designerSerializationManager);

            var markupExtension = new TestMarkupExtensionWithProperties
            {
                StringProperty = "test=value,with{special}chars"
            };
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true };
            using var writer = XmlWriter.Create(sb, settings);
            manager.WorkflowMarkupStack.Push(writer);

            try
            {
                writer.WriteStartElement("", "Root", "http://schemas.microsoft.com/winfx/2006/xaml");
                writer.WriteAttributeString("xmlns", "ns0", null, "clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;Assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535");

                using (designerSerializationManager.CreateSession())
                {
                    // Act
                    _serializer.SerializeToString(manager, markupExtension);
                }

                writer.WriteEndElement();
                writer.Flush();

                // Assert
                var result = sb.ToString();
                Assert.Contains("\\", result); // Should contain escape characters
            }
            finally
            {
                manager.WorkflowMarkupStack.Pop();
                writer.Dispose();
            }
        }

        [Fact]
        public void SerializeToString_SerializesConstructorArguments()
        {
            // Arrange
            var designerSerializationManager = new DesignerSerializationManager();
            var manager = new WorkflowMarkupSerializationManager(designerSerializationManager);

            var serializer = new TestableMarkupExtensionSerializerWithConstructor();
            var markupExtension = new TestMarkupExtensionWithConstructor("testValue");
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true };
            using var writer = XmlWriter.Create(sb, settings);
            manager.WorkflowMarkupStack.Push(writer);

            try
            {
                writer.WriteStartElement("", "Root", "http://schemas.microsoft.com/winfx/2006/xaml");
                writer.WriteAttributeString("xmlns", "ns0", null, "clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;Assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535");

                using (designerSerializationManager.CreateSession())
                {
                    // Act
                    serializer.SerializeToString(manager, markupExtension);
                }

                writer.WriteEndElement();
                writer.Flush();

                // Assert
                var result = sb.ToString();
                Assert.Contains("testValue", result);
            }
            finally
            {
                manager.WorkflowMarkupStack.Pop();
                writer.Dispose();
            }
        }

        [Fact]
        public void SerializeToString_HandlesNullConstructorArguments()
        {
            // Arrange
            var designerSerializationManager = new DesignerSerializationManager();
            var manager = new WorkflowMarkupSerializationManager(designerSerializationManager);

            var serializer = new TestableMarkupExtensionSerializerWithNullableConstructor();
            var markupExtension = new TestMarkupExtensionWithNullableConstructor(null);
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true };
            using var writer = XmlWriter.Create(sb, settings);
            manager.WorkflowMarkupStack.Push(writer);

            try
            {
                writer.WriteStartElement("", "Root", "http://schemas.microsoft.com/winfx/2006/xaml");
                writer.WriteAttributeString("xmlns", "ns0", null, "clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;Assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535");

                using (designerSerializationManager.CreateSession())
                {
                    // Act
                    serializer.SerializeToString(manager, markupExtension);
                }

                writer.WriteEndElement();
                writer.Flush();

                // Assert - should complete without error
                var result = sb.ToString();
                Assert.Contains("{", result);
                Assert.Contains("}", result);
            }
            finally
            {
                manager.WorkflowMarkupStack.Pop();
                writer.Dispose();
            }
        }

        [Fact]
        public void SerializeToString_HandlesTypeConstructorArguments()
        {
            // Arrange
            var designerSerializationManager = new DesignerSerializationManager();
            var manager = new WorkflowMarkupSerializationManager(designerSerializationManager);

            var serializer = new TestableMarkupExtensionSerializerWithTypeConstructor();
            var markupExtension = new TestMarkupExtensionWithTypeConstructor(typeof(string));
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true };
            using var writer = XmlWriter.Create(sb, settings);
            manager.WorkflowMarkupStack.Push(writer);

            try
            {
                writer.WriteStartElement("", "Root", "http://schemas.microsoft.com/winfx/2006/xaml");
                writer.WriteAttributeString("xmlns", "ns0", null, "clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;Assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535");
                writer.WriteAttributeString("xmlns", "ns1", null, "clr-namespace:System;Assembly=System.Private.CoreLib, Version=10.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e");

                using (designerSerializationManager.CreateSession())
                {
                    // Act
                    serializer.SerializeToString(manager, markupExtension);
                }

                writer.WriteEndElement();
                writer.Flush();

                // Assert
                var result = sb.ToString();
                Assert.Contains("{", result);
                Assert.Contains("}", result);
            }
            finally
            {
                manager.WorkflowMarkupStack.Pop();
                writer.Dispose();
            }
        }

        [Fact]
        public void SerializeToString_HandlesIntConstructorArguments()
        {
            // Arrange
            var designerSerializationManager = new DesignerSerializationManager();
            var manager = new WorkflowMarkupSerializationManager(designerSerializationManager);

            var serializer = new TestableMarkupExtensionSerializerWithIntConstructor();
            var markupExtension = new TestMarkupExtensionWithIntConstructor(42);
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true };
            using var writer = XmlWriter.Create(sb, settings);
            manager.WorkflowMarkupStack.Push(writer);

            try
            {
                writer.WriteStartElement("", "Root", "http://schemas.microsoft.com/winfx/2006/xaml");
                writer.WriteAttributeString("xmlns", "ns0", null, "clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;Assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535");

                using (designerSerializationManager.CreateSession())
                {
                    // Act
                    serializer.SerializeToString(manager, markupExtension);
                }

                writer.WriteEndElement();
                writer.Flush();

                // Assert
                var result = sb.ToString();
                Assert.Contains("42", result);
            }
            finally
            {
                manager.WorkflowMarkupStack.Pop();
                writer.Dispose();
            }
        }

        [Fact]
        public void SerializeToString_HandlesTypePropertyValues()
        {
            // Arrange
            var designerSerializationManager = new DesignerSerializationManager();
            var manager = new WorkflowMarkupSerializationManager(designerSerializationManager);

            var markupExtension = new TestMarkupExtensionWithTypeProperty
            {
                TypeProperty = typeof(int)
            };
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true };
            using var writer = XmlWriter.Create(sb, settings);
            manager.WorkflowMarkupStack.Push(writer);

            try
            {
                writer.WriteStartElement("", "Root", "http://schemas.microsoft.com/winfx/2006/xaml");
                writer.WriteAttributeString("xmlns", "ns0", null, "clr-namespace:LogicBuilder.Workflow.Tests.ComponentModel.Serialization;Assembly=LogicBuilder.Workflow.ComponentModel.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535");

                using (designerSerializationManager.CreateSession())
                {
                    // Act
                    _serializer.SerializeToString(manager, markupExtension);
                }

                writer.WriteEndElement();
                writer.Flush();

                // Assert
                var result = sb.ToString();
                Assert.Contains("TypeProperty", result);
            }
            finally
            {
                manager.WorkflowMarkupStack.Pop();
                writer.Dispose();
            }
        }

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
                    BindingFlags.NonPublic | BindingFlags.Static);
                return (string)method!.Invoke(this, [value])!;
            }
        }

        private class TestableMarkupExtensionSerializerWithConstructor : MarkupExtensionSerializer
        {
            protected override InstanceDescriptor GetInstanceDescriptor(WorkflowMarkupSerializationManager serializationManager, object value)
            {
                if (value is TestMarkupExtensionWithConstructor ext)
                {
                    var ctor = typeof(TestMarkupExtensionWithConstructor).GetConstructor([typeof(string)]);
                    return new InstanceDescriptor(ctor, new object[] { ext.Value });
                }
                return base.GetInstanceDescriptor(serializationManager, value);
            }
        }

        private class TestableMarkupExtensionSerializerWithNullableConstructor : MarkupExtensionSerializer
        {
            protected override InstanceDescriptor GetInstanceDescriptor(WorkflowMarkupSerializationManager serializationManager, object value)
            {
                if (value is TestMarkupExtensionWithNullableConstructor ext)
                {
                    var ctor = typeof(TestMarkupExtensionWithNullableConstructor).GetConstructor([typeof(string)]);
                    return new InstanceDescriptor(ctor, new object[] { ext.Value! });
                }
                return base.GetInstanceDescriptor(serializationManager, value);
            }
        }

        private class TestableMarkupExtensionSerializerWithTypeConstructor : MarkupExtensionSerializer
        {
            protected override InstanceDescriptor GetInstanceDescriptor(WorkflowMarkupSerializationManager serializationManager, object value)
            {
                if (value is TestMarkupExtensionWithTypeConstructor ext)
                {
                    var ctor = typeof(TestMarkupExtensionWithTypeConstructor).GetConstructor([typeof(Type)]);
                    return new InstanceDescriptor(ctor, new object[] { ext.TargetType });
                }
                return base.GetInstanceDescriptor(serializationManager, value);
            }
        }

        private class TestableMarkupExtensionSerializerWithIntConstructor : MarkupExtensionSerializer
        {
            protected override InstanceDescriptor GetInstanceDescriptor(WorkflowMarkupSerializationManager serializationManager, object value)
            {
                if (value is TestMarkupExtensionWithIntConstructor ext)
                {
                    var ctor = typeof(TestMarkupExtensionWithIntConstructor).GetConstructor([typeof(int)]);
                    return new InstanceDescriptor(ctor, new object[] { ext.Value });
                }
                return base.GetInstanceDescriptor(serializationManager, value);
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

        private class TestMarkupExtensionWithNullableConstructor(string? value) : MarkupExtension
        {
            private readonly string? _value = value;

            [ConstructorArgument("value")]
            public string? Value
            {
                get { return _value; }
            }

            public override object ProvideValue(IServiceProvider provider)
            {
                return _value ?? string.Empty;
            }
        }

        private class TestMarkupExtensionWithTypeConstructor(Type targetType) : MarkupExtension
        {
            private readonly Type _targetType = targetType;

            [ConstructorArgument("targetType")]
            public Type TargetType
            {
                get { return _targetType; }
            }

            public override object ProvideValue(IServiceProvider provider)
            {
                return _targetType;
            }
        }

        private class TestMarkupExtensionWithIntConstructor(int value) : MarkupExtension
        {
            private readonly int _value = value;

            [ConstructorArgument("value")]
            public int Value
            {
                get { return _value; }
            }

            public override object ProvideValue(IServiceProvider provider)
            {
                return _value;
            }
        }

        private class TestMarkupExtensionWithTypeProperty : MarkupExtension
        {
            public TestMarkupExtensionWithTypeProperty() { }

            public Type TypeProperty { get; set; } = typeof(object);

            public override object ProvideValue(IServiceProvider provider)
            {
                return this;
            }
        }

        #endregion
    }
}