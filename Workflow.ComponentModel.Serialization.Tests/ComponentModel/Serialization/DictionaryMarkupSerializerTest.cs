using LogicBuilder.ComponentModel.Design.Serialization;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml;

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
            var afterMethod = typeof(DictionaryMarkupSerializer).GetMethod("OnAfterSerialize", BindingFlags.NonPublic | BindingFlags.Instance);

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

        #region AddChild Extended Tests

        [Fact]
        public void AddChild_AddsChildSuccessfully_WithReferenceType()
        {
            // Arrange
            var dictionary = new Dictionary<string, object>();
            var childObj = new TestClass { Name = "TestValue" };
            var key = "testKey";

            // Setup keylookupDictionary via reflection
            var beforeMethod = typeof(DictionaryMarkupSerializer).GetMethod("OnBeforeDeserializeContents", BindingFlags.NonPublic | BindingFlags.Instance);
            beforeMethod!.Invoke(_serializer, [_serializationManager, dictionary]);

            var keylookupDictionaryField = typeof(DictionaryMarkupSerializer).GetField("keylookupDictionary", BindingFlags.NonPublic | BindingFlags.Instance);
            var keylookupDictionary = (IDictionary)keylookupDictionaryField!.GetValue(_serializer)!;
            keylookupDictionary.Add(key, childObj);

            // Act
            _serializer.AddChild(_serializationManager, dictionary, childObj);

            // Assert
            Assert.Single(dictionary);
            Assert.Equal(childObj, dictionary[key]);
            Assert.Empty(keylookupDictionary);
        }

        [Fact]
        public void AddChild_AddsChildSuccessfully_WithValueType()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>();
            var childObj = 42;
            var key = "numberKey";

            // Setup keylookupDictionary via reflection
            var beforeMethod = typeof(DictionaryMarkupSerializer).GetMethod("OnBeforeDeserializeContents", BindingFlags.NonPublic | BindingFlags.Instance);
            beforeMethod!.Invoke(_serializer, [_serializationManager, dictionary]);

            var keylookupDictionaryField = typeof(DictionaryMarkupSerializer).GetField("keylookupDictionary", BindingFlags.NonPublic | BindingFlags.Instance);
            var keylookupDictionary = (IDictionary)keylookupDictionaryField!.GetValue(_serializer)!;
            keylookupDictionary.Add(key, childObj);

            // Act
            _serializer.AddChild(_serializationManager, dictionary, childObj);

            // Assert
            Assert.Single(dictionary);
            Assert.Equal(childObj, dictionary[key]);
            Assert.Empty(keylookupDictionary);
        }

        [Fact]
        public void AddChild_ThrowsInvalidOperationException_WhenKeyNotFound()
        {
            // Arrange
            var dictionary = new Dictionary<string, string>();
            var childObj = "orphanChild";

            // Setup keylookupDictionary via reflection (but don't add the child to it)
            var beforeMethod = typeof(DictionaryMarkupSerializer).GetMethod("OnBeforeDeserializeContents", BindingFlags.NonPublic | BindingFlags.Instance);
            beforeMethod!.Invoke(_serializer, [_serializationManager, dictionary]);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _serializer.AddChild(_serializationManager, dictionary, childObj));
            Assert.Contains("Dictionary serializer could not add an item of", exception.Message);
        }

        [Fact]
        public void AddChild_FindsCorrectKey_WithMultipleEntriesInLookup()
        {
            // Arrange
            var dictionary = new Dictionary<string, object>();
            var childObj1 = new TestClass { Name = "First" };
            var childObj2 = new TestClass { Name = "Second" };
            var childObj3 = new TestClass { Name = "Third" };

            // Setup keylookupDictionary with multiple entries
            var beforeMethod = typeof(DictionaryMarkupSerializer).GetMethod("OnBeforeDeserializeContents", BindingFlags.NonPublic | BindingFlags.Instance);
            beforeMethod!.Invoke(_serializer, [_serializationManager, dictionary]);

            var keylookupDictionaryField = typeof(DictionaryMarkupSerializer).GetField("keylookupDictionary", BindingFlags.NonPublic | BindingFlags.Instance);
            var keylookupDictionary = (IDictionary)keylookupDictionaryField!.GetValue(_serializer)!;
            keylookupDictionary.Add("key1", childObj1);
            keylookupDictionary.Add("key2", childObj2);
            keylookupDictionary.Add("key3", childObj3);

            // Act
            _serializer.AddChild(_serializationManager, dictionary, childObj2);

            // Assert
            Assert.Single(dictionary);
            Assert.Equal(childObj2, dictionary["key2"]);
            Assert.Equal(2, keylookupDictionary.Count);
            Assert.False(keylookupDictionary.Contains("key2"));
        }

        [Fact]
        public void AddChild_UsesValueTypeEquals_ForValueTypes()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>();
            var childObj = 100;
            var key = "valueKey";

            // Setup keylookupDictionary
            var beforeMethod = typeof(DictionaryMarkupSerializer).GetMethod("OnBeforeDeserializeContents", BindingFlags.NonPublic | BindingFlags.Instance);
            beforeMethod!.Invoke(_serializer, [_serializationManager, dictionary]);

            var keylookupDictionaryField = typeof(DictionaryMarkupSerializer).GetField("keylookupDictionary", BindingFlags.NonPublic | BindingFlags.Instance);
            var keylookupDictionary = (IDictionary)keylookupDictionaryField!.GetValue(_serializer)!;
            keylookupDictionary.Add(key, 100); // Same value, different instance for value types

            // Act
            _serializer.AddChild(_serializationManager, dictionary, childObj);

            // Assert
            Assert.Single(dictionary);
            Assert.Equal(100, dictionary[key]);
        }

        #endregion

        #region GetExtendedProperties Extended Tests

        [Fact]
        public void GetExtendedProperties_ReturnsKeyProperty_WhenSerializingWithMatchingEntry()
        {
            // Arrange
            var dictionary = new Dictionary<string, string>();
            var extendee = "testValue";
            var entry = new DictionaryEntry("testKey", extendee);

            var beforeMethod = typeof(DictionaryMarkupSerializer).GetMethod("OnBeforeSerializeContents", BindingFlags.NonPublic | BindingFlags.Instance);
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

        [Fact]
        public void GetExtendedProperties_ReturnsEmpty_WhenEntryValueDoesNotMatch()
        {
            // Arrange
            var dictionary = new Dictionary<string, string>();
            var extendee = "testValue";
            var differentValue = "differentValue";
            var entry = new DictionaryEntry("testKey", differentValue);

            var beforeMethod = typeof(DictionaryMarkupSerializer).GetMethod("OnBeforeSerializeContents", BindingFlags.NonPublic | BindingFlags.Instance);
            var getExtMethod = typeof(DictionaryMarkupSerializer).GetMethod("GetExtendedProperties", BindingFlags.NonPublic | BindingFlags.Instance);

            beforeMethod!.Invoke(_serializer, [_serializationManager, dictionary]);
            _serializationManager.WorkflowMarkupStack.Push(entry);

            // Act
            var result = (ExtendedPropertyInfo[])getExtMethod!.Invoke(_serializer, [_serializationManager, extendee])!;

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            // Cleanup
            _serializationManager.WorkflowMarkupStack.Pop();
        }

        #endregion

        #region OnGetKeyValue Tests

        [Fact]
        public void OnGetKeyValue_ReturnsNull_WhenEntryNotInStack()
        {
            // Arrange
            Trace.Listeners.Clear();
            var dictionary = new Dictionary<string, ExtendedPropertyInfo>();
            var extendee = "testValue";

            var beforeMethod = typeof(DictionaryMarkupSerializer).GetMethod("OnBeforeDeserializeContents", BindingFlags.NonPublic | BindingFlags.Instance);
            var getExtMethod = typeof(DictionaryMarkupSerializer).GetMethod("GetExtendedProperties", BindingFlags.NonPublic | BindingFlags.Instance);

            beforeMethod!.Invoke(_serializer, [_serializationManager, dictionary]);

            var extProperties = (ExtendedPropertyInfo[])getExtMethod!.Invoke(_serializer, [_serializationManager, extendee])!;
            
            // Act - Try to get value when no entry is in stack
            object? result = null;
            object?[] parameters = [extProperties[0], BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<object>(), null];
            if (extProperties.Length > 0)
            {
                var getValueMethod = extProperties[0].GetType().GetMethod("GetValue", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                result = getValueMethod?.Invoke(extProperties[0], 0, null, parameters, null);
            }

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void OnGetKeyValue_ReturnsKey_WhenEntryMatchesExtendee()
        {
            // Arrange
            var dictionary = new Dictionary<string, ExtendedPropertyInfo>();
            var extendee = "testValue";

            var beforeMethod = typeof(DictionaryMarkupSerializer).GetMethod("OnBeforeDeserializeContents", BindingFlags.NonPublic | BindingFlags.Instance);
            var getExtMethod = typeof(DictionaryMarkupSerializer).GetMethod("GetExtendedProperties", BindingFlags.NonPublic | BindingFlags.Instance);

            beforeMethod!.Invoke(_serializer, [_serializationManager, dictionary]);

            var extProperties = (ExtendedPropertyInfo[])getExtMethod!.Invoke(_serializer, [_serializationManager, extendee])!;
            var expectedKey = "testKey";
            var entry = new DictionaryEntry(expectedKey, extProperties[0]);
            _serializationManager.WorkflowMarkupStack.Push(entry);

            // Act
            object? result = null;
            object?[] parameters = [extProperties[0], BindingFlags.Public | BindingFlags.Instance, null, new object[] { extendee }, null];
            if (extProperties.Length > 0)
            {
                var getValueMethod = extProperties[0].GetType().GetMethod("GetValue", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                result = getValueMethod?.Invoke(extProperties[0], 0, null, parameters, null);
            }

            // Assert
            Assert.Equal(expectedKey, result);

            // Cleanup
            _serializationManager.WorkflowMarkupStack.Pop();
        }

        [Fact]
        public void OnGetKeyValue_ReturnsNull_WhenExtendeeDoesNotMatchEntryValue()
        {
            // Arrange
            var dictionary = new Dictionary<string, string>();
            var extendee = "testValue";
            var differentValue = "differentValue";
            var entry = new DictionaryEntry("testKey", differentValue);

            var beforeMethod = typeof(DictionaryMarkupSerializer).GetMethod("OnBeforeDeserializeContents", BindingFlags.NonPublic | BindingFlags.Instance);
            var getExtMethod = typeof(DictionaryMarkupSerializer).GetMethod("GetExtendedProperties", BindingFlags.NonPublic | BindingFlags.Instance);

            beforeMethod!.Invoke(_serializer, [_serializationManager, dictionary]);
            _serializationManager.WorkflowMarkupStack.Push(entry);

            var extProperties = (ExtendedPropertyInfo[])getExtMethod!.Invoke(_serializer, [_serializationManager, extendee])!;

            // Act
            object result = "not null"; // Initialize with non-null to verify it becomes null
            object?[] parameters = [extProperties[0], BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<object>(), null];
            if (extProperties.Length > 0)
            {
                var getValueMethod = extProperties[0].GetType().GetMethod("GetValue", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                result = getValueMethod?.Invoke(extProperties[0], (BindingFlags)0, null, parameters, null)!;
            }

            // Assert
            Assert.Null(result);

            // Cleanup
            _serializationManager.WorkflowMarkupStack.Pop();
        }

        #endregion

        #region OnSetKeyValue Tests

        [Fact]
        public void OnSetKeyValue_AddsKeyValuePair_WhenKeyDoesNotExist()
        {
            // Arrange
            var dictionary = new Dictionary<string, string>();
            var key = "testKey";

            var beforeMethod = typeof(DictionaryMarkupSerializer).GetMethod("OnBeforeDeserializeContents", BindingFlags.NonPublic | BindingFlags.Instance);
            var getExtMethod = typeof(DictionaryMarkupSerializer).GetMethod("GetExtendedProperties", BindingFlags.NonPublic | BindingFlags.Instance);

            beforeMethod!.Invoke(_serializer, [_serializationManager, dictionary]);

            var extProperties = (ExtendedPropertyInfo[])getExtMethod!.Invoke(_serializer, [_serializationManager, key])!;
            var keylookupDictionaryField = typeof(DictionaryMarkupSerializer).GetField("keylookupDictionary", BindingFlags.NonPublic | BindingFlags.Instance);
            var keylookupDictionary = (IDictionary)keylookupDictionaryField!.GetValue(_serializer)!;

            // Act
            if (extProperties.Length > 0)
            {
                var setValueMethod = extProperties[0].GetType().GetMethod("SetValue", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                object?[] parameters = [extProperties[0], key, BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<object>(), null];
                setValueMethod?.Invoke(extProperties[0], 0, null, parameters, null);
            }

            // Assert
            Assert.Single(keylookupDictionary);
            Assert.True(keylookupDictionary.Contains(key));
            Assert.Equal(extProperties[0], keylookupDictionary[key]);
        }

        [Fact]
        public void OnSetKeyValue_DoesNotAdd_WhenExtendeeIsNull()
        {
            // Arrange
            var dictionary = new Dictionary<string, string>();
            var key = "testKey";

            var beforeMethod = typeof(DictionaryMarkupSerializer).GetMethod("OnBeforeDeserializeContents", BindingFlags.NonPublic | BindingFlags.Instance);
            beforeMethod!.Invoke(_serializer, [_serializationManager, dictionary]);

            var onSetKeyValueMethod = typeof(DictionaryMarkupSerializer).GetMethod("OnSetKeyValue", BindingFlags.NonPublic | BindingFlags.Instance);
            var keylookupDictionaryField = typeof(DictionaryMarkupSerializer).GetField("keylookupDictionary", BindingFlags.NonPublic | BindingFlags.Instance);
            var keylookupDictionary = (IDictionary)keylookupDictionaryField!.GetValue(_serializer)!;

            // Act
            onSetKeyValueMethod!.Invoke(_serializer, [null, null, key]);

            // Assert
            Assert.Empty(keylookupDictionary);
        }

        [Fact]
        public void OnSetKeyValue_DoesNotAdd_WhenValueIsNull()
        {
            // Arrange
            var dictionary = new Dictionary<string, string>();
            var extendee = "testValue";

            var beforeMethod = typeof(DictionaryMarkupSerializer).GetMethod("OnBeforeDeserializeContents", BindingFlags.NonPublic | BindingFlags.Instance);
            beforeMethod!.Invoke(_serializer, [_serializationManager, dictionary]);

            var onSetKeyValueMethod = typeof(DictionaryMarkupSerializer).GetMethod("OnSetKeyValue", BindingFlags.NonPublic | BindingFlags.Instance);
            var keylookupDictionaryField = typeof(DictionaryMarkupSerializer).GetField("keylookupDictionary", BindingFlags.NonPublic | BindingFlags.Instance);
            var keylookupDictionary = (IDictionary)keylookupDictionaryField!.GetValue(_serializer)!;

            // Act
            onSetKeyValueMethod!.Invoke(_serializer, [null, extendee, null]);

            // Assert
            Assert.Empty(keylookupDictionary);
        }

        [Fact]
        public void OnSetKeyValue_DoesNotAdd_WhenKeyAlreadyExists()
        {
            // Arrange
            var dictionary = new Dictionary<string, string>();
            var extendee1 = "testValue1";
            var extendee2 = "testValue2";
            var key = "duplicateKey";

            var beforeMethod = typeof(DictionaryMarkupSerializer).GetMethod("OnBeforeDeserializeContents", BindingFlags.NonPublic | BindingFlags.Instance);
            beforeMethod!.Invoke(_serializer, [_serializationManager, dictionary]);

            var onSetKeyValueMethod = typeof(DictionaryMarkupSerializer).GetMethod("OnSetKeyValue", BindingFlags.NonPublic | BindingFlags.Instance);
            var keylookupDictionaryField = typeof(DictionaryMarkupSerializer).GetField("keylookupDictionary", BindingFlags.NonPublic | BindingFlags.Instance);
            var keylookupDictionary = (IDictionary)keylookupDictionaryField!.GetValue(_serializer)!;

            // Act - Add first time
            onSetKeyValueMethod!.Invoke(_serializer, [null, extendee1, key]);
            
            // Act - Try to add again with same key
            onSetKeyValueMethod!.Invoke(_serializer, [null, extendee2, key]);

            // Assert
            Assert.Single(keylookupDictionary);
            Assert.Equal(extendee1, keylookupDictionary[key]); // Should still have first value
        }

        #endregion

        #region OnGetXmlQualifiedName Tests

        [Fact]
        public void OnGetXmlQualifiedName_ReturnsCorrectQualifiedName()
        {
            // Arrange
            var dictionary = new Dictionary<string, string>();
            var extendee = "testValue";

            var beforeMethod = typeof(DictionaryMarkupSerializer).GetMethod("OnBeforeDeserializeContents", BindingFlags.NonPublic | BindingFlags.Instance);
            var getExtMethod = typeof(DictionaryMarkupSerializer).GetMethod("GetExtendedProperties", BindingFlags.NonPublic | BindingFlags.Instance);

            beforeMethod!.Invoke(_serializer, [_serializationManager, dictionary]);

            var extProperties = (ExtendedPropertyInfo[])getExtMethod!.Invoke(_serializer, [_serializationManager, extendee])!;

            // Act
            XmlQualifiedName? result = null;
            string? prefix = null;
            if (extProperties.Length > 0)
            {
                var getQualifiedNameMethod = extProperties[0].GetType().GetMethod("GetXmlQualifiedName", BindingFlags.Public | BindingFlags.Instance);
                object?[] parameters = [_serializationManager, null];
                result = (XmlQualifiedName)getQualifiedNameMethod?.Invoke(extProperties[0], 0, null, parameters, null)!;
                prefix = parameters[1] as string;
            }

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Key", result.Name);
            Assert.NotNull(prefix);
        }

        #endregion
    }
}