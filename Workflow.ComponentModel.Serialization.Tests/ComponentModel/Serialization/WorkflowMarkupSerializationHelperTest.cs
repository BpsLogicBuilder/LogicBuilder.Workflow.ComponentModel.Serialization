using LogicBuilder.Workflow.ComponentModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        #region Test Helper Classes

        private class TestClass
        {
            public string Name { get; set; } = "";
            public int Value { get; set; } // NOSONAR
            public DateTime Date { get; set; } // NOSONAR
        }

        #endregion
    }
}