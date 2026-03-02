using LogicBuilder.Workflow.ComponentModel.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Xunit;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization
{
    public class WorkflowMarkupSerializationHelperTest
    {
        private readonly WorkflowMarkupSerializationHelper _helper;

        public WorkflowMarkupSerializationHelperTest()
        {
            _helper = new WorkflowMarkupSerializationHelper();
        }

        #region AdvanceReader Tests

        [Fact]
        public void AdvanceReader_StopsAtElement()
        {
            // Arrange
            string xml = "<root><child>text</child></root>";
            using XmlReader reader = XmlReader.Create(new StringReader(xml));
            reader.Read(); // Move to root element

            // Act
            _helper.AdvanceReader(reader);

            // Assert
            Assert.Equal(XmlNodeType.Element, reader.NodeType);
            Assert.Equal("root", reader.Name);
        }

        [Fact]
        public void AdvanceReader_StopsAtText()
        {
            // Arrange
            string xml = "<root>text content</root>";
            using XmlReader reader = XmlReader.Create(new StringReader(xml));
            reader.Read(); // Move to root element
            reader.Read(); // Move to text

            // Act
            _helper.AdvanceReader(reader);

            // Assert
            Assert.Equal(XmlNodeType.Text, reader.NodeType);
            Assert.Equal("text content", reader.Value);
        }

        [Fact]
        public void AdvanceReader_StopsAtEndElement()
        {
            // Arrange
            string xml = "<root></root>";
            using XmlReader reader = XmlReader.Create(new StringReader(xml));
            reader.Read(); // Move to root element
            reader.Read(); // Move to end element

            // Act
            _helper.AdvanceReader(reader);

            // Assert
            Assert.Equal(XmlNodeType.EndElement, reader.NodeType);
            Assert.Equal("root", reader.Name);
        }

        [Fact]
        public void AdvanceReader_SkipsWhitespace()
        {
            // Arrange
            string xml = "<root>  <child /></root>";
            using XmlReader reader = XmlReader.Create(new StringReader(xml));
            reader.Read(); // Move to root element
            reader.Read(); // Move to whitespace

            // Act
            _helper.AdvanceReader(reader);

            // Assert
            Assert.Equal(XmlNodeType.Element, reader.NodeType);
            Assert.Equal("child", reader.Name);
        }

        [Fact]
        public void AdvanceReader_SkipsComments()
        {
            // Arrange
            string xml = "<root><!-- comment --><child /></root>";
            using XmlReader reader = XmlReader.Create(new StringReader(xml));
            reader.Read(); // Move to root element
            reader.Read(); // Move to comment

            // Act
            _helper.AdvanceReader(reader);

            // Assert
            Assert.Equal(XmlNodeType.Element, reader.NodeType);
            Assert.Equal("child", reader.Name);
        }

        [Fact]
        public void AdvanceReader_SkipsProcessingInstructions()
        {
            // Arrange
            string xml = "<?xml version=\"1.0\"?><root><?custom instruction?><child /></root>";
            using XmlReader reader = XmlReader.Create(new StringReader(xml));
            reader.Read(); // Move to declaration
            reader.Read(); // Move to root element
            reader.Read(); // Move to processing instruction

            // Act
            _helper.AdvanceReader(reader);

            // Assert
            Assert.Equal(XmlNodeType.Element, reader.NodeType);
            Assert.Equal("child", reader.Name);
        }

        [Fact]
        public void AdvanceReader_SkipsMultipleNonTargetNodes()
        {
            // Arrange
            string xml = "<root><!-- comment -->  <?custom instruction?>  <child /></root>";
            using XmlReader reader = XmlReader.Create(new StringReader(xml));
            reader.Read(); // Move to root element
            reader.Read(); // Move to comment

            // Act
            _helper.AdvanceReader(reader);

            // Assert
            Assert.Equal(XmlNodeType.Element, reader.NodeType);
            Assert.Equal("child", reader.Name);
        }

        [Fact]
        public void AdvanceReader_HandlesReaderAtEndOfDocument()
        {
            // Arrange
            string xml = "<root />";
            using XmlReader reader = XmlReader.Create(new StringReader(xml));
            reader.Read(); // Move to root element
            reader.Read(); // Move past end of document

            // Act
            _helper.AdvanceReader(reader);

            // Assert
            Assert.True(reader.EOF);
        }

        [Fact]
        public void AdvanceReader_StopsAtTextAfterSkippingWhitespace()
        {
            // Arrange
            string xml = "<root>  text</root>";
            using XmlReader reader = XmlReader.Create(new StringReader(xml));
            reader.Read(); // Move to root element
            reader.Read(); // Move to whitespace

            // Act
            _helper.AdvanceReader(reader);

            // Assert
            Assert.Equal(XmlNodeType.Text, reader.NodeType);
        }

        [Fact]
        public void AdvanceReader_StopsAtEndElementAfterSkippingWhitespace()
        {
            // Arrange
            string xml = "<root>  </root>";
            using XmlReader reader = XmlReader.Create(new StringReader(xml));
            reader.Read(); // Move to root element
            reader.Read(); // Move to whitespace

            // Act
            _helper.AdvanceReader(reader);

            // Assert
            Assert.Equal(XmlNodeType.EndElement, reader.NodeType);
            Assert.Equal("root", reader.Name);
        }

        #endregion

        #region IsValidCompactAttributeFormat Tests

        [Fact]
        public void IsValidCompactAttributeFormat_ReturnsTrueForValidFormat()
        {
            // Arrange
            string attributeValue = "{Binding Path=Name}";

            // Act
            bool result = _helper.IsValidCompactAttributeFormat(attributeValue);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValidCompactAttributeFormat_ReturnsTrueForMinimalValidFormat()
        {
            // Arrange
            string attributeValue = "{x}";

            // Act
            bool result = _helper.IsValidCompactAttributeFormat(attributeValue);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValidCompactAttributeFormat_ReturnsFalseForEmptyString()
        {
            // Arrange
            string attributeValue = "";

            // Act
            bool result = _helper.IsValidCompactAttributeFormat(attributeValue);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidCompactAttributeFormat_ReturnsFalseForEmptyBraces()
        {
            // Arrange
            string attributeValue = "{}";

            // Act
            bool result = _helper.IsValidCompactAttributeFormat(attributeValue);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidCompactAttributeFormat_ReturnsFalseForMissingOpeningBrace()
        {
            // Arrange
            string attributeValue = "Binding Path=Name}";

            // Act
            bool result = _helper.IsValidCompactAttributeFormat(attributeValue);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidCompactAttributeFormat_ReturnsFalseForMissingClosingBrace()
        {
            // Arrange
            string attributeValue = "{Binding Path=Name";

            // Act
            bool result = _helper.IsValidCompactAttributeFormat(attributeValue);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidCompactAttributeFormat_ReturnsFalseForNoBraces()
        {
            // Arrange
            string attributeValue = "Binding Path=Name";

            // Act
            bool result = _helper.IsValidCompactAttributeFormat(attributeValue);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidCompactAttributeFormat_ReturnsFalseForOnlyOpeningBrace()
        {
            // Arrange
            string attributeValue = "{";

            // Act
            bool result = _helper.IsValidCompactAttributeFormat(attributeValue);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidCompactAttributeFormat_ReturnsFalseForOnlyClosingBrace()
        {
            // Arrange
            string attributeValue = "}";

            // Act
            bool result = _helper.IsValidCompactAttributeFormat(attributeValue);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidCompactAttributeFormat_ReturnsTrueForNestedBraces()
        {
            // Arrange
            string attributeValue = "{Binding {StaticResource Key}}";

            // Act
            bool result = _helper.IsValidCompactAttributeFormat(attributeValue);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region LookupProperty Tests

        [Fact]
        public void LookupProperty_ReturnsPropertyWhenFound()
        {
            // Arrange
            var properties = typeof(TestClass).GetProperties().ToList();
            string propertyName = "Name";

            // Act
            PropertyInfo result = _helper.LookupProperty(properties, propertyName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Name", result.Name);
        }

        [Fact]
        public void LookupProperty_ReturnsNullWhenPropertyNotFound()
        {
            // Arrange
            var properties = typeof(TestClass).GetProperties().ToList();
            string propertyName = "NonExistentProperty";

            // Act
            PropertyInfo result = _helper.LookupProperty(properties, propertyName);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void LookupProperty_ReturnsNullWhenPropertiesIsNull()
        {
            // Arrange
            IList<PropertyInfo>? properties = null;
            string propertyName = "Name";

            // Act
            PropertyInfo result = _helper.LookupProperty(properties, propertyName);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void LookupProperty_ReturnsNullWhenPropertyNameIsNull()
        {
            // Arrange
            var properties = typeof(TestClass).GetProperties().ToList();
            string? propertyName = null;

            // Act
            PropertyInfo result = _helper.LookupProperty(properties, propertyName);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void LookupProperty_ReturnsNullWhenPropertyNameIsEmpty()
        {
            // Arrange
            var properties = typeof(TestClass).GetProperties().ToList();
            string propertyName = "";

            // Act
            PropertyInfo result = _helper.LookupProperty(properties, propertyName);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void LookupProperty_ReturnsFirstMatchingProperty()
        {
            // Arrange
            var properties = typeof(TestClass).GetProperties().ToList();
            string propertyName = "Value";

            // Act
            PropertyInfo result = _helper.LookupProperty(properties, propertyName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Value", result.Name);
            Assert.Equal(typeof(int), result.PropertyType);
        }

        [Fact]
        public void LookupProperty_IsCaseSensitive()
        {
            // Arrange
            var properties = typeof(TestClass).GetProperties().ToList();
            string propertyName = "name"; // lowercase

            // Act
            PropertyInfo result = _helper.LookupProperty(properties, propertyName);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region RemoveEscapes Tests

        [Fact]
        public void RemoveEscapes_RemovesBackslashEscapes()
        {
            // Arrange
            string value = @"test\,value";

            // Act
            _helper.RemoveEscapes(ref value);

            // Assert
            Assert.Equal("test,value", value);
        }

        [Fact]
        public void RemoveEscapes_RemovesMultipleEscapes()
        {
            // Arrange
            string value = @"test\,value\=data";

            // Act
            _helper.RemoveEscapes(ref value);

            // Assert
            Assert.Equal("test,value=data", value);
        }

        [Fact]
        public void RemoveEscapes_DoesNotModifyStringWithoutEscapes()
        {
            // Arrange
            string value = "testvalue";

            // Act
            _helper.RemoveEscapes(ref value);

            // Assert
            Assert.Equal("testvalue", value);
        }

        [Fact]
        public void RemoveEscapes_HandlesEmptyString()
        {
            // Arrange
            string value = "";

            // Act
            _helper.RemoveEscapes(ref value);

            // Assert
            Assert.Equal("", value);
        }

        [Fact]
        public void RemoveEscapes_HandlesStringWithOnlyBackslash()
        {
            // Arrange
            string value = @"\";

            // Act
            _helper.RemoveEscapes(ref value);

            // Assert
            Assert.Equal("", value);
        }

        [Fact]
        public void RemoveEscapes_HandlesBackslashAtEnd()
        {
            // Arrange
            string value = @"test\";

            // Act
            _helper.RemoveEscapes(ref value);

            // Assert
            Assert.Equal("test", value);
        }

        [Fact]
        public void RemoveEscapes_HandlesBackslashAtBeginning()
        {
            // Arrange
            string value = @"\test";

            // Act
            _helper.RemoveEscapes(ref value);

            // Assert
            Assert.Equal("test", value);
        }

        [Fact]
        public void RemoveEscapes_HandlesConsecutiveBackslashes()
        {
            // Arrange
            string value = @"test\\value";

            // Act
            _helper.RemoveEscapes(ref value);

            // Assert
            Assert.Equal(@"test\value", value);
        }

        [Fact]
        public void RemoveEscapes_HandlesMultipleConsecutiveBackslashes()
        {
            // Arrange
            string value = @"test\\\\value";

            // Act
            _helper.RemoveEscapes(ref value);

            // Assert
            Assert.Equal(@"test\\value", value);
        }

        [Fact]
        public void RemoveEscapes_PreservesCharactersAfterBackslash()
        {
            // Arrange
            string value = @"\a\b\c";

            // Act
            _helper.RemoveEscapes(ref value);

            // Assert
            Assert.Equal("abc", value);
        }

        [Fact]
        public void RemoveEscapes_HandlesComplexEscapePattern()
        {
            // Arrange
            string value = @"key\=value\,another\=pair";

            // Act
            _helper.RemoveEscapes(ref value);

            // Assert
            Assert.Equal("key=value,another=pair", value);
        }

        #endregion

        #region IsMarkupExtension Tests

        [Fact]
        public void IsMarkupExtension_ReturnsTrueForArrayType()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("Array", "http://schemas.microsoft.com/winfx/2006/xaml");

            // Act
            bool result = _helper.IsMarkupExtension(xmlQualifiedName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsMarkupExtension_ReturnsTrueForNullString()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("Null", "http://schemas.microsoft.com/winfx/2006/xaml");

            // Act
            bool result = _helper.IsMarkupExtension(xmlQualifiedName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsMarkupExtension_ReturnsTrueForNullExtensionType()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("NullExtension", "http://schemas.microsoft.com/winfx/2006/xaml");

            // Act
            bool result = _helper.IsMarkupExtension(xmlQualifiedName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsMarkupExtension_ReturnsTrueForTypeString()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("Type", "http://schemas.microsoft.com/winfx/2006/xaml");

            // Act
            bool result = _helper.IsMarkupExtension(xmlQualifiedName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsMarkupExtension_ReturnsTrueForTypeExtensionType()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("TypeExtension", "http://schemas.microsoft.com/winfx/2006/xaml");

            // Act
            bool result = _helper.IsMarkupExtension(xmlQualifiedName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsMarkupExtension_ReturnsFalseForInvalidNamespace()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("Array", "http://schemas.microsoft.com/winfx/2006/xaml/workflow");

            // Act
            bool result = _helper.IsMarkupExtension(xmlQualifiedName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsMarkupExtension_ReturnsFalseForInvalidName()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("InvalidType", "http://schemas.microsoft.com/winfx/2006/xaml");

            // Act
            bool result = _helper.IsMarkupExtension(xmlQualifiedName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsMarkupExtension_ReturnsFalseForEmptyName()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("", "http://schemas.microsoft.com/winfx/2006/xaml");

            // Act
            bool result = _helper.IsMarkupExtension(xmlQualifiedName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsMarkupExtension_ReturnsFalseForCorrectNameButEmptyNamespace()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("Array", "");

            // Act
            bool result = _helper.IsMarkupExtension(xmlQualifiedName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsMarkupExtension_ReturnsFalseForCaseVariationInNamespace()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("Array", "http://schemas.microsoft.com/winfx/2006/XAML");

            // Act
            bool result = _helper.IsMarkupExtension(xmlQualifiedName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsMarkupExtension_ReturnsFalseForCaseVariationInName()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("array", "http://schemas.microsoft.com/winfx/2006/xaml");

            // Act
            bool result = _helper.IsMarkupExtension(xmlQualifiedName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsMarkupExtension_ReturnsFalseForNullAsLowercase()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("null", "http://schemas.microsoft.com/winfx/2006/xaml");

            // Act
            bool result = _helper.IsMarkupExtension(xmlQualifiedName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsMarkupExtension_ReturnsFalseForCustomType()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("CustomExtension", "http://schemas.microsoft.com/winfx/2006/xaml");

            // Act
            bool result = _helper.IsMarkupExtension(xmlQualifiedName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsMarkupExtension_ReturnsFalseForDifferentNamespaceScheme()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("Array", "http://www.w3.org/2001/XMLSchema");

            // Act
            bool result = _helper.IsMarkupExtension(xmlQualifiedName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsMarkupExtension_ReturnsFalseForPartialNamespaceMatch()
        {
            // Arrange
            var xmlQualifiedName = new XmlQualifiedName("Array", "http://schemas.microsoft.com/winfx/2006/xaml/");

            // Act
            bool result = _helper.IsMarkupExtension(xmlQualifiedName);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region CreateInstance Tests

        [Fact]
        public void CreateInstance_CreatesInstanceOfSimpleClass()
        {
            // Arrange
            Type type = typeof(TestClass);

            // Act
            object result = _helper.CreateInstance(type);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TestClass>(result);
        }

        [Fact]
        public void CreateInstance_CreatesInstanceOfList()
        {
            // Arrange
            Type type = typeof(List<int>);

            // Act
            object result = _helper.CreateInstance(type);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<int>>(result);
        }

        [Fact]
        public void CreateInstance_CreatesInstanceOfDictionary()
        {
            // Arrange
            Type type = typeof(Dictionary<string, int>);

            // Act
            object result = _helper.CreateInstance(type);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Dictionary<string, int>>(result);
        }

        [Fact]
        public void CreateInstance_CreatesInstanceOfValueType()
        {
            // Arrange
            Type type = typeof(int);

            // Act
            object result = _helper.CreateInstance(type);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<int>(result);
            Assert.Equal(0, result);
        }

        [Fact]
        public void CreateInstance_CreatesInstanceOfDateTime()
        {
            // Arrange
            Type type = typeof(DateTime);

            // Act
            object result = _helper.CreateInstance(type);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<DateTime>(result);
        }

        [Fact]
        public void CreateInstance_CreatesInstanceOfGuid()
        {
            // Arrange
            Type type = typeof(Guid);

            // Act
            object result = _helper.CreateInstance(type);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Guid>(result);
        }

        [Fact]
        public void CreateInstance_ThrowsArgumentNullExceptionWhenTypeIsNull()
        {
            // Arrange
            Type? type = null;

            // Act & Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => _helper.CreateInstance(type));
            Assert.Equal("type", exception.ParamName);
        }

        [Fact]
        public void CreateInstance_ThrowsExceptionForAbstractClass()
        {
            // Arrange
            Type type = typeof(AbstractTestClass);

            // Act & Assert
            Assert.Throws<MissingMethodException>(() => _helper.CreateInstance(type));
        }

        [Fact]
        public void CreateInstance_ThrowsExceptionForInterface()
        {
            // Arrange
            Type type = typeof(ITestInterface);

            // Act & Assert
            Assert.Throws<MissingMethodException>(() => _helper.CreateInstance(type));
        }

        [Fact]
        public void CreateInstance_ThrowsExceptionForClassWithoutParameterlessConstructor()
        {
            // Arrange
            Type type = typeof(ClassWithoutParameterlessConstructor);

            // Act & Assert
            Assert.Throws<MissingMethodException>(() => _helper.CreateInstance(type));
        }

        [Fact]
        public void CreateInstance_CreatesInstanceOfNestedClass()
        {
            // Arrange
            Type type = typeof(TestClass.NestedClass);

            // Act
            object result = _helper.CreateInstance(type);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TestClass.NestedClass>(result);
        }

        [Fact]
        public void CreateInstance_CreatesInstanceOfGenericClass()
        {
            // Arrange
            Type type = typeof(GenericTestClass<string>);

            // Act
            object result = _helper.CreateInstance(type);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<GenericTestClass<string>>(result);
        }

        #endregion

        #region Test Helper Classes

        private class TestClass
        {
            public string Name { get; set; } = "";
            public int Value { get; set; } // NOSONAR
            public DateTime Date { get; set; } // NOSONAR

            public class NestedClass
            {
                public int Id { get; set; } // NOSONAR
            }
        }

        private abstract class AbstractTestClass
        {
            public abstract void DoSomething();
        }

        private interface ITestInterface
        {
            void DoSomething();
        }

        private class ClassWithoutParameterlessConstructor
        {
            public ClassWithoutParameterlessConstructor(string name)
            {
                Name = name;
            }

            public string Name { get; set; }
        }

        private class GenericTestClass<T>
        {
            public T? Value { get; set; }
        }

        #endregion
    }
}