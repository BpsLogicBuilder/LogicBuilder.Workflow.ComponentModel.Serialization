using System;

namespace LogicBuilder.Workflow.Tests
{
    public class UtilityTest
    {
        #region CreateGuid Tests

        [Fact]
        public void CreateGuid_ReturnsValidGuid_WhenGuidStringIsValid()
        {
            // Arrange
            string guidString = "12345678-1234-1234-1234-123456789abc";
            Guid expected = new(guidString);

            // Act
            Guid result = Utility.CreateGuid(guidString);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CreateGuid_ReturnsValidGuid_WhenGuidStringHasNoHyphens()
        {
            // Arrange
            string guidString = "12345678123412341234123456789abc";
            Guid expected = new(guidString);

            // Act
            Guid result = Utility.CreateGuid(guidString);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CreateGuid_ReturnsValidGuid_WhenGuidStringHasBraces()
        {
            // Arrange
            string guidString = "{12345678-1234-1234-1234-123456789abc}";
            Guid expected = new(guidString);

            // Act
            Guid result = Utility.CreateGuid(guidString);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CreateGuid_ReturnsValidGuid_WhenGuidStringHasParentheses()
        {
            // Arrange
            string guidString = "(12345678-1234-1234-1234-123456789abc)";
            Guid expected = new(guidString);

            // Act
            Guid result = Utility.CreateGuid(guidString);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CreateGuid_ThrowsArgumentNullException_WhenGuidStringIsNull()
        {
            // Arrange
            string guidString = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => Utility.CreateGuid(guidString));
        }

        [Fact]
        public void CreateGuid_ThrowsFormatException_WhenGuidStringIsInvalid()
        {
            // Arrange
            string guidString = "invalid-guid-string";

            // Act & Assert
            Assert.Throws<FormatException>(() => Utility.CreateGuid(guidString));
        }

        [Fact]
        public void CreateGuid_ThrowsFormatException_WhenGuidStringIsEmpty()
        {
            // Arrange
            string guidString = string.Empty;

            // Act & Assert
            Assert.Throws<FormatException>(() => Utility.CreateGuid(guidString));
        }

        [Fact]
        public void CreateGuid_ThrowsFormatException_WhenGuidStringIsTooShort()
        {
            // Arrange
            string guidString = "12345678";

            // Act & Assert
            Assert.Throws<FormatException>(() => Utility.CreateGuid(guidString));
        }

        #endregion

        #region TryCreateGuid Tests

        [Fact]
        public void TryCreateGuid_ReturnsTrueAndValidGuid_WhenGuidStringIsValid()
        {
            // Arrange
            string guidString = "12345678-1234-1234-1234-123456789abc";
            Guid expected = new(guidString);

            // Act
            bool success = Utility.TryCreateGuid(guidString, out Guid result);

            // Assert
            Assert.True(success);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TryCreateGuid_ReturnsTrueAndValidGuid_WhenGuidStringHasNoHyphens()
        {
            // Arrange
            string guidString = "12345678123412341234123456789abc";
            Guid expected = new(guidString);

            // Act
            bool success = Utility.TryCreateGuid(guidString, out Guid result);

            // Assert
            Assert.True(success);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TryCreateGuid_ReturnsTrueAndValidGuid_WhenGuidStringHasBraces()
        {
            // Arrange
            string guidString = "{12345678-1234-1234-1234-123456789abc}";
            Guid expected = new(guidString);

            // Act
            bool success = Utility.TryCreateGuid(guidString, out Guid result);

            // Assert
            Assert.True(success);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TryCreateGuid_ReturnsTrueAndValidGuid_WhenGuidStringHasParentheses()
        {
            // Arrange
            string guidString = "(12345678-1234-1234-1234-123456789abc)";
            Guid expected = new(guidString);

            // Act
            bool success = Utility.TryCreateGuid(guidString, out Guid result);

            // Assert
            Assert.True(success);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TryCreateGuid_ReturnsFalseAndEmptyGuid_WhenGuidStringIsNull()
        {
            // Arrange
            string guidString = null!;

            // Act
            bool success = Utility.TryCreateGuid(guidString, out Guid result);

            // Assert
            Assert.False(success);
            Assert.Equal(Guid.Empty, result);
        }

        [Fact]
        public void TryCreateGuid_ReturnsFalseAndEmptyGuid_WhenGuidStringIsInvalid()
        {
            // Arrange
            string guidString = "invalid-guid-string";

            // Act
            bool success = Utility.TryCreateGuid(guidString, out Guid result);

            // Assert
            Assert.False(success);
            Assert.Equal(Guid.Empty, result);
        }

        [Fact]
        public void TryCreateGuid_ReturnsFalseAndEmptyGuid_WhenGuidStringIsEmpty()
        {
            // Arrange
            string guidString = string.Empty;

            // Act
            bool success = Utility.TryCreateGuid(guidString, out Guid result);

            // Assert
            Assert.False(success);
            Assert.Equal(Guid.Empty, result);
        }

        [Fact]
        public void TryCreateGuid_ReturnsFalseAndEmptyGuid_WhenGuidStringIsTooShort()
        {
            // Arrange
            string guidString = "12345678";

            // Act
            bool success = Utility.TryCreateGuid(guidString, out Guid result);

            // Assert
            Assert.False(success);
            Assert.Equal(Guid.Empty, result);
        }

        [Fact]
        public void TryCreateGuid_ReturnsFalseAndEmptyGuid_WhenGuidStringHasInvalidCharacters()
        {
            // Arrange
            string guidString = "zzzzzzzz-1234-1234-1234-123456789abc";

            // Act
            bool success = Utility.TryCreateGuid(guidString, out Guid result);

            // Assert
            Assert.False(success);
            Assert.Equal(Guid.Empty, result);
        }

        [Fact]
        public void TryCreateGuid_ReturnsTrueAndValidGuid_WhenGuidStringHasUppercaseLetters()
        {
            // Arrange
            string guidString = "12345678-1234-1234-1234-123456789ABC";
            Guid expected = new(guidString);

            // Act
            bool success = Utility.TryCreateGuid(guidString, out Guid result);

            // Assert
            Assert.True(success);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TryCreateGuid_ReturnsTrueAndValidGuid_WhenGuidStringHasMixedCase()
        {
            // Arrange
            string guidString = "12345678-1234-1234-1234-123456789AbC";
            Guid expected = new(guidString);

            // Act
            bool success = Utility.TryCreateGuid(guidString, out Guid result);

            // Assert
            Assert.True(success);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TryCreateGuid_ReturnsFalseAndEmptyGuid_WhenGuidStringHasTooManyCharacters()
        {
            // Arrange
            string guidString = "12345678-1234-1234-1234-123456789abc-extra";

            // Act
            bool success = Utility.TryCreateGuid(guidString, out Guid result);

            // Assert
            Assert.False(success);
            Assert.Equal(Guid.Empty, result);
        }

        #endregion
    }
}