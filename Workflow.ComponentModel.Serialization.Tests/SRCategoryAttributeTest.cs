using System;
using System.ComponentModel;
using System.Reflection;

namespace LogicBuilder.Workflow.Tests
{
    public class SRCategoryAttributeTest
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_WithCategory_CreatesInstance()
        {
            // Arrange & Act
            var attribute = new SRCategoryAttribute("TestCategory");

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void Constructor_WithCategoryAndResourceSet_CreatesInstance()
        {
            // Arrange & Act
            var attribute = new SRCategoryAttribute("TestCategory", "TestResourceSet");

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void Constructor_WithNullCategory_DoesNotThrow()
        {
            // Arrange & Act
            var attribute = new SRCategoryAttribute(null);

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void Constructor_WithEmptyCategory_DoesNotThrow()
        {
            // Arrange & Act
            var attribute = new SRCategoryAttribute(string.Empty);

            // Assert
            Assert.NotNull(attribute);
        }

        #endregion

        #region Category Property Tests

        [Fact]
        public void Category_WithValidResourceKey_ReturnsCategory()
        {
            // Arrange
            var attribute = new SRCategoryAttribute("DeletingActivities");

            // Act
            var category = attribute.Category;

            // Assert
            Assert.NotNull(category);
            Assert.NotEqual("DeletingActivities", category); //should be localized
        }

#if !DEBUG
        [Fact]
        public void Category_WithInvalidResourceKey_ReturnsOriginalKey()
        {
            // Arrange
            var attribute = new SRCategoryAttribute("NonExistentKey_12345");

            // Act
            var category = attribute.Category;

            // Assert
            // When resource is not found, it returns the key itself
            Assert.Equal("NonExistentKey_12345", category);
        }
#endif

        [Fact]
        public void Category_CalledMultipleTimes_ReturnsSameValue()
        {
            // Arrange
            var attribute = new SRCategoryAttribute("Activity");

            // Act
            var category1 = attribute.Category;
            var category2 = attribute.Category;

            // Assert
            Assert.Equal(category1, category2);
        }

#endregion

        #region GetLocalizedString Tests via Reflection

        [Fact]
        public void GetLocalizedString_WithEmptyResourceSet_UsesSRGetString()
        {
            // Arrange
            var attribute = new SRCategoryAttribute("Activity");

            // Act
            var result = InvokeGetLocalizedString(attribute, "DeletingActivities");

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual("DeletingActivities", result);
        }

        [Fact]
        public void GetLocalizedString_WithCustomResourceSet_UsesResourceManager()
        {
            // Arrange
            // Using the actual resource set name from the assembly
            var attribute = new SRCategoryAttribute("Activity", "LogicBuilder.Workflow.Resources");

            // Act
            var result = InvokeGetLocalizedString(attribute, "Activity");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetLocalizedString_WithNullValue_HandlesGracefully()
        {
            // Arrange
            var attribute = new SRCategoryAttribute("TestCategory");

            var exception =Assert.Throws<TargetInvocationException>(() => InvokeGetLocalizedString(attribute, null!));

            Assert.IsType<ArgumentNullException>(exception.InnerException);
        }

#if !DEBUG
        [Fact]
        public void GetLocalizedString_WithEmptyValue_HandlesGracefully()
        {
            // Arrange
            var attribute = new SRCategoryAttribute("TestCategory");

            // Act
            var result = InvokeGetLocalizedString(attribute, string.Empty);

            // Assert
            // Just verify it doesn't crash and returns some value
            Assert.Null(result);
        }
#endif 

#endregion

        #region Attribute Usage Tests

        [Fact]
        public void AttributeUsage_SupportsAllTargets()
        {
            // Arrange
            var attributeType = typeof(SRCategoryAttribute);

            // Act
            var usageAttribute = attributeType.GetCustomAttribute<AttributeUsageAttribute>();

            // Assert
            Assert.NotNull(usageAttribute);
            Assert.Equal(AttributeTargets.All, usageAttribute.ValidOn);
        }

        [Fact]
        public void SRCategoryAttribute_IsSealed()
        {
            // Arrange
            var attributeType = typeof(SRCategoryAttribute);

            // Act & Assert
            Assert.True(attributeType.IsSealed);
        }

        [Fact]
        public void SRCategoryAttribute_InheritsFromCategoryAttribute()
        {
            // Arrange
            var attributeType = typeof(SRCategoryAttribute);

            // Act & Assert
            Assert.True(typeof(CategoryAttribute).IsAssignableFrom(attributeType));
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void SRCategoryAttribute_CanBeAppliedToClass()
        {
            // Arrange
            var testType = typeof(TestClassWithCategory);

            // Act
            var attribute = testType.GetCustomAttribute<SRCategoryAttribute>();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void SRCategoryAttribute_CanBeAppliedToProperty()
        {
            // Arrange
            var propertyInfo = typeof(TestClassWithCategory).GetProperty(nameof(TestClassWithCategory.TestProperty));

            // Act
            var attribute = propertyInfo!.GetCustomAttribute<SRCategoryAttribute>();

            // Assert
            Assert.NotNull(attribute);
        }

        [Theory]
        [InlineData("Activity")]
        [InlineData("Handlers")]
        [InlineData("Conditions")]
        [InlineData("Parameters")]
        public void SRCategoryAttribute_WorksWithKnownResourceKeys(string knownKey)
        {
            // Arrange
            var attribute = new SRCategoryAttribute(knownKey);

            // Act
            var category = attribute.Category;

            // Assert
            Assert.NotNull(category);
            Assert.NotEmpty(category);
        }

        #endregion

        #region Helper Methods

        private static string InvokeGetLocalizedString(SRCategoryAttribute attribute, string value)
        {
            var method = typeof(SRCategoryAttribute).GetMethod(
                "GetLocalizedString",
                BindingFlags.NonPublic | BindingFlags.Instance);

            return (string)method!.Invoke(attribute, [value])!;
        }

        #endregion

        #region Test Helper Classes

        [SRCategory("Activity")]
        private class TestClassWithCategory
        {
            [SRCategory("Parameters")]
            public string? TestProperty { get; set; }
        }

        #endregion
    }
}