using LogicBuilder.Workflow.ComponentModel.Compiler;
using System;
using System.Collections;
using System.Globalization;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Compiler
{
    public class ValidationErrorTest
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_TwoParameters_CreatesValidationError()
        {
            // Arrange
            string errorText = "Test error message";
            int errorNumber = 100;

            // Act
            var error = new ValidationError(errorText, errorNumber);

            // Assert
            Assert.Equal(errorText, error.ErrorText);
            Assert.Equal(errorNumber, error.ErrorNumber);
            Assert.False(error.IsWarning);
            Assert.Null(error.PropertyName);
        }

        [Fact]
        public void Constructor_ThreeParameters_CreatesValidationError()
        {
            // Arrange
            string errorText = "Test error message";
            int errorNumber = 100;
            bool isWarning = true;

            // Act
            var error = new ValidationError(errorText, errorNumber, isWarning);

            // Assert
            Assert.Equal(errorText, error.ErrorText);
            Assert.Equal(errorNumber, error.ErrorNumber);
            Assert.True(error.IsWarning);
            Assert.Null(error.PropertyName);
        }

        [Fact]
        public void Constructor_FourParameters_CreatesValidationError()
        {
            // Arrange
            string errorText = "Test error message";
            int errorNumber = 100;
            bool isWarning = false;
            string propertyName = "TestProperty";

            // Act
            var error = new ValidationError(errorText, errorNumber, isWarning, propertyName);

            // Assert
            Assert.Equal(errorText, error.ErrorText);
            Assert.Equal(errorNumber, error.ErrorNumber);
            Assert.False(error.IsWarning);
            Assert.Equal(propertyName, error.PropertyName);
        }

        [Fact]
        public void Constructor_WithNullErrorText_CreatesValidationError()
        {
            // Arrange
            string? errorText = null;
            int errorNumber = 100;

            // Act
            var error = new ValidationError(errorText, errorNumber);

            // Assert
            Assert.Null(error.ErrorText);
            Assert.Equal(errorNumber, error.ErrorNumber);
        }

        [Fact]
        public void Constructor_WithEmptyErrorText_CreatesValidationError()
        {
            // Arrange
            string errorText = string.Empty;
            int errorNumber = 100;

            // Act
            var error = new ValidationError(errorText, errorNumber);

            // Assert
            Assert.Equal(string.Empty, error.ErrorText);
            Assert.Equal(errorNumber, error.ErrorNumber);
        }

        [Fact]
        public void Constructor_WithNullPropertyName_CreatesValidationError()
        {
            // Arrange
            string errorText = "Test error";
            int errorNumber = 100;
            string? propertyName = null;

            // Act
            var error = new ValidationError(errorText, errorNumber, false, propertyName);

            // Assert
            Assert.Null(error.PropertyName);
        }

        [Fact]
        public void Constructor_WithZeroErrorNumber_CreatesValidationError()
        {
            // Arrange
            string errorText = "Test error";
            int errorNumber = 0;

            // Act
            var error = new ValidationError(errorText, errorNumber);

            // Assert
            Assert.Equal(0, error.ErrorNumber);
        }

        [Fact]
        public void Constructor_WithNegativeErrorNumber_CreatesValidationError()
        {
            // Arrange
            string errorText = "Test error";
            int errorNumber = -1;

            // Act
            var error = new ValidationError(errorText, errorNumber);

            // Assert
            Assert.Equal(-1, error.ErrorNumber);
        }

        #endregion

        #region Property Tests

        [Fact]
        public void ErrorText_ReturnsCorrectValue()
        {
            // Arrange
            string expectedText = "This is an error message";
            var error = new ValidationError(expectedText, 200);

            // Act
            string actualText = error.ErrorText;

            // Assert
            Assert.Equal(expectedText, actualText);
        }

        [Fact]
        public void ErrorNumber_ReturnsCorrectValue()
        {
            // Arrange
            int expectedNumber = 300;
            var error = new ValidationError("Error", expectedNumber);

            // Act
            int actualNumber = error.ErrorNumber;

            // Assert
            Assert.Equal(expectedNumber, actualNumber);
        }

        [Fact]
        public void IsWarning_ReturnsFalse_WhenNotSetAsWarning()
        {
            // Arrange
            var error = new ValidationError("Error", 100, false);

            // Act
            bool isWarning = error.IsWarning;

            // Assert
            Assert.False(isWarning);
        }

        [Fact]
        public void IsWarning_ReturnsTrue_WhenSetAsWarning()
        {
            // Arrange
            var error = new ValidationError("Warning", 100, true);

            // Act
            bool isWarning = error.IsWarning;

            // Assert
            Assert.True(isWarning);
        }

        [Fact]
        public void PropertyName_CanBeSet()
        {
            // Arrange
            var error = new ValidationError("Error", 100);
            string expectedName = "MyProperty";

            // Act
            error.PropertyName = expectedName;

            // Assert
            Assert.Equal(expectedName, error.PropertyName);
        }

        [Fact]
        public void PropertyName_CanBeSetToNull()
        {
            // Arrange
            var error = new ValidationError("Error", 100, false, "InitialProperty")
            {
                // Act
                PropertyName = null
            };

            // Assert
            Assert.Null(error.PropertyName);
        }

        [Fact]
        public void PropertyName_CanBeChanged()
        {
            // Arrange
            var error = new ValidationError("Error", 100, false, "FirstProperty");
            string newName = "SecondProperty";

            // Act
            error.PropertyName = newName;

            // Assert
            Assert.Equal(newName, error.PropertyName);
        }

        #endregion

        #region UserData Tests

        [Fact]
        public void UserData_IsNotNull()
        {
            // Arrange
            var error = new ValidationError("Error", 100);

            // Act
            IDictionary userData = error.UserData;

            // Assert
            Assert.NotNull(userData);
        }

        [Fact]
        public void UserData_IsEmptyInitially()
        {
            // Arrange
            var error = new ValidationError("Error", 100);

            // Act
            IDictionary userData = error.UserData;

            // Assert
            Assert.Empty(userData);
        }

        [Fact]
        public void UserData_CanStoreValues()
        {
            // Arrange
            var error = new ValidationError("Error", 100);
            string key = "TestKey";
            string value = "TestValue";

            // Act
            error.UserData[key] = value;

            // Assert
            Assert.Equal(value, error.UserData[key]);
        }

        [Fact]
        public void UserData_CanStoreMultipleValues()
        {
            // Arrange
            var error = new ValidationError("Error", 100);

            // Act
            error.UserData["Key1"] = "Value1";
            error.UserData["Key2"] = 42;
            error.UserData["Key3"] = true;

            // Assert
            Assert.Equal("Value1", error.UserData["Key1"]);
            Assert.Equal(42, error.UserData["Key2"]);
            Assert.Equal(true, error.UserData["Key3"]);
            Assert.Equal(3, error.UserData.Count);
        }

        [Fact]
        public void UserData_ReturnsSameInstance()
        {
            // Arrange
            var error = new ValidationError("Error", 100);

            // Act
            IDictionary userData1 = error.UserData;
            IDictionary userData2 = error.UserData;

            // Assert
            Assert.Same(userData1, userData2);
        }

        [Fact]
        public void UserData_CanRemoveValues()
        {
            // Arrange
            var error = new ValidationError("Error", 100);
            string key = "TestKey";
            error.UserData[key] = "TestValue";

            // Act
            error.UserData.Remove(key);

            // Assert
            Assert.False(error.UserData.Contains(key));
        }

        [Fact]
        public void UserData_CanClearAllValues()
        {
            // Arrange
            var error = new ValidationError("Error", 100);
            error.UserData["Key1"] = "Value1";
            error.UserData["Key2"] = "Value2";

            // Act
            error.UserData.Clear();

            // Assert
            Assert.Empty(error.UserData);
        }

        #endregion

        #region GetNotSetValidationError Tests

        [Fact]
        public void GetNotSetValidationError_CreatesValidationError()
        {
            // Arrange
            string propertyName = "TestProperty";

            // Act
            ValidationError error = ValidationError.GetNotSetValidationError(propertyName);

            // Assert
            Assert.NotNull(error);
        }

        [Fact]
        public void GetNotSetValidationError_SetsPropertyName()
        {
            // Arrange
            string propertyName = "TestProperty";

            // Act
            ValidationError error = ValidationError.GetNotSetValidationError(propertyName);

            // Assert
            Assert.Equal(propertyName, error.PropertyName);
        }

        [Fact]
        public void GetNotSetValidationError_SetsCorrectErrorNumber()
        {
            // Arrange
            string propertyName = "TestProperty";

            // Act
            ValidationError error = ValidationError.GetNotSetValidationError(propertyName);

            // Assert
            Assert.Equal(0x116, error.ErrorNumber);
        }

        [Fact]
        public void GetNotSetValidationError_IsNotWarning()
        {
            // Arrange
            string propertyName = "TestProperty";

            // Act
            ValidationError error = ValidationError.GetNotSetValidationError(propertyName);

            // Assert
            Assert.False(error.IsWarning);
        }

        [Fact]
        public void GetNotSetValidationError_ErrorTextContainsPropertyName()
        {
            // Arrange
            string propertyName = "TestProperty";

            // Act
            ValidationError error = ValidationError.GetNotSetValidationError(propertyName);

            // Assert
            Assert.NotNull(error.ErrorText);
            Assert.Contains(propertyName, error.ErrorText);
        }

        [Fact]
        public void GetNotSetValidationError_WithNullPropertyName_CreatesValidationError()
        {
            // Arrange
            string? propertyName = null;

            // Act
            ValidationError error = ValidationError.GetNotSetValidationError(propertyName);

            // Assert
            Assert.NotNull(error);
            Assert.Null(error.PropertyName);
        }

        [Fact]
        public void GetNotSetValidationError_WithEmptyPropertyName_CreatesValidationError()
        {
            // Arrange
            string propertyName = string.Empty;

            // Act
            ValidationError error = ValidationError.GetNotSetValidationError(propertyName);

            // Assert
            Assert.NotNull(error);
            Assert.Equal(string.Empty, error.PropertyName);
        }

        #endregion

        #region ToString Tests

        [Fact]
        public void ToString_ReturnsErrorFormat_WhenIsWarningIsFalse()
        {
            // Arrange
            var error = new ValidationError("Test error", 100, false);

            // Act
            string result = error.ToString();

            // Assert
            Assert.Contains("error", result);
            Assert.Contains("100", result);
            Assert.Contains("Test error", result);
        }

        [Fact]
        public void ToString_ReturnsWarningFormat_WhenIsWarningIsTrue()
        {
            // Arrange
            var error = new ValidationError("Test warning", 200, true);

            // Act
            string result = error.ToString();

            // Assert
            Assert.Contains("warning", result);
            Assert.Contains("200", result);
            Assert.Contains("Test warning", result);
        }

        [Fact]
        public void ToString_IncludesErrorNumber()
        {
            // Arrange
            var error = new ValidationError("Error message", 12345);

            // Act
            string result = error.ToString();

            // Assert
            Assert.Contains("12345", result);
        }

        [Fact]
        public void ToString_IncludesErrorText()
        {
            // Arrange
            string errorText = "This is a specific error message";
            var error = new ValidationError(errorText, 100);

            // Act
            string result = error.ToString();

            // Assert
            Assert.Contains(errorText, result);
        }

        [Fact]
        public void ToString_UsesInvariantCulture()
        {
            // Arrange
            var error = new ValidationError("Error", 999);
            var currentCulture = CultureInfo.CurrentCulture;

            try
            {
                // Act - change culture to verify invariant culture is used
                CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
                string result = error.ToString();

                // Assert - the format should not be affected by culture
                Assert.Contains("error 999: Error", result);
            }
            finally
            {
                CultureInfo.CurrentCulture = currentCulture;
            }
        }

        [Fact]
        public void ToString_WithNullErrorText_DoesNotThrow()
        {
            // Arrange
            var error = new ValidationError(null, 100);

            // Act
            Exception exception = Record.Exception(() => error.ToString());

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void ToString_WithEmptyErrorText_ReturnsValidFormat()
        {
            // Arrange
            var error = new ValidationError(string.Empty, 100);

            // Act
            string result = error.ToString();

            // Assert
            Assert.Contains("error", result);
            Assert.Contains("100", result);
        }

        [Theory]
        [InlineData(0, false, "error 0:")]
        [InlineData(100, false, "error 100:")]
        [InlineData(200, true, "warning 200:")]
        [InlineData(-1, false, "error -1:")]
        public void ToString_ReturnsExpectedFormat(int errorNumber, bool isWarning, string expectedPrefix)
        {
            // Arrange
            var error = new ValidationError("Message", errorNumber, isWarning);

            // Act
            string result = error.ToString();

            // Assert
            Assert.StartsWith(expectedPrefix, result);
        }

        #endregion

        #region Serialization Tests

        [Fact]
        public void ValidationError_HasSerializableAttribute()
        {
            // Arrange
            var type = typeof(ValidationError);

            // Act
            var attributes = type.GetCustomAttributes(typeof(SerializableAttribute), false);

            // Assert
            Assert.NotEmpty(attributes);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void ValidationError_CanBeUsedInCollection()
        {
            // Arrange
            var errors = new System.Collections.Generic.List<ValidationError>();
            var error1 = new ValidationError("Error 1", 1);
            var error2 = new ValidationError("Error 2", 2);

            // Act
            errors.Add(error1);
            errors.Add(error2);

            // Assert
            Assert.Equal(2, errors.Count);
            Assert.Contains(error1, errors);
            Assert.Contains(error2, errors);
        }

        [Fact]
        public void ValidationError_PropertiesAreIndependent()
        {
            // Arrange
            var error1 = new ValidationError("Error 1", 1, false, "Property1");
            var error2 = new ValidationError("Error 2", 2, true, "Property2");

            // Act
            error1.UserData["key"] = "value1";
            error2.UserData["key"] = "value2";

            // Assert
            Assert.NotEqual(error1.ErrorText, error2.ErrorText);
            Assert.NotEqual(error1.ErrorNumber, error2.ErrorNumber);
            Assert.NotEqual(error1.IsWarning, error2.IsWarning);
            Assert.NotEqual(error1.PropertyName, error2.PropertyName);
            Assert.NotEqual(error1.UserData["key"], error2.UserData["key"]);
        }

        [Fact]
        public void ValidationError_WithMultilineErrorText_HandlesCorrectly()
        {
            // Arrange
            string multilineText = "Line 1\nLine 2\nLine 3";
            var error = new ValidationError(multilineText, 100);

            // Act
            string errorText = error.ErrorText;
            string toString = error.ToString();

            // Assert
            Assert.Equal(multilineText, errorText);
            Assert.Contains(multilineText, toString);
        }

        [Fact]
        public void ValidationError_WithSpecialCharacters_HandlesCorrectly()
        {
            // Arrange
            string specialText = "Error with special chars: <>&\"'";
            var error = new ValidationError(specialText, 100);

            // Act
            string result = error.ToString();

            // Assert
            Assert.Contains(specialText, result);
        }

        [Fact]
        public void GetNotSetValidationError_CreatesDifferentInstances()
        {
            // Arrange & Act
            ValidationError error1 = ValidationError.GetNotSetValidationError("Property1");
            ValidationError error2 = ValidationError.GetNotSetValidationError("Property2");

            // Assert
            Assert.NotSame(error1, error2);
            Assert.NotEqual(error1.PropertyName, error2.PropertyName);
        }

        #endregion
    }
}