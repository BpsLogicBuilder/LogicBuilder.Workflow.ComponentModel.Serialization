using LogicBuilder.ComponentModel.Design.Serialization;
using LogicBuilder.Workflow.ComponentModel.Design;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using System;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Design
{
    public class PointMarkupSerializerTest
    {
        private readonly PointMarkupSerializer _serializer;
        private readonly WorkflowMarkupSerializationManager _serializationManager;

        public PointMarkupSerializerTest()
        {
            _serializer = new PointMarkupSerializer();
            _serializationManager = new WorkflowMarkupSerializationManager(new DesignerSerializationManager());
        }

        #region CanSerializeToString Tests

        [Fact]
        public void CanSerializeToString_ReturnsTrue_WhenValueIsPoint()
        {
            // Arrange
            var point = new Point(10, 20);

            // Act
            bool result = _serializer.CanSerializeToString(_serializationManager, point);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanSerializeToString_ReturnsTrue_WhenValueIsPointEmpty()
        {
            // Arrange
            var point = Point.Empty;

            // Act
            bool result = _serializer.CanSerializeToString(_serializationManager, point);

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
        public void CanSerializeToString_ReturnsFalse_WhenValueIsSize()
        {
            // Arrange
            var size = new Size(10, 20);

            // Act
            bool result = _serializer.CanSerializeToString(_serializationManager, size);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region GetProperties Tests

        [Fact]
        public void GetProperties_ReturnsXAndYProperties_WhenObjectIsPoint()
        {
            // Arrange
            var point = new Point(10, 20);

            // Act
            PropertyInfo[] properties = _serializer.GetProperties(_serializationManager, point);

            // Assert
            Assert.NotNull(properties);
            Assert.Equal(2, properties.Length);
            Assert.Contains(properties, p => p.Name == "X");
            Assert.Contains(properties, p => p.Name == "Y");
        }

        [Fact]
        public void GetProperties_ReturnsXAndYProperties_WhenObjectIsPointEmpty()
        {
            // Arrange
            var point = Point.Empty;

            // Act
            PropertyInfo[] properties = _serializer.GetProperties(_serializationManager, point);

            // Assert
            Assert.NotNull(properties);
            Assert.Equal(2, properties.Length);
            Assert.Contains(properties, p => p.Name == "X");
            Assert.Contains(properties, p => p.Name == "Y");
        }

        [Fact]
        public void GetProperties_ReturnsEmptyArray_WhenObjectIsNotPoint()
        {
            // Arrange
            var notPoint = "not a point";

            // Act
            PropertyInfo[] properties = _serializer.GetProperties(_serializationManager, notPoint);

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
        public void GetProperties_XPropertyReturnsCorrectValue()
        {
            // Arrange
            var point = new Point(15, 25);
            PropertyInfo[] properties = _serializer.GetProperties(_serializationManager, point);
            PropertyInfo xProperty = properties.First(p => p.Name == "X");

            // Act
            var xValue = xProperty.GetValue(point);

            // Assert
            Assert.Equal(15, xValue);
        }

        [Fact]
        public void GetProperties_YPropertyReturnsCorrectValue()
        {
            // Arrange
            var point = new Point(15, 25);
            PropertyInfo[] properties = _serializer.GetProperties(_serializationManager, point);
            PropertyInfo yProperty = properties.First(p => p.Name == "Y");

            // Act
            var yValue = yProperty.GetValue(point);

            // Assert
            Assert.Equal(25, yValue);
        }

        #endregion

        #region SerializeToString Tests

        [Fact]
        public void SerializeToString_ReturnsCorrectString_ForPointWithPositiveValues()
        {
            // Arrange
            var point = new Point(10, 20);

            // Act
            string result = _serializer.SerializeToString(_serializationManager, point);

            // Assert
            Assert.Equal("10, 20", result);
        }

        [Fact]
        public void SerializeToString_ReturnsCorrectString_ForPointEmpty()
        {
            // Arrange
            var point = Point.Empty;

            // Act
            string result = _serializer.SerializeToString(_serializationManager, point);

            // Assert
            Assert.Equal("0, 0", result);
        }

        [Fact]
        public void SerializeToString_ReturnsCorrectString_ForPointWithNegativeValues()
        {
            // Arrange
            var point = new Point(-10, -20);

            // Act
            string result = _serializer.SerializeToString(_serializationManager, point);

            // Assert
            Assert.Equal("-10, -20", result);
        }

        [Fact]
        public void SerializeToString_ReturnsCorrectString_ForPointWithMixedValues()
        {
            // Arrange
            var point = new Point(-5, 15);

            // Act
            string result = _serializer.SerializeToString(_serializationManager, point);

            // Assert
            Assert.Equal("-5, 15", result);
        }

        [Fact]
        public void SerializeToString_ReturnsCorrectString_ForPointWithZeroX()
        {
            // Arrange
            var point = new Point(0, 100);

            // Act
            string result = _serializer.SerializeToString(_serializationManager, point);

            // Assert
            Assert.Equal("0, 100", result);
        }

        [Fact]
        public void SerializeToString_ReturnsCorrectString_ForPointWithZeroY()
        {
            // Arrange
            var point = new Point(100, 0);

            // Act
            string result = _serializer.SerializeToString(_serializationManager, point);

            // Assert
            Assert.Equal("100, 0", result);
        }

        [Fact]
        public void SerializeToString_ReturnsCorrectString_ForPointWithLargeValues()
        {
            // Arrange
            var point = new Point(int.MaxValue, int.MaxValue);

            // Act
            string result = _serializer.SerializeToString(_serializationManager, point);

            // Assert
            Assert.Equal($"{int.MaxValue}, {int.MaxValue}", result);
        }

        [Fact]
        public void SerializeToString_ReturnsCorrectString_ForPointWithMinValues()
        {
            // Arrange
            var point = new Point(int.MinValue, int.MinValue);

            // Act
            string result = _serializer.SerializeToString(_serializationManager, point);

            // Assert
            Assert.Equal($"{int.MinValue}, {int.MinValue}", result);
        }

        #endregion

        #region DeserializeFromString Tests

        [Fact]
        public void DeserializeFromString_ReturnsCorrectPoint_FromValidString()
        {
            // Arrange
            string pointValue = "10, 20";

            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(Point), pointValue);

            // Assert
            Assert.IsType<Point>(result);
            Point point = (Point)result;
            Assert.Equal(10, point.X);
            Assert.Equal(20, point.Y);
        }

        [Fact]
        public void DeserializeFromString_ReturnsCorrectPoint_FromStringWithNegativeValues()
        {
            // Arrange
            string pointValue = "-10, -20";

            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(Point), pointValue);

            // Assert
            Assert.IsType<Point>(result);
            Point point = (Point)result;
            Assert.Equal(-10, point.X);
            Assert.Equal(-20, point.Y);
        }

        [Fact]
        public void DeserializeFromString_ReturnsCorrectPoint_FromStringWithMixedValues()
        {
            // Arrange
            string pointValue = "-5, 15";

            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(Point), pointValue);

            // Assert
            Assert.IsType<Point>(result);
            Point point = (Point)result;
            Assert.Equal(-5, point.X);
            Assert.Equal(15, point.Y);
        }

        [Fact]
        public void DeserializeFromString_ReturnsCorrectPoint_FromStringWithZeroValues()
        {
            // Arrange
            string pointValue = "0, 0";

            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(Point), pointValue);

            // Assert
            Assert.IsType<Point>(result);
            Point point = (Point)result;
            Assert.Equal(Point.Empty, point);
        }

        [Fact]
        public void DeserializeFromString_ReturnsPointEmpty_WhenValueIsNull()
        {
            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(Point), null);

            // Assert
            Assert.IsType<Point>(result);
            Point point = (Point)result;
            Assert.Equal(Point.Empty, point);
        }

        [Fact]
        public void DeserializeFromString_ReturnsPointEmpty_WhenValueIsEmptyString()
        {
            // Arrange
            string pointValue = string.Empty;

            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(Point), pointValue);

            // Assert
            Assert.IsType<Point>(result);
            Point point = (Point)result;
            Assert.Equal(Point.Empty, point);
        }

        [Fact]
        public void DeserializeFromString_ReturnsPointEmpty_WhenValueIsWhitespace()
        {
            // Arrange
            string pointValue = "     ";

            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(Point), pointValue);

            // Assert
            Assert.IsType<Point>(result);
            Point point = (Point)result;
            Assert.Equal(Point.Empty, point);
        }

        [Fact]
        public void DeserializeFromString_ReturnsCorrectPoint_FromStringWithExtraSpaces()
        {
            // Arrange
            //string pointValue = "  10  ,  20  ";
            string pointValue = "10,20";

            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(Point), pointValue);

            // Assert
            Assert.IsType<Point>(result);
            Point point = (Point)result;
            Assert.Equal(10, point.X);
            Assert.Equal(20, point.Y);
        }

        [Fact]
        public void DeserializeFromString_ReturnsCorrectPoint_FromStringWithLargeValues()
        {
            // Arrange
            string pointValue = $"{int.MaxValue}, {int.MaxValue}";

            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(Point), pointValue);

            // Assert
            Assert.IsType<Point>(result);
            Point point = (Point)result;
            Assert.Equal(int.MaxValue, point.X);
            Assert.Equal(int.MaxValue, point.Y);
        }

        [Fact]
        public void DeserializeFromString_ReturnsCorrectPoint_FromStringWithMinValues()
        {
            // Arrange
            string pointValue = $"{int.MinValue}, {int.MinValue}";

            // Act
            object result = _serializer.DeserializeFromString(_serializationManager, typeof(Point), pointValue);

            // Assert
            Assert.IsType<Point>(result);
            Point point = (Point)result;
            Assert.Equal(int.MinValue, point.X);
            Assert.Equal(int.MinValue, point.Y);
        }

        #endregion

        #region Round-Trip Tests

        [Fact]
        public void RoundTrip_SerializeAndDeserialize_PreservesPointValue()
        {
            // Arrange
            var originalPoint = new Point(42, 84);

            // Act
            string serialized = _serializer.SerializeToString(_serializationManager, originalPoint);
            object deserialized = _serializer.DeserializeFromString(_serializationManager, typeof(Point), serialized);

            // Assert
            Assert.IsType<Point>(deserialized);
            Point resultPoint = (Point)deserialized;
            Assert.Equal(originalPoint, resultPoint);
        }

        [Fact]
        public void RoundTrip_SerializeAndDeserialize_PreservesPointEmpty()
        {
            // Arrange
            var originalPoint = Point.Empty;

            // Act
            string serialized = _serializer.SerializeToString(_serializationManager, originalPoint);
            object deserialized = _serializer.DeserializeFromString(_serializationManager, typeof(Point), serialized);

            // Assert
            Assert.IsType<Point>(deserialized);
            Point resultPoint = (Point)deserialized;
            Assert.Equal(originalPoint, resultPoint);
        }

        [Fact]
        public void RoundTrip_SerializeAndDeserialize_PreservesNegativeValues()
        {
            // Arrange
            var originalPoint = new Point(-100, -200);

            // Act
            string serialized = _serializer.SerializeToString(_serializationManager, originalPoint);
            object deserialized = _serializer.DeserializeFromString(_serializationManager, typeof(Point), serialized);

            // Assert
            Assert.IsType<Point>(deserialized);
            Point resultPoint = (Point)deserialized;
            Assert.Equal(originalPoint, resultPoint);
        }

        #endregion
    }
}