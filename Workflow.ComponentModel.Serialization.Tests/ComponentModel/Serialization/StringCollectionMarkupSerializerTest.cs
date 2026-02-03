using LogicBuilder.ComponentModel.Design.Serialization;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization
{
    public class StringCollectionMarkupSerializerTest
    {
        private readonly StringCollectionMarkupSerializer _serializer;
        private readonly WorkflowMarkupSerializationManager _serializationManager;

        public StringCollectionMarkupSerializerTest()
        {
            _serializer = new StringCollectionMarkupSerializer();
            _serializationManager = new WorkflowMarkupSerializationManager(new DesignerSerializationManager());
        }

        [Fact]
        public void GetProperties_ReturnsEmptyArray()
        {
            // Arrange
            var list = new List<string> { "a", "b" };

            // Act
            PropertyInfo[] result = _serializer.GetProperties(_serializationManager, list);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void CanSerializeToString_ReturnsTrue_ForStringCollection()
        {
            // Arrange
            var list = new List<string> { "x", "y" };

            // Act
            bool canSerialize = _serializer.CanSerializeToString(_serializationManager, list);

            // Assert
            Assert.True(canSerialize);
        }

        [Fact]
        public void CanSerializeToString_ReturnsFalse_ForNonStringCollection()
        {
            // Arrange
            var intList = new List<int> { 1, 2 };

            // Act
            bool canSerialize = _serializer.CanSerializeToString(_serializationManager, intList);

            // Assert
            Assert.False(canSerialize);
        }

        [Fact]
        public void CanSerializeToString_ThrowsArgumentNullException_WhenManagerIsNull()
        {
            // Arrange
            var list = new List<string> { "a" };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _serializer.CanSerializeToString(null, list));
        }

        [Fact]
        public void CanSerializeToString_ThrowsArgumentNullException_WhenValueIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _serializer.CanSerializeToString(_serializationManager, null));
        }

        [Fact]
        public void SerializeToString_ReturnsCommaSeparatedString()
        {
            // Arrange
            var list = new List<string> { "one", "two", "three" };

            // Act
            string result = _serializer.SerializeToString(_serializationManager, list);

            // Assert
            Assert.Equal("one, two, three", result);
        }

        [Fact]
        public void SerializeToString_HandlesCommasInStrings()
        {
            // Arrange
            var list = new List<string> { "a,1", "b,2" };

            // Act
            string result = _serializer.SerializeToString(_serializationManager, list);

            // Assert
            Assert.Equal("a\\,1, b\\,2", result);
        }

        [Fact]
        public void SerializeToString_ThrowsArgumentNullException_WhenManagerIsNull()
        {
            // Arrange
            var list = new List<string> { "a" };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _serializer.SerializeToString(null, list));
        }

        [Fact]
        public void SerializeToString_ThrowsArgumentNullException_WhenValueIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _serializer.SerializeToString(_serializationManager, null));
        }

        [Fact]
        public void DeserializeFromString_ReturnsStringCollection()
        {
            // Arrange
            string serialized = "foo, bar, baz";

            // Act
            var result = _serializer.DeserializeFromString(_serializationManager, typeof(List<string>), serialized);

            // Assert
            Assert.IsType<ICollection<string>>(result, exactMatch: false);
            var collection = (ICollection<string>)result;
            Assert.Contains("foo", collection);
            Assert.Contains("bar", collection);
            Assert.Contains("baz", collection);
        }

        [Fact]
        public void DeserializeFromString_HandlesEscapedCommas()
        {
            // Arrange
            string serialized = "a\\,1, b\\,2";

            // Act
            var result = _serializer.DeserializeFromString(_serializationManager, typeof(List<string>), serialized);

            // Assert
            Assert.IsType<ICollection<string>>(result, exactMatch: false);
            var collection = (ICollection<string>)result;
            Assert.Contains("a,1", collection);
            Assert.Contains("b,2", collection);
        }

        [Fact]
        public void DeserializeFromString_ThrowsArgumentNullException_WhenManagerIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _serializer.DeserializeFromString(null, typeof(List<string>), "foo"));
        }

        [Fact]
        public void DeserializeFromString_ThrowsArgumentNullException_WhenPropertyTypeIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _serializer.DeserializeFromString(_serializationManager, null, "foo"));
        }

        [Fact]
        public void DeserializeFromString_ThrowsArgumentNullException_WhenValueIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _serializer.DeserializeFromString(_serializationManager, typeof(List<string>), null));
        }
    }
}