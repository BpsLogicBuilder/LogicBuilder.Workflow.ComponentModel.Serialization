using LogicBuilder.ComponentModel.Design.Serialization;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Xunit;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization
{
    public class CollectionMarkupSerializerTest
    {
        private readonly CollectionMarkupSerializer _serializer;
        private readonly WorkflowMarkupSerializationManager _serializationManager;

        public CollectionMarkupSerializerTest()
        {
            _serializer = new CollectionMarkupSerializer();
            _serializationManager = new WorkflowMarkupSerializationManager(new DesignerSerializationManager());
        }

        #region GetChildren Tests

        [Fact]
        public void GetChildren_ThrowsArgumentNullException_WhenObjectIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _serializer.GetChildren(_serializationManager, null));
        }

        [Fact]
        public void GetChildren_ThrowsException_WhenObjectIsNotValidCollectionType()
        {
            // Arrange
            var invalidObject = "not a collection";

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _serializer.GetChildren(_serializationManager, invalidObject));
            Assert.Contains("The object of type", exception.Message);
            Assert.Contains("needs to implement", exception.Message);
            Assert.Contains("in order to be serialized as a Collection", exception.Message);
        }

        [Fact]
        public void GetChildren_ThrowsException_WhenObjectIsArray()
        {
            // Arrange
            var array = new int[] { 1, 2, 3 };

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _serializer.GetChildren(_serializationManager, array));
            Assert.Contains("The object of type", exception.Message);
            Assert.Contains("needs to implement", exception.Message);
            Assert.Contains("in order to be serialized as a Collection", exception.Message);
        }

        [Fact]
        public void GetChildren_ReturnsArrayList_WithAllItemsFromList()
        {
            // Arrange
            var list = new List<string> { "item1", "item2", "item3" };

            // Act
            IList result = _serializer.GetChildren(_serializationManager, list);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("item1", result[0]);
            Assert.Equal("item2", result[1]);
            Assert.Equal("item3", result[2]);
        }

        [Fact]
        public void GetChildren_ReturnsEmptyArrayList_WhenCollectionIsEmpty()
        {
            // Arrange
            var list = new List<int>();

            // Act
            IList result = _serializer.GetChildren(_serializationManager, list);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetChildren_ReturnsArrayList_WithItemsFromCollection()
        {
            // Arrange
            var collection = new Collection<int> { 10, 20, 30 };

            // Act
            IList result = _serializer.GetChildren(_serializationManager, collection);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal(10, result[0]);
            Assert.Equal(20, result[1]);
            Assert.Equal(30, result[2]);
        }

        #endregion

        #region GetProperties Tests

        [Fact]
        public void GetProperties_ReturnsEmptyArray()
        {
            // Arrange
            var list = new List<string> { "test" };

            // Act
            PropertyInfo[] result = _serializer.GetProperties(_serializationManager, list);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region ShouldSerializeValue Tests

        [Fact]
        public void ShouldSerializeValue_ReturnsFalse_WhenValueIsNull()
        {
            // Act
            bool result = _serializer.ShouldSerializeValue(_serializationManager, null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ShouldSerializeValue_ThrowsException_WhenValueIsNotValidCollectionType()
        {
            // Arrange
            var invalidValue = "not a collection";

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _serializer.ShouldSerializeValue(_serializationManager, invalidValue));
            Assert.Contains("The object of type", exception.Message);
            Assert.Contains("needs to implement", exception.Message);
            Assert.Contains("in order to be serialized as a Collection", exception.Message);
        }

        [Fact]
        public void ShouldSerializeValue_ThrowsException_WhenValueIsArray()
        {
            // Arrange
            var array = new int[] { 1, 2, 3 };

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _serializer.ShouldSerializeValue(_serializationManager, array));
            Assert.Contains("The object of type", exception.Message);
            Assert.Contains("needs to implement", exception.Message);
            Assert.Contains("in order to be serialized as a Collection", exception.Message);
        }

        [Fact]
        public void ShouldSerializeValue_ReturnsTrue_WhenCollectionHasItems()
        {
            // Arrange
            var list = new List<string> { "item1", "item2" };

            // Act
            bool result = _serializer.ShouldSerializeValue(_serializationManager, list);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ShouldSerializeValue_ReturnsFalse_WhenCollectionIsEmpty()
        {
            // Arrange
            var list = new List<int>();

            // Act
            bool result = _serializer.ShouldSerializeValue(_serializationManager, list);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region ClearChildren Tests

        [Fact]
        public void ClearChildren_ThrowsArgumentNullException_WhenObjectIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _serializer.ClearChildren(_serializationManager, null));
        }

        [Fact]
        public void ClearChildren_ThrowsException_WhenObjectIsNotValidCollectionType()
        {
            // Arrange
            var invalidObject = "not a collection";

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _serializer.ClearChildren(_serializationManager, invalidObject));
            Assert.Contains("in order to be serialized as a Collection", exception.Message);
        }

        [Fact]
        public void ClearChildren_ThrowsException_WhenObjectIsArray()
        {
            // Arrange
            var array = new int[] { 1, 2, 3 };

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _serializer.ClearChildren(_serializationManager, array));
            Assert.Contains("The object of type", exception.Message);
            Assert.Contains("needs to implement", exception.Message);
            Assert.Contains("in order to be serialized as a Collection", exception.Message);
        }

        [Fact]
        public void ClearChildren_ClearsCollection_WhenCollectionHasItems()
        {
            // Arrange
            var list = new List<string> { "item1", "item2", "item3" };

            // Act
            _serializer.ClearChildren(_serializationManager, list);

            // Assert
            Assert.Empty(list);
        }

        [Fact]
        public void ClearChildren_DoesNotThrow_WhenCollectionIsAlreadyEmpty()
        {
            // Arrange
            var list = new List<int>();

            // Act
            _serializer.ClearChildren(_serializationManager, list);

            // Assert
            Assert.Empty(list);
        }

        #endregion

        #region AddChild Tests

        [Fact]
        public void AddChild_ThrowsArgumentNullException_WhenParentObjectIsNull()
        {
            // Arrange
            var childObj = "child";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _serializer.AddChild(_serializationManager, null, childObj));
        }

        [Fact]
        public void AddChild_ThrowsException_WhenParentObjectIsNotValidCollectionType()
        {
            // Arrange
            var invalidParent = "not a collection";
            var childObj = "child";

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _serializer.AddChild(_serializationManager, invalidParent, childObj));
            Assert.Contains("The object of type", exception.Message);
            Assert.Contains("needs to implement", exception.Message);
            Assert.Contains("in order to be serialized as a Collection", exception.Message);
        }

        [Fact]
        public void AddChild_ThrowsException_WhenParentObjectIsArray()
        {
            // Arrange
            var array = new int[] { 1, 2, 3 };
            var childObj = 4;

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _serializer.AddChild(_serializationManager, array, childObj));
            Assert.Contains("The object of type", exception.Message);
            Assert.Contains("needs to implement", exception.Message);
            Assert.Contains("in order to be serialized as a Collection", exception.Message);
        }

        [Fact]
        public void AddChild_AddsChildToList()
        {
            // Arrange
            var list = new List<string> { "item1" };
            var childObj = "item2";

            // Act
            _serializer.AddChild(_serializationManager, list, childObj);

            // Assert
            Assert.Equal(2, list.Count);
            Assert.Equal("item2", list[1]);
        }

        [Fact]
        public void AddChild_AddsChildToEmptyCollection()
        {
            // Arrange
            var list = new List<int>();
            var childObj = 42;

            // Act
            _serializer.AddChild(_serializationManager, list, childObj);

            // Assert
            Assert.Single(list);
            Assert.Equal(42, list[0]);
        }

        [Fact]
        public void AddChild_AddsMultipleChildren()
        {
            // Arrange
            var list = new List<string>();

            // Act
            _serializer.AddChild(_serializationManager, list, "first");
            _serializer.AddChild(_serializationManager, list, "second");
            _serializer.AddChild(_serializationManager, list, "third");

            // Assert
            Assert.Equal(3, list.Count);
            Assert.Equal("first", list[0]);
            Assert.Equal("second", list[1]);
            Assert.Equal("third", list[2]);
        }

        #endregion

        #region IsValidCollectionType Tests

        [Fact]
        public void IsValidCollectionType_ReturnsFalse_WhenTypeIsNull()
        {
            // Act
            bool result = CollectionMarkupSerializer.IsValidCollectionType(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidCollectionType_ReturnsFalse_WhenTypeIsArray()
        {
            // Arrange
            var arrayType = typeof(int[]);

            // Act
            bool result = CollectionMarkupSerializer.IsValidCollectionType(arrayType);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidCollectionType_ReturnsTrue_WhenTypeImplementsICollection()
        {
            // Arrange
            var listType = typeof(List<string>);

            // Act
            bool result = CollectionMarkupSerializer.IsValidCollectionType(listType);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValidCollectionType_ReturnsTrue_WhenTypeImplementsICollectionGeneric()
        {
            // Arrange
            var collectionType = typeof(Collection<int>);

            // Act
            bool result = CollectionMarkupSerializer.IsValidCollectionType(collectionType);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValidCollectionType_ReturnsTrue_WhenTypeImplementsIListGeneric()
        {
            // Arrange
            var listType = typeof(List<object>);

            // Act
            bool result = CollectionMarkupSerializer.IsValidCollectionType(listType);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValidCollectionType_ReturnsFalse_WhenTypeIsString()
        {
            // Arrange
            var stringType = typeof(string);

            // Act
            bool result = CollectionMarkupSerializer.IsValidCollectionType(stringType);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidCollectionType_ReturnsFalse_WhenTypeIsNonCollection()
        {
            // Arrange
            var intType = typeof(int);

            // Act
            bool result = CollectionMarkupSerializer.IsValidCollectionType(intType);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidCollectionType_ReturnsTrue_WhenTypeIsArrayList()
        {
            // Arrange
            var arrayListType = typeof(ArrayList);

            // Act
            bool result = CollectionMarkupSerializer.IsValidCollectionType(arrayListType);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsInvalidCollectionType_ReturnsTrue_WhenTypeIsHashSet()
        {
            // Arrange
            var hashSetType = typeof(HashSet<string>);

            // Act
            bool result = CollectionMarkupSerializer.IsValidCollectionType(hashSetType);

            // Assert
            Assert.False(result);
        }

        #endregion
    }
}