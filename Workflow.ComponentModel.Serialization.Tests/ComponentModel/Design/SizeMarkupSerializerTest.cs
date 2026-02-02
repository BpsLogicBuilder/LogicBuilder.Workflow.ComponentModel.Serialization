using LogicBuilder.ComponentModel.Design.Serialization;
using LogicBuilder.Workflow.ComponentModel.Design;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Design
{
    public class SizeMarkupSerializerTest
    {
        private readonly SizeMarkupSerializer _serializer;
        private readonly WorkflowMarkupSerializationManager _serializationManager;

        public SizeMarkupSerializerTest()
        {
            _serializer = new SizeMarkupSerializer();
            _serializationManager = new WorkflowMarkupSerializationManager(new DesignerSerializationManager());
        }

        #region CanSerializeToString Tests

        [Fact]
        public void CanSerializeToString_ReturnsTrue_WhenValueIsSize()
        {
            // Arrange
            var size = new Size(10, 20);

            // Act
            bool result = _serializer.CanSerializeToString(_serializationManager, size);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanSerializeToString_ReturnsTrue_WhenValueIsSizeEmpty()
        {
            // Arrange
            var size = Size.Empty;

            // Act
            bool result = _serializer.CanSerializeToString(_serializationManager, size);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanSerializeToString_ReturnsFalse_WhenValueIsString()
        {
            // Arrange
            var value = "10, 20";

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

        [Fact]
        public void CanSerializeToString_ReturnsFalse_WhenValueIsPoint()
        {
            // Arrange
            var point = new Point(10, 20);

            // Act
            bool result = _serializer.CanSerializeToString(_serializationManager, point);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region GetProperties Tests

        [Fact]
        public void GetProperties_ReturnsWidthAndHeightProperties_WhenObjectIsSize()
        {
            // Arrange
            var size = new Size(10, 20);

            // Act
            PropertyInfo[] properties = _serializer.GetProperties(_serializationManager, size);

            // Assert
            Assert.NotNull(properties);
            Assert.Equal(2, properties.Length);
            Assert.Contains(properties, p => p.Name == "Width");
            Assert.Contains(properties, p => p.Name == "Height");
        }

        [Fact]
        public void GetProperties_ReturnsWidthAndHeightProperties_WhenObjectIsSizeEmpty()
        {
            // Arrange
            var size = Size.Empty;

            // Act
            PropertyInfo[] properties = _serializer.GetProperties(_serializationManager, size);

            // Assert
            Assert.NotNull(properties);
            Assert.Equal(2, properties.Length);
            Assert.Contains(properties, p => p.Name == "Width");
            Assert.Contains(properties, p => p.Name == "Height");
        }

        [Fact]
        public void GetProperties_ReturnsEmptyArray_WhenObjectIsNotSize()
        {
            // Arrange
            var notSize = "not a size";

            // Act
            PropertyInfo[] properties = _serializer.GetProperties(_serializationManager, notSize);

            // Assert
            Assert.NotNull(properties);
            Assert.Empty(properties);
        }

        [Fact]
        public void GetProperties_ReturnsEmptyArray_WhenObjectIsNull()
        {
            // Act
            PropertyInfo[] properties = _serializer.GetProperties(_serializationManager, null);

            // Assert
            Assert.NotNull(properties);
            Assert.Empty(properties);
        }

        [Fact]
        public void GetProperties_WidthPropertyReturnsCorrectValue()
        {
            // Arrange
            var size = new Size(15, 25);
            PropertyInfo[] properties = _serializer.GetProperties(_serializationManager, size);
            PropertyInfo widthProperty = properties.First(p => p.Name == "Width");

            // Act
            var widthValue = widthProperty.GetValue(size);

            // Assert
            Assert.Equal(15, widthValue);
        }

        [Fact]
        public void GetProperties_HeightPropertyReturnsCorrectValue()
        {
            // Arrange
            var size = new Size(15, 25);
            PropertyInfo[] properties = _serializer.GetProperties(_serializationManager, size);
            PropertyInfo heightProperty = properties.First(p => p.Name == "Height");

            // Act
            var heightValue = heightProperty.GetValue(size);

            // Assert
            Assert.Equal(25, heightValue);
        }

        #endregion

        #region SerializeToString Tests

        [Fact]
        public void SerializeToString_ReturnsCorrectString_ForSizeWithPositiveValues()
        {
            // Arrange
            var size = new Size(10, 20);

            // Act
            string result = _serializer.SerializeToString(_serializationManager, size);

            // Assert
            Assert.Equal("10, 20", result);
        }

        [Fact]
        public void SerializeToString_ReturnsCorrectString_ForSizeEmpty()
        {
            // Arrange
            var size = Size.Empty;

            // Act
            string result = _serializer.SerializeToString(_serializationManager, size);

            // Assert
            Assert.Equal("0, 0", result);
        }

        [Fact]
        public void SerializeToString_ReturnsCorrectString_ForSizeWithZeroWidth()
        {
            // Arrange
            var size = new Size(0, 100);

            // Act
            string result = _serializer.SerializeToString(_serializationManager, size);

            // Assert
            Assert.Equal("0, 100", result);
        }

        [Fact]
        public void SerializeToString_ReturnsCorrectString_ForSizeWithZeroHeight()
        {
            // Arrange
            var size = new Size(100, 0);

            // Act
            string result = _serializer.SerializeToString(_serializationManager, size);

            // Assert
            Assert.Equal("100, 0", result);
        }

        [Fact]
        public void SerializeToString_ReturnsCorrectString_ForSizeWithLargeValues()
        {
            // Arrange
            var size = new Size(int.MaxValue, int.MaxValue);

            // Act
            string result = _serializer.SerializeToString(_serializationManager, size);

            // Assert
            Assert.Equal($"{int.MaxValue}, {int.MaxValue}", result);
        }

        [Fact]
        public void SerializeToString_ReturnsCorrectString_ForSizeWithOneValue()
        {
            // Arrange
            var size = new Size(50, 75);

            // Act
            string result = _serializer.SerializeToString(_serializationManager, size);

            // Assert
            Assert.Equal("50, 75", result);
        }

        #endregion

        #region DeserializeFromString Tests

        [Fact]
        public void DeserializeFromString_ReturnsCorrectSize_FromValidString()
        {
            // Arrange
            string sizeValue = "10, 20";

            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(Size), sizeValue);

            // Assert
            Assert.IsType<Size>(result);
            Size size = (Size)result;
            Assert.Equal(10, size.Width);
            Assert.Equal(20, size.Height);
        }

        [Fact]
        public void DeserializeFromString_ReturnsCorrectSize_FromStringWithZeroValues()
        {
            // Arrange
            string sizeValue = "0, 0";

            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(Size), sizeValue);

            // Assert
            Assert.IsType<Size>(result);
            Size size = (Size)result;
            Assert.Equal(Size.Empty, size);
        }

        [Fact]
        public void DeserializeFromString_ReturnsSizeEmpty_WhenValueIsNull()
        {
            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(Size), null);

            // Assert
            Assert.IsType<Size>(result);
            Size size = (Size)result;
            Assert.Equal(Size.Empty, size);
        }

        [Fact]
        public void DeserializeFromString_ReturnsSizeEmpty_WhenValueIsEmptyString()
        {
            // Arrange
            string sizeValue = string.Empty;

            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(Size), sizeValue);

            // Assert
            Assert.IsType<Size>(result);
            Size size = (Size)result;
            Assert.Equal(Size.Empty, size);
        }

        [Fact]
        public void DeserializeFromString_ReturnsSizeEmpty_WhenValueIsWhitespace()
        {
            // Arrange
            string sizeValue = "     ";

            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(Size), sizeValue);

            // Assert
            Assert.IsType<Size>(result);
            Size size = (Size)result;
            Assert.Equal(Size.Empty, size);
        }

        [Fact]
        public void DeserializeFromString_ReturnsCorrectSize_FromStringWithLargeValues()
        {
            // Arrange
            string sizeValue = "1000, 2000";

            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(Size), sizeValue);

            // Assert
            Assert.IsType<Size>(result);
            Size size = (Size)result;
            Assert.Equal(1000, size.Width);
            Assert.Equal(2000, size.Height);
        }

        [Fact]
        public void DeserializeFromString_ReturnsCorrectSize_FromStringWithNoSpaces()
        {
            // Arrange
            string sizeValue = "10,20";

            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(Size), sizeValue);

            // Assert
            Assert.IsType<Size>(result);
            Size size = (Size)result;
            Assert.Equal(10, size.Width);
            Assert.Equal(20, size.Height);
        }

        [Fact]
        public void DeserializeFromString_ReturnsCorrectSize_FromStringWithExtraSpaces()
        {
            // Arrange
            string sizeValue = "10,  20";

            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(Size), sizeValue);

            // Assert
            Assert.IsType<Size>(result);
            Size size = (Size)result;
            Assert.Equal(10, size.Width);
            Assert.Equal(20, size.Height);
        }

        #endregion

        #region RoundTrip Tests

        [Fact]
        public void RoundTrip_PreservesSize_ForNormalValues()
        {
            // Arrange
            var originalSize = new Size(100, 200);

            // Act
            string serialized = _serializer.SerializeToString(_serializationManager, originalSize);
            object deserialized = _serializer.DeserializeFromString(_serializationManager, typeof(Size), serialized);

            // Assert
            Assert.IsType<Size>(deserialized);
            Assert.Equal(originalSize, (Size)deserialized);
        }

        [Fact]
        public void RoundTrip_PreservesSizeEmpty()
        {
            // Arrange
            var originalSize = Size.Empty;

            // Act
            string serialized = _serializer.SerializeToString(_serializationManager, originalSize);
            object deserialized = _serializer.DeserializeFromString(_serializationManager, typeof(Size), serialized);

            // Assert
            Assert.IsType<Size>(deserialized);
            Assert.Equal(originalSize, (Size)deserialized);
        }

        [Fact]
        public void RoundTrip_PreservesSize_ForLargeValues()
        {
            // Arrange
            var originalSize = new Size(9999, 8888);

            // Act
            string serialized = _serializer.SerializeToString(_serializationManager, originalSize);
            object deserialized = _serializer.DeserializeFromString(_serializationManager, typeof(Size), serialized);

            // Assert
            Assert.IsType<Size>(deserialized);
            Assert.Equal(originalSize, (Size)deserialized);
        }

        #endregion
    }
}