using LogicBuilder.ComponentModel.Design.Serialization;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization
{
    public class DictionaryMarkupSerializerTest
    {
        private readonly DictionaryMarkupSerializer _serializer;
        private readonly WorkflowMarkupSerializationManager _serializationManager;

        public DictionaryMarkupSerializerTest()
        {
            _serializer = new DictionaryMarkupSerializer();
            _serializationManager = new WorkflowMarkupSerializationManager(new DesignerSerializationManager());
        }

        #region GetChildren Tests

        [Fact]
        public void GetChildren_ThrowsInvalidOperationException_WhenObjectIsNotDictionary()
        {
            // Arrange
            var invalidObject = "not a dictionary";

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _serializer.GetChildren(_serializationManager, invalidObject));
            Assert.Contains("Dictionary serializer can not serialize objects not supporting IDictionary.", exception.Message);
        }

        [Fact]
        public void GetChildren_ThrowsInvalidOperationException_WhenObjectIsNull()
        {
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _serializer.GetChildren(_serializationManager, null));
            Assert.Contains("Dictionary serializer can not serialize objects not supporting IDictionary.", exception.Message);
        }

        [Fact]
        public void GetChildren_ReturnsListOfDictionaryEntries_WhenDictionaryHasItems()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>
            {
                { "first", 1 },
                { "second", 2 },
                { "third", 3 }
            };

            // Act
            List<object> result = [.. _serializer.GetChildren(_serializationManager, dictionary).OfType<object>()];

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.All(result, item => Assert.IsType<DictionaryEntry>(item));
        }

        [Fact]
        public void GetChildren_ReturnsEmptyList_WhenDictionaryIsEmpty()
        {
            // Arrange
            var dictionary = new Dictionary<string, object>();

            // Act
            IList result = _serializer.GetChildren(_serializationManager, dictionary);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetChildren_ReturnsCorrectDictionaryEntries()
        {
            // Arrange
            var dictionary = new Dictionary<string, string>
            {
                { "key1", "value1" },
                { "key2", "value2" }
            };

            // Act
            IList result = _serializer.GetChildren(_serializationManager, dictionary);

            // Assert
            Assert.Equal(2, result.Count);
            var entries = new List<DictionaryEntry>();
            foreach (var item in result)
            {
                entries.Add((DictionaryEntry)item);
            }
            Assert.Contains(entries, e => e.Key.Equals("key1") && e.Value!.Equals("value1"));
            Assert.Contains(entries, e => e.Key.Equals("key2") && e.Value!.Equals("value2"));
        }

        [Fact]
        public void GetChildren_WorksWithHashtable()
        {
            // Arrange
            var hashtable = new Hashtable
            {
                { "key1", "value1" },
                { 42, "value2" }
            };

            // Act
            IList result = _serializer.GetChildren(_serializationManager, hashtable);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        #endregion

        #region GetProperties Tests

        [Fact]
        public void GetProperties_ReturnsEmptyArray()
        {
            // Arrange
            var dictionary = new Dictionary<string, int> { { "test", 1 } };

            // Act
            PropertyInfo[] result = _serializer.GetProperties(_serializationManager, dictionary);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetProperties_ReturnsEmptyArray_EvenForNullObject()
        {
            // Act
            PropertyInfo[] result = _serializer.GetProperties(_serializationManager, null);

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
        public void ShouldSerializeValue_ThrowsInvalidOperationException_WhenValueIsNotDictionary()
        {
            // Arrange
            var invalidValue = "not a dictionary";

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _serializer.ShouldSerializeValue(_serializationManager, invalidValue));
            Assert.Contains("Dictionary serializer can not serialize objects not supporting IDictionary.", exception.Message);
        }

        [Fact]
        public void ShouldSerializeValue_ReturnsTrue_WhenDictionaryHasItems()
        {
            // Arrange
            var dictionary = new Dictionary<string, string> { { "key", "value" } };

            // Act
            bool result = _serializer.ShouldSerializeValue(_serializationManager, dictionary);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ShouldSerializeValue_ReturnsFalse_WhenDictionaryIsEmpty()
        {
            // Arrange
            var dictionary = new Dictionary<int, int>();

            // Act
            bool result = _serializer.ShouldSerializeValue(_serializationManager, dictionary);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ShouldSerializeValue_ReturnsTrue_WhenHashtableHasItems()
        {
            // Arrange
            var hashtable = new Hashtable { { "key", "value" } };

            // Act
            bool result = _serializer.ShouldSerializeValue(_serializationManager, hashtable);

            // Assert
            Assert.True(result);
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
        public void ClearChildren_ThrowsInvalidOperationException_WhenObjectIsNotDictionary()
        {
            // Arrange
            var invalidObject = "not a dictionary";

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _serializer.ClearChildren(_serializationManager, invalidObject));
            Assert.Contains("Dictionary serializer can not serialize objects not supporting IDictionary.", exception.Message);
        }

        [Fact]
        public void ClearChildren_ClearsDictionary_WhenDictionaryHasItems()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>
            {
                { "first", 1 },
                { "second", 2 },
                { "third", 3 }
            };

            // Act
            _serializer.ClearChildren(_serializationManager, dictionary);

            // Assert
            Assert.Empty(dictionary);
        }

        [Fact]
        public void ClearChildren_DoesNotThrow_WhenDictionaryIsAlreadyEmpty()
        {
            // Arrange
            var dictionary = new Dictionary<string, string>();

            // Act
            _serializer.ClearChildren(_serializationManager, dictionary);

            // Assert
            Assert.Empty(dictionary);
        }

        [Fact]
        public void ClearChildren_ClearsHashtable()
        {
            // Arrange
            var hashtable = new Hashtable
            {
                { "key1", "value1" },
                { "key2", "value2" }
            };

            // Act
            _serializer.ClearChildren(_serializationManager, hashtable);

            // Assert
            Assert.Empty(hashtable);
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
        public void AddChild_ThrowsArgumentNullException_WhenChildObjectIsNull()
        {
            // Arrange
            var dictionary = new Dictionary<string, string>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _serializer.AddChild(_serializationManager, dictionary, null));
        }

        [Fact]
        public void AddChild_ThrowsInvalidOperationException_WhenParentObjectIsNotDictionary()
        {
            // Arrange
            var invalidParent = "not a dictionary";
            var childObj = "child";

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _serializer.AddChild(_serializationManager, invalidParent, childObj));
            Assert.Contains("Dictionary serializer can not serialize objects not supporting IDictionary.", exception.Message);
        }

        #endregion

        #region Serialization Lifecycle Tests

        [Fact]
        public void OnBeforeSerializeContents_AddsProviderToSerializationManager()
        {
            // Arrange
            var dictionary = new Dictionary<string, string> { { "key", "value" } };
            var method = typeof(DictionaryMarkupSerializer).GetMethod("OnBeforeSerializeContents", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            method!.Invoke(_serializer, [_serializationManager, dictionary]);

            // Assert
            Assert.Contains(_serializer, _serializationManager.ExtendedPropertiesProviders);
        }

        [Fact]
        public void OnAfterSerialize_RemovesProviderFromSerializationManager()
        {
            // Arrange
            var dictionary = new Dictionary<string, string> { { "key", "value" } };
            var beforeMethod = typeof(DictionaryMarkupSerializer).GetMethod("OnBeforeSerializeContents", BindingFlags.NonPublic | BindingFlags.Instance);
            var afterMethod = typeof(DictionaryMarkupSerializer).GetMethod("OnAfterSerialize", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            beforeMethod!.Invoke(_serializer, [_serializationManager, dictionary]);
            afterMethod!.Invoke(_serializer, [_serializationManager, dictionary]);

            // Assert
            Assert.DoesNotContain(_serializer, _serializationManager.ExtendedPropertiesProviders);
        }

        [Fact]
        public void OnBeforeDeserializeContents_AddsProviderToSerializationManager()
        {
            // Arrange
            var dictionary = new Dictionary<string, string>();
            var method = typeof(DictionaryMarkupSerializer).GetMethod("OnBeforeDeserializeContents", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            method!.Invoke(_serializer, [_serializationManager, dictionary]);

            // Assert
            Assert.Contains(_serializer, _serializationManager.ExtendedPropertiesProviders);
        }

        [Fact]
        public void OnAfterDeserialize_RemovesProviderFromSerializationManager()
        {
            // Arrange
            var dictionary = new Dictionary<string, string>();
            var beforeMethod = typeof(DictionaryMarkupSerializer).GetMethod("OnBeforeDeserializeContents", BindingFlags.NonPublic | BindingFlags.Instance);
            var afterMethod = typeof(DictionaryMarkupSerializer).GetMethod("OnAfterDeserialize", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            beforeMethod!.Invoke(_serializer, [_serializationManager, dictionary]);
            afterMethod!.Invoke(_serializer, [_serializationManager, dictionary]);

            // Assert
            Assert.DoesNotContain(_serializer, _serializationManager.ExtendedPropertiesProviders);
        }

        #endregion

        #region GetExtendedProperties Tests

        [Fact]
        public void GetExtendedProperties_ReturnsEmptyArray_WhenNotDeserializing()
        {
            // Arrange
            var extendee = "test";
            var method = typeof(DictionaryMarkupSerializer).GetMethod("GetExtendedProperties", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var result = (ExtendedPropertyInfo[])method!.Invoke(_serializer, [_serializationManager, extendee])!;

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetExtendedProperties_ReturnsKeyProperty_WhenDeserializing()
        {
            // Arrange
            var dictionary = new Dictionary<string, string>();
            var extendee = "testValue";
            var entry = new DictionaryEntry("testKey", extendee);
            
            var beforeMethod = typeof(DictionaryMarkupSerializer).GetMethod("OnBeforeDeserializeContents", BindingFlags.NonPublic | BindingFlags.Instance);
            var getExtMethod = typeof(DictionaryMarkupSerializer).GetMethod("GetExtendedProperties", BindingFlags.NonPublic | BindingFlags.Instance);
            
            beforeMethod!.Invoke(_serializer, [_serializationManager, dictionary]);
            _serializationManager.WorkflowMarkupStack.Push(entry);

            // Act
            var result = (ExtendedPropertyInfo[])getExtMethod!.Invoke(_serializer, [_serializationManager, extendee])!;

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Key", result[0].Name);

            // Cleanup
            _serializationManager.WorkflowMarkupStack.Pop();
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void DictionarySerializationWorkflow_WorksCorrectly()
        {
            // Arrange
            var sourceDictionary = new Dictionary<string, int>
            {
                { "first", 1 },
                { "second", 2 }
            };

            // Act - Get children
            List<object> children = [.. _serializer.GetChildren(_serializationManager, sourceDictionary).OfType<object>()];

            // Assert - Verify children
            Assert.Equal(2, children.Count);
            Assert.All(children, child => Assert.IsType<DictionaryEntry>(child));

            // Act - Clear and reconstruct (simulating deserialization)
            var targetDictionary = new Dictionary<string, int>
            {
                { "old", 999 }
            };
            _serializer.ClearChildren(_serializationManager, targetDictionary);
            Assert.Empty(targetDictionary);
        }

        [Fact]
        public void ShouldSerializeValue_WorksCorrectly_ForComplexTypes()
        {
            // Arrange
            var dictionary = new Dictionary<string, object>
            {
                { "key1", new { Name = "Test", Value = 42 } },
                { "key2", new List<int> { 1, 2, 3 } }
            };

            // Act
            bool result = _serializer.ShouldSerializeValue(_serializationManager, dictionary);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GetChildren_PreservesOrder_ForOrderedDictionary()
        {
            // Arrange
            var dictionary = new Dictionary<string, string>
            {
                { "a", "first" },
                { "b", "second" },
                { "c", "third" }
            };

            // Act
            IList result = _serializer.GetChildren(_serializationManager, dictionary);

            // Assert
            Assert.Equal(3, result.Count);
            var keys = new List<string>();
            foreach (DictionaryEntry entry in result)
            {
                keys.Add(entry.Key.ToString()!);
            }
            Assert.Equal(3, keys.Count);
        }

        [Fact]
        public void DictionarySerializer_HandlesValueTypes()
        {
            // Arrange
            var dictionary = new Dictionary<int, int>
            {
                { 1, 100 },
                { 2, 200 }
            };

            // Act
            IList children = _serializer.GetChildren(_serializationManager, dictionary);
            bool shouldSerialize = _serializer.ShouldSerializeValue(_serializationManager, dictionary);

            // Assert
            Assert.Equal(2, children.Count);
            Assert.True(shouldSerialize);
        }

        [Fact]
        public void DictionarySerializer_HandlesReferenceTypes()
        {
            // Arrange
            var obj1 = new TestClass { Name = "Test1" };
            var obj2 = new TestClass { Name = "Test2" };
            var dictionary = new Dictionary<string, TestClass>
            {
                { "key1", obj1 },
                { "key2", obj2 }
            };

            // Act
            IList children = _serializer.GetChildren(_serializationManager, dictionary);

            // Assert
            Assert.Equal(2, children.Count);
            var entries = new List<DictionaryEntry>();
            foreach (var child in children)
            {
                entries.Add((DictionaryEntry)child);
            }
            Assert.Contains(entries, e => e.Key.Equals("key1") && ((TestClass)e.Value!).Name == "Test1");
            Assert.Contains(entries, e => e.Key.Equals("key2") && ((TestClass)e.Value!).Name == "Test2");
        }

        #endregion

        #region Helper Classes

        private class TestClass
        {
            public string Name { get; set; } = string.Empty;
        }

        #endregion
    }
}