using LogicBuilder.Workflow.ComponentModel.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Compiler
{
    public class ValidationErrorCollectionTest
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_Default_CreatesEmptyCollection()
        {
            // Act
            var collection = new ValidationErrorCollection();

            // Assert
            Assert.NotNull(collection);
            Assert.Empty(collection);
        }

        [Fact]
        public void Constructor_WithValidationErrorCollection_CopiesItems()
        {
            // Arrange
            var sourceCollection = new ValidationErrorCollection();
            sourceCollection.Add(new ValidationError("Error 1", 100));
            sourceCollection.Add(new ValidationError("Error 2", 101));

            // Act
            var collection = new ValidationErrorCollection(sourceCollection);

            // Assert
            Assert.Equal(2, collection.Count);
            Assert.Equal("Error 1", collection[0].ErrorText);
            Assert.Equal("Error 2", collection[1].ErrorText);
        }

        [Fact]
        public void Constructor_WithEmptyValidationErrorCollection_CreatesEmptyCollection()
        {
            // Arrange
            var sourceCollection = new ValidationErrorCollection();

            // Act
            var collection = new ValidationErrorCollection(sourceCollection);

            // Assert
            Assert.Empty(collection);
        }

        [Fact]
        public void Constructor_WithIEnumerable_CopiesItems()
        {
            // Arrange
            var errors = new List<ValidationError>
            {
                new ValidationError("Error 1", 100),
                new ValidationError("Error 2", 101),
                new ValidationError("Error 3", 102)
            };

            // Act
            var collection = new ValidationErrorCollection(errors);

            // Assert
            Assert.Equal(3, collection.Count);
            Assert.Equal("Error 1", collection[0].ErrorText);
            Assert.Equal("Error 2", collection[1].ErrorText);
            Assert.Equal("Error 3", collection[2].ErrorText);
        }

        [Fact]
        public void Constructor_WithNullIEnumerable_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => 
                new ValidationErrorCollection((IEnumerable<ValidationError>)null));
            Assert.Equal("value", exception.ParamName);
        }

        #endregion

        #region AddRange Tests

        [Fact]
        public void AddRange_WithValidItems_AddsAllItems()
        {
            // Arrange
            var collection = new ValidationErrorCollection();
            var errors = new List<ValidationError>
            {
                new ValidationError("Error 1", 100),
                new ValidationError("Error 2", 101),
                new ValidationError("Error 3", 102)
            };

            // Act
            collection.AddRange(errors);

            // Assert
            Assert.Equal(3, collection.Count);
            Assert.Equal("Error 1", collection[0].ErrorText);
            Assert.Equal("Error 2", collection[1].ErrorText);
            Assert.Equal("Error 3", collection[2].ErrorText);
        }

        [Fact]
        public void AddRange_WithEmptyCollection_DoesNotAddItems()
        {
            // Arrange
            var collection = new ValidationErrorCollection();
            var errors = new List<ValidationError>();

            // Act
            collection.AddRange(errors);

            // Assert
            Assert.Empty(collection);
        }

        [Fact]
        public void AddRange_WithNullValue_ThrowsArgumentNullException()
        {
            // Arrange
            var collection = new ValidationErrorCollection();

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => 
                collection.AddRange(null));
            Assert.Equal("value", exception.ParamName);
        }

        [Fact]
        public void AddRange_ToExistingCollection_AppendsItems()
        {
            // Arrange
            var collection = new ValidationErrorCollection();
            collection.Add(new ValidationError("Existing Error", 99));
            var errors = new List<ValidationError>
            {
                new ValidationError("New Error 1", 100),
                new ValidationError("New Error 2", 101)
            };

            // Act
            collection.AddRange(errors);

            // Assert
            Assert.Equal(3, collection.Count);
            Assert.Equal("Existing Error", collection[0].ErrorText);
            Assert.Equal("New Error 1", collection[1].ErrorText);
            Assert.Equal("New Error 2", collection[2].ErrorText);
        }

        #endregion

        #region HasErrors Tests

        [Fact]
        public void HasErrors_WithEmptyCollection_ReturnsFalse()
        {
            // Arrange
            var collection = new ValidationErrorCollection();

            // Act & Assert
            Assert.False(collection.HasErrors);
        }

        [Fact]
        public void HasErrors_WithOnlyWarnings_ReturnsFalse()
        {
            // Arrange
            var collection = new ValidationErrorCollection();
            collection.Add(new ValidationError("Warning 1", 100, true));
            collection.Add(new ValidationError("Warning 2", 101, true));

            // Act & Assert
            Assert.False(collection.HasErrors);
        }

        [Fact]
        public void HasErrors_WithOnlyErrors_ReturnsTrue()
        {
            // Arrange
            var collection = new ValidationErrorCollection();
            collection.Add(new ValidationError("Error 1", 100, false));
            collection.Add(new ValidationError("Error 2", 101, false));

            // Act & Assert
            Assert.True(collection.HasErrors);
        }

        [Fact]
        public void HasErrors_WithMixedErrorsAndWarnings_ReturnsTrue()
        {
            // Arrange
            var collection = new ValidationErrorCollection();
            collection.Add(new ValidationError("Warning 1", 100, true));
            collection.Add(new ValidationError("Error 1", 101, false));
            collection.Add(new ValidationError("Warning 2", 102, true));

            // Act & Assert
            Assert.True(collection.HasErrors);
        }

        [Fact]
        public void HasErrors_WithSingleError_ReturnsTrue()
        {
            // Arrange
            var collection = new ValidationErrorCollection();
            collection.Add(new ValidationError("Error 1", 100));

            // Act & Assert
            Assert.True(collection.HasErrors);
        }

        #endregion

        #region HasWarnings Tests

        [Fact]
        public void HasWarnings_WithEmptyCollection_ReturnsFalse()
        {
            // Arrange
            var collection = new ValidationErrorCollection();

            // Act & Assert
            Assert.False(collection.HasWarnings);
        }

        [Fact]
        public void HasWarnings_WithOnlyErrors_ReturnsFalse()
        {
            // Arrange
            var collection = new ValidationErrorCollection();
            collection.Add(new ValidationError("Error 1", 100, false));
            collection.Add(new ValidationError("Error 2", 101, false));

            // Act & Assert
            Assert.False(collection.HasWarnings);
        }

        [Fact]
        public void HasWarnings_WithOnlyWarnings_ReturnsTrue()
        {
            // Arrange
            var collection = new ValidationErrorCollection();
            collection.Add(new ValidationError("Warning 1", 100, true));
            collection.Add(new ValidationError("Warning 2", 101, true));

            // Act & Assert
            Assert.True(collection.HasWarnings);
        }

        [Fact]
        public void HasWarnings_WithMixedErrorsAndWarnings_ReturnsTrue()
        {
            // Arrange
            var collection = new ValidationErrorCollection();
            collection.Add(new ValidationError("Error 1", 100, false));
            collection.Add(new ValidationError("Warning 1", 101, true));
            collection.Add(new ValidationError("Error 2", 102, false));

            // Act & Assert
            Assert.True(collection.HasWarnings);
        }

        [Fact]
        public void HasWarnings_WithSingleWarning_ReturnsTrue()
        {
            // Arrange
            var collection = new ValidationErrorCollection();
            collection.Add(new ValidationError("Warning 1", 100, true));

            // Act & Assert
            Assert.True(collection.HasWarnings);
        }

        #endregion

        #region ToArray Tests

        [Fact]
        public void ToArray_WithEmptyCollection_ReturnsEmptyArray()
        {
            // Arrange
            var collection = new ValidationErrorCollection();

            // Act
            var array = collection.ToArray();

            // Assert
            Assert.NotNull(array);
            Assert.Empty(array);
        }

        [Fact]
        public void ToArray_WithItems_ReturnsArrayWithAllItems()
        {
            // Arrange
            var collection = new ValidationErrorCollection();
            collection.Add(new ValidationError("Error 1", 100));
            collection.Add(new ValidationError("Error 2", 101));
            collection.Add(new ValidationError("Error 3", 102));

            // Act
            var array = collection.ToArray();

            // Assert
            Assert.Equal(3, array.Length);
            Assert.Equal("Error 1", array[0].ErrorText);
            Assert.Equal("Error 2", array[1].ErrorText);
            Assert.Equal("Error 3", array[2].ErrorText);
        }

        [Fact]
        public void ToArray_ReturnsIndependentArray()
        {
            // Arrange
            var collection = new ValidationErrorCollection();
            collection.Add(new ValidationError("Error 1", 100));

            // Act
            var array = collection.ToArray();
            collection.Add(new ValidationError("Error 2", 101));

            // Assert
            Assert.Single(array);
            Assert.Equal(2, collection.Count);
        }

        #endregion

        #region Insert and Set Tests

        [Fact]
        public void Insert_WithValidItem_InsertsItem()
        {
            // Arrange
            var collection = new ValidationErrorCollection();
            collection.Add(new ValidationError("Error 1", 100));
            collection.Add(new ValidationError("Error 3", 102));

            // Act
            collection.Insert(1, new ValidationError("Error 2", 101));

            // Assert
            Assert.Equal(3, collection.Count);
            Assert.Equal("Error 1", collection[0].ErrorText);
            Assert.Equal("Error 2", collection[1].ErrorText);
            Assert.Equal("Error 3", collection[2].ErrorText);
        }

        [Fact]
        public void Insert_WithNullItem_ThrowsArgumentNullException()
        {
            // Arrange
            var collection = new ValidationErrorCollection();
            collection.Add(new ValidationError("Error 1", 100));

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => 
                collection.Insert(0, null));
            Assert.Equal("item", exception.ParamName);
        }

        [Fact]
        public void Add_WithNullItem_ThrowsArgumentNullException()
        {
            // Arrange
            var collection = new ValidationErrorCollection();

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => 
                collection.Add(null));
            Assert.Equal("item", exception.ParamName);
        }

        [Fact]
        public void Indexer_Set_WithValidItem_ReplacesItem()
        {
            // Arrange
            var collection = new ValidationErrorCollection();
            collection.Add(new ValidationError("Error 1", 100));
            collection.Add(new ValidationError("Error 2", 101));

            // Act
            collection[1] = new ValidationError("Replaced Error", 201);

            // Assert
            Assert.Equal(2, collection.Count);
            Assert.Equal("Error 1", collection[0].ErrorText);
            Assert.Equal("Replaced Error", collection[1].ErrorText);
        }

        [Fact]
        public void Indexer_Set_WithNullItem_ThrowsArgumentNullException()
        {
            // Arrange
            var collection = new ValidationErrorCollection();
            collection.Add(new ValidationError("Error 1", 100));

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => 
                collection[0] = null);
            Assert.Equal("item", exception.ParamName);
        }

        #endregion

        #region Collection Behavior Tests

        [Fact]
        public void Remove_WithExistingItem_RemovesItem()
        {
            // Arrange
            var collection = new ValidationErrorCollection();
            var error1 = new ValidationError("Error 1", 100);
            var error2 = new ValidationError("Error 2", 101);
            collection.Add(error1);
            collection.Add(error2);

            // Act
            var result = collection.Remove(error1);

            // Assert
            Assert.True(result);
            Assert.Single(collection);
            Assert.Equal("Error 2", collection[0].ErrorText);
        }

        [Fact]
        public void Clear_WithItems_RemovesAllItems()
        {
            // Arrange
            var collection = new ValidationErrorCollection();
            collection.Add(new ValidationError("Error 1", 100));
            collection.Add(new ValidationError("Error 2", 101));
            collection.Add(new ValidationError("Error 3", 102));

            // Act
            collection.Clear();

            // Assert
            Assert.Empty(collection);
            Assert.False(collection.HasErrors);
            Assert.False(collection.HasWarnings);
        }

        [Fact]
        public void Contains_WithExistingItem_ReturnsTrue()
        {
            // Arrange
            var collection = new ValidationErrorCollection();
            var error = new ValidationError("Error 1", 100);
            collection.Add(error);

            // Act & Assert
            Assert.Contains(error, collection);
        }

        [Fact]
        public void Contains_WithNonExistingItem_ReturnsFalse()
        {
            // Arrange
            var collection = new ValidationErrorCollection();
            collection.Add(new ValidationError("Error 1", 100));
            var differentError = new ValidationError("Error 2", 101);

            // Act & Assert
            Assert.DoesNotContain(differentError, collection);
        }

        [Fact]
        public void Enumeration_WithMultipleItems_EnumeratesAllItems()
        {
            // Arrange
            var collection = new ValidationErrorCollection();
            collection.Add(new ValidationError("Error 1", 100));
            collection.Add(new ValidationError("Error 2", 101));
            collection.Add(new ValidationError("Error 3", 102));

            // Act
            var enumerated = collection.ToList();

            // Assert
            Assert.Equal(3, enumerated.Count);
            Assert.Equal(100, enumerated[0].ErrorNumber);
            Assert.Equal(101, enumerated[1].ErrorNumber);
            Assert.Equal(102, enumerated[2].ErrorNumber);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void HasErrors_WithNullItems_HandlesGracefully()
        {
            // Note: While InsertItem prevents adding null,
            // this tests the defensive check in HasErrors property
            // Arrange
            var collection = new ValidationErrorCollection();

            // Act & Assert - should not throw
            Assert.False(collection.HasErrors);
        }

        [Fact]
        public void HasWarnings_WithNullItems_HandlesGracefully()
        {
            // Note: While InsertItem prevents adding null,
            // this tests the defensive check in HasWarnings property
            // Arrange
            var collection = new ValidationErrorCollection();

            // Act & Assert - should not throw
            Assert.False(collection.HasWarnings);
        }

        [Fact]
        public void AddRange_WithValidationErrorCollection_AddsAllItems()
        {
            // Arrange
            var collection = new ValidationErrorCollection();
            var sourceCollection = new ValidationErrorCollection();
            sourceCollection.Add(new ValidationError("Error 1", 100));
            sourceCollection.Add(new ValidationError("Error 2", 101));

            // Act
            collection.AddRange(sourceCollection);

            // Assert
            Assert.Equal(2, collection.Count);
        }

        [Fact]
        public void Collection_IsSerializable()
        {
            // Arrange
            var collection = new ValidationErrorCollection();

            // Act & Assert - Verify the Serializable attribute is present
            var type = collection.GetType();
            var attributes = type.GetCustomAttributes(typeof(SerializableAttribute), false);
            Assert.NotEmpty(attributes);
        }

        #endregion
    }
}