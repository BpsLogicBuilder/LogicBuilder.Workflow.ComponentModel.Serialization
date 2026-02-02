using LogicBuilder.ComponentModel.Design.Serialization;
using LogicBuilder.Workflow.ComponentModel.Design;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using System;
using System.Drawing;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Design
{
    public class ColorMarkupSerializerTest
    {
        private readonly ColorMarkupSerializer _serializer;
        private readonly WorkflowMarkupSerializationManager _serializationManager;

        public ColorMarkupSerializerTest()
        {
            _serializer = new ColorMarkupSerializer();
            _serializationManager = new WorkflowMarkupSerializationManager(new DesignerSerializationManager());
        }

        #region CanSerializeToString Tests

        [Fact]
        public void CanSerializeToString_ReturnsTrue_WhenValueIsColor()
        {
            // Arrange
            var color = Color.Red;

            // Act
            bool result = _serializer.CanSerializeToString(_serializationManager, color);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanSerializeToString_ReturnsFalse_WhenValueIsString()
        {
            // Arrange
            var value = "not a color";

            // Act
            bool result = _serializer.CanSerializeToString(_serializationManager, value);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanSerializeToString_ReturnsFalse_WhenValueIsInt()
        {
            // Arrange
            var value = 42;

            // Act
            bool result = _serializer.CanSerializeToString(_serializationManager, value);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanSerializeToString_ReturnsFalse_WhenValueIsNull()
        {
            // Act
            bool result = _serializer.CanSerializeToString(_serializationManager, null);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region SerializeToString Tests

        [Fact]
        public void SerializeToString_ThrowsArgumentNullException_WhenSerializationManagerIsNull()
        {
            // Arrange
            var color = Color.Red;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _serializer.SerializeToString(null, color));
        }

        [Fact]
        public void SerializeToString_ThrowsArgumentNullException_WhenValueIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _serializer.SerializeToString(_serializationManager, null));
        }

        [Fact]
        public void SerializeToString_ReturnsCorrectHexString_ForRedColor()
        {
            // Arrange
            var color = Color.FromArgb(255, 255, 0, 0); // Red with full alpha

            // Act
            string result = _serializer.SerializeToString(_serializationManager, color);

            // Assert
            Assert.Equal("0XFFFF0000", result);
        }

        [Fact]
        public void SerializeToString_ReturnsCorrectHexString_ForGreenColor()
        {
            // Arrange
            var color = Color.FromArgb(255, 0, 255, 0); // Green with full alpha

            // Act
            string result = _serializer.SerializeToString(_serializationManager, color);

            // Assert
            Assert.Equal("0XFF00FF00", result);
        }

        [Fact]
        public void SerializeToString_ReturnsCorrectHexString_ForBlueColor()
        {
            // Arrange
            var color = Color.FromArgb(255, 0, 0, 255); // Blue with full alpha

            // Act
            string result = _serializer.SerializeToString(_serializationManager, color);

            // Assert
            Assert.Equal("0XFF0000FF", result);
        }

        [Fact]
        public void SerializeToString_ReturnsCorrectHexString_ForWhiteColor()
        {
            // Arrange
            var color = Color.White; // ARGB: 255, 255, 255, 255

            // Act
            string result = _serializer.SerializeToString(_serializationManager, color);

            // Assert
            Assert.Equal("0XFFFFFFFF", result);
        }

        [Fact]
        public void SerializeToString_ReturnsCorrectHexString_ForBlackColor()
        {
            // Arrange
            var color = Color.Black; // ARGB: 255, 0, 0, 0

            // Act
            string result = _serializer.SerializeToString(_serializationManager, color);

            // Assert
            Assert.Equal("0XFF000000", result);
        }

        [Fact]
        public void SerializeToString_ReturnsCorrectHexString_ForTransparentColor()
        {
            // Arrange
            var color = Color.FromArgb(0, 0, 0, 0); // Fully transparent

            // Act
            string result = _serializer.SerializeToString(_serializationManager, color);

            // Assert
            Assert.Equal("0X00000000", result);
        }

        [Fact]
        public void SerializeToString_ReturnsCorrectHexString_ForSemiTransparentColor()
        {
            // Arrange
            var color = Color.FromArgb(128, 100, 150, 200); // Semi-transparent

            // Act
            string result = _serializer.SerializeToString(_serializationManager, color);

            // Assert
            Assert.Equal("0X806496C8", result);
        }

        [Fact]
        public void SerializeToString_ReturnsCorrectHexString_ForCustomColor()
        {
            // Arrange
            var color = Color.FromArgb(10, 20, 30, 40);

            // Act
            string result = _serializer.SerializeToString(_serializationManager, color);

            // Assert
            Assert.Equal("0X0A141E28", result);
        }

        [Fact]
        public void SerializeToString_ReturnsEmptyString_WhenValueIsNotColor()
        {
            // Arrange
            var value = "not a color";

            // Act
            string result = _serializer.SerializeToString(_serializationManager, value);

            // Assert
            Assert.Equal(String.Empty, result);
        }

        #endregion

        #region DeserializeFromString Tests

        [Fact]
        public void DeserializeFromString_ReturnsColor_WhenValueIsValidHexString()
        {
            // Arrange
            string hexValue = "0XFF0000FF"; // Red with full alpha

            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(Color), hexValue);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Color>(result);
            var color = (Color)result;
            Assert.Equal(255, color.A);
            Assert.Equal(0, color.R);
            Assert.Equal(0, color.G);
            Assert.Equal(255, color.B);
        }

        [Fact]
        public void DeserializeFromString_ReturnsColor_WhenValueIsLowercaseHexString()
        {
            // Arrange
            string hexValue = "0x00ff00ff"; // Green with full alpha (lowercase)

            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(Color), hexValue);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Color>(result);
            var color = (Color)result;
            Assert.Equal(0, color.A);
            Assert.Equal(255, color.R);
            Assert.Equal(0, color.G);
            Assert.Equal(255, color.B);
        }

        [Fact]
        public void DeserializeFromString_ReturnsWhiteColor_WhenValueIsAllFs()
        {
            // Arrange
            string hexValue = "0XFFFFFFFF";

            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(Color), hexValue);

            // Assert
            Assert.NotNull(result);
            var color = (Color)result;
            Assert.Equal(255, color.A);
            Assert.Equal(255, color.R);
            Assert.Equal(255, color.G);
            Assert.Equal(255, color.B);
        }

        [Fact]
        public void DeserializeFromString_ReturnsBlackColor_WhenValueIsAllZeros()
        {
            // Arrange
            string hexValue = "0X00000000";

            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(Color), hexValue);

            // Assert
            Assert.NotNull(result);
            var color = (Color)result;
            Assert.Equal(0, color.A);
            Assert.Equal(0, color.R);
            Assert.Equal(0, color.G);
            Assert.Equal(0, color.B);
        }

        [Fact]
        public void DeserializeFromString_ReturnsTransparentRedColor_WhenValueHasZeroAlpha()
        {
            // Arrange
            string hexValue = "0X0000FF00"; // Red with zero alpha

            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(Color), hexValue);

            // Assert
            Assert.NotNull(result);
            var color = (Color)result;
            Assert.Equal(0, color.A);
            Assert.Equal(0, color.R);
            Assert.Equal(255, color.G);
            Assert.Equal(0, color.B);
        }

        [Fact]
        public void DeserializeFromString_ReturnsSemiTransparentColor_WhenValueHasPartialAlpha()
        {
            // Arrange
            string hexValue = "0X6496C880"; // Semi-transparent custom color

            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(Color), hexValue);

            // Assert
            Assert.NotNull(result);
            var color = (Color)result;
            Assert.Equal(100, color.A);
            Assert.Equal(150, color.R);
            Assert.Equal(200, color.G);
            Assert.Equal(128, color.B);
        }

        [Fact]
        public void DeserializeFromString_ReturnsNull_WhenValueIsEmpty()
        {
            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(Color), string.Empty);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void DeserializeFromString_ReturnsNull_WhenValueIsNull()
        {
            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(Color), null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void DeserializeFromString_ReturnsNull_WhenPropertyTypeIsNotColor()
        {
            // Arrange
            string hexValue = "0XFF0000FF";

            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(string), hexValue);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region Round-Trip Tests

        [Fact]
        public void RoundTrip_PreservesColor_ForRedColor()
        {
            // Arrange
            var originalColor = Color.FromArgb(255, 255, 0, 0);

            // Act
            string serialized = _serializer.SerializeToString(_serializationManager, originalColor);
            object deserialized = _serializer.DeserializeFromString(_serializationManager, typeof(Color), serialized);

            // Assert
            Assert.IsType<Color>(deserialized);
            var resultColor = (Color)deserialized;
            Assert.Equal(originalColor.A, resultColor.A);
            Assert.Equal(originalColor.R, resultColor.R);
            Assert.Equal(originalColor.G, resultColor.G);
            Assert.Equal(originalColor.B, resultColor.B);
        }

        [Fact]
        public void RoundTrip_PreservesColor_ForTransparentColor()
        {
            // Arrange
            var originalColor = Color.FromArgb(0, 128, 64, 192);

            // Act
            string serialized = _serializer.SerializeToString(_serializationManager, originalColor);
            object deserialized = _serializer.DeserializeFromString(_serializationManager, typeof(Color), serialized);

            // Assert
            Assert.IsType<Color>(deserialized);
            var resultColor = (Color)deserialized;
            Assert.Equal(originalColor.A, resultColor.A);
            Assert.Equal(originalColor.R, resultColor.R);
            Assert.Equal(originalColor.G, resultColor.G);
            Assert.Equal(originalColor.B, resultColor.B);
        }

        #endregion
    }
}