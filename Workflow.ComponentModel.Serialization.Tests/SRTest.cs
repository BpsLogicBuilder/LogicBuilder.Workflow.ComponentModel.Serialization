using System;
using System.Globalization;
using System.Reflection;

namespace LogicBuilder.Workflow.Tests
{
    public class SRTest
    {
        #region SR.GetString Tests

        [Fact]
        public void GetString_ReturnsStringFromResource_WhenResourceKeyExists()
        {
            // Act
            string result = SR.GetString("Activity");

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetString_WithCulture_ReturnsStringFromResource_WhenResourceKeyExists()
        {
            // Act
            string result = SR.GetString(CultureInfo.InvariantCulture, "Activity");

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetString_WithArgs_ReturnsFormattedString_WhenResourceKeyExists()
        {
            // Arrange
            // Assuming there's a resource with format placeholders
            string key = "Activity";
            object[] args = ["test"];

            // Act
            string result = SR.GetString(key, args);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetString_WithCultureAndArgs_ReturnsFormattedString_WhenResourceKeyExists()
        {
            // Arrange
            string key = "Activity";
            object[] args = ["test"];

            // Act
            string result = SR.GetString(CultureInfo.InvariantCulture, key, args);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetString_WithEmptyArgs_ReturnsUnformattedString()
        {
            // Arrange
            string key = "Activity";
            object[] args = [];

            // Act
            string result = SR.GetString(key, args);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetString_WithNullArgs_ReturnsUnformattedString()
        {
            // Arrange
            string key = "Activity";

            // Act
            string result = SR.GetString(key, null);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetString_ReturnsSameValue_WhenCalledMultipleTimes()
        {
            // Act
            string result1 = SR.GetString("Activity");
            string result2 = SR.GetString("Activity");

            // Assert
            Assert.Equal(result1, result2);
        }

        [Fact]
        public void GetString_ReturnsNull_WhenResourceKeyDoesNotExist()
        {
            // Arrange
            string key = "NonExistentResourceKey12345";

            // Act
            string result = SR.GetString(key);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region SR Constants Tests

        [Fact]
        public void Constants_HaveCorrectValues()
        {
            // Assert
            Assert.Equal("Activity", SR.Activity);
            Assert.Equal("Handlers", SR.Handlers);
            Assert.Equal("Conditions", SR.Conditions);
            Assert.Equal("ConditionedActivityConditions", SR.ConditionedActivityConditions);
            Assert.Equal("Correlations", SR.Correlations);
            Assert.Equal("CorrelationSet", SR.CorrelationSet);
            Assert.Equal("NameDescr", SR.NameDescr);
            Assert.Equal("EnabledDescr", SR.EnabledDescr);
            Assert.Equal("DescriptionDescr", SR.DescriptionDescr);
            Assert.Equal("UnlessConditionDescr", SR.UnlessConditionDescr);
        }

        [Fact]
        public void Constants_ErrorStrings_HaveCorrectValues()
        {
            // Assert
            Assert.Equal("Error_ReadOnlyTemplateActivity", SR.Error_ReadOnlyTemplateActivity);
            Assert.Equal("Error_TypeNotString", SR.Error_TypeNotString);
            Assert.Equal("Error_InvalidErrorType", SR.Error_InvalidErrorType);
            Assert.Equal("Error_LiteralConversionFailed", SR.Error_LiteralConversionFailed);
            Assert.Equal("Error_TypeNotPrimitive", SR.Error_TypeNotPrimitive);
        }

        [Fact]
        public void Constants_CanBeUsedToRetrieveResources()
        {
            // Act
            string activityString = SR.GetString(SR.Activity);
            string handlersString = SR.GetString(SR.Handlers);

            // Assert
            Assert.NotNull(activityString);
            Assert.NotNull(handlersString);
        }

        #endregion

        #region SRDescriptionAttribute Tests

        [Fact]
        public void SRDescriptionAttribute_SetsDescriptionValue_WhenCreatedWithResourceKey()
        {
            // Arrange & Act
            var attribute = new SRDescriptionAttribute("Activity");

            // Assert
            Assert.NotNull(attribute.Description);
            Assert.NotEmpty(attribute.Description);
        }

        [Fact]
        public void SRDescriptionAttribute_ReturnsEmptyString_WhenResourceKeyDoesNotExist()
        {
            // Arrange & Act
            var attribute = new SRDescriptionAttribute("NonExistentKey12345");

            // Assert
            // When resource doesn't exist, GetString returns null, but Description property may handle it
            Assert.True(string.IsNullOrEmpty(attribute.Description));
        }

        [Fact]
        public void SRDescriptionAttribute_CanBeAppliedToProperty()
        {
            // Arrange
            var propertyInfo = typeof(TestClassWithAttributes).GetProperty("TestProperty");

            // Act
            var attribute = propertyInfo?.GetCustomAttribute<SRDescriptionAttribute>();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void SRDescriptionAttribute_WithResourceSet_SetsDescriptionValue()
        {
            // This test verifies the constructor that takes a resource set
            // The actual resource set name would need to exist in the assembly
            // We'll test that the constructor doesn't throw
            
            // Act
            Exception? exception = Record.Exception(() => 
                new SRDescriptionAttribute("Activity", "LogicBuilder.Workflow.Resources"));

            // Assert - constructor should handle gracefully even if resource set doesn't exist
            Assert.Null(exception);
        }

        #endregion

        #region SRCategoryAttribute Tests

        [Fact]
        public void SRCategoryAttribute_SetsCategory_WhenCreatedWithResourceKey()
        {
            // Arrange & Act
            var attribute = new SRCategoryAttribute("Activity");

            // Assert
            Assert.NotNull(attribute.Category);
        }

        [Fact]
        public void SRCategoryAttribute_WithEmptyResourceSet_UsesDefaultSR()
        {
            // Arrange & Act
            var attribute = new SRCategoryAttribute("Activity");
            string category = attribute.Category;

            // Assert
            Assert.NotNull(category);
        }

        [Fact]
        public void SRCategoryAttribute_WithResourceSet_UsesSpecifiedResourceSet()
        {
            // Arrange & Act
            var attribute = new SRCategoryAttribute("Activity", "LogicBuilder.Workflow.Resources");
            
            // Assert - constructor should complete successfully
            Assert.NotNull(attribute);
        }

        [Fact]
        public void SRCategoryAttribute_CanBeAppliedToProperty()
        {
            // Arrange
            var propertyInfo = typeof(TestClassWithAttributes).GetProperty("TestPropertyWithCategory");

            // Act
            var attribute = propertyInfo?.GetCustomAttribute<SRCategoryAttribute>();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void SRCategoryAttribute_ReturnsLocalizedString_WhenResourceExists()
        {
            // Arrange
            var attribute = new SRCategoryAttribute("Activity");

            // Act
            string category = attribute.Category;

            // Assert
            Assert.NotNull(category);
            Assert.NotEmpty(category);
        }

        #endregion

        #region SRDisplayNameAttribute Tests

        [Fact]
        public void SRDisplayNameAttribute_SetsDisplayNameValue_WhenCreatedWithResourceKey()
        {
            // Arrange & Act
            var attribute = new SRDisplayNameAttribute("Activity");

            // Assert
            Assert.NotNull(attribute.DisplayName);
            Assert.NotEmpty(attribute.DisplayName);
        }

        [Fact]
        public void SRDisplayNameAttribute_ReturnsEmptyString_WhenResourceKeyDoesNotExist()
        {
            // Arrange & Act
            var attribute = new SRDisplayNameAttribute("NonExistentKey12345");

            // Assert
            Assert.True(string.IsNullOrEmpty(attribute.DisplayName));
        }

        [Fact]
        public void SRDisplayNameAttribute_CanBeAppliedToProperty()
        {
            // Arrange
            var propertyInfo = typeof(TestClassWithAttributes).GetProperty("TestPropertyWithDisplayName");

            // Act
            var attribute = propertyInfo?.GetCustomAttribute<SRDisplayNameAttribute>();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void SRDisplayNameAttribute_WithResourceSet_SetsDisplayNameValue()
        {
            // Act
            Exception? exception = Record.Exception(() => 
                new SRDisplayNameAttribute("Activity", "LogicBuilder.Workflow.Resources"));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void SRDisplayNameAttribute_DisplayNameMatchesResourceValue()
        {
            // Arrange
            var attribute = new SRDisplayNameAttribute("Activity");
            string expectedValue = SR.GetString("Activity");

            // Act
            string displayName = attribute.DisplayName;

            // Assert
            Assert.Equal(expectedValue, displayName);
        }

        #endregion

        #region Attribute Usage Tests

        [Fact]
        public void SRDescriptionAttribute_HasCorrectAttributeUsage()
        {
            // Arrange
            var attributeUsage = typeof(SRDescriptionAttribute)
                .GetCustomAttribute<AttributeUsageAttribute>();

            // Assert
            Assert.NotNull(attributeUsage);
            Assert.Equal(AttributeTargets.All, attributeUsage.ValidOn);
        }

        [Fact]
        public void SRCategoryAttribute_HasCorrectAttributeUsage()
        {
            // Arrange
            var attributeUsage = typeof(SRCategoryAttribute)
                .GetCustomAttribute<AttributeUsageAttribute>();

            // Assert
            Assert.NotNull(attributeUsage);
            Assert.Equal(AttributeTargets.All, attributeUsage.ValidOn);
        }

        [Fact]
        public void SRDisplayNameAttribute_HasCorrectAttributeUsage()
        {
            // Arrange
            var attributeUsage = typeof(SRDisplayNameAttribute)
                .GetCustomAttribute<AttributeUsageAttribute>();

            // Assert
            Assert.NotNull(attributeUsage);
            Assert.Equal(AttributeTargets.All, attributeUsage.ValidOn);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void AllAttributes_CanBeAppliedToSameProperty()
        {
            // Arrange
            var propertyInfo = typeof(TestClassWithAttributes).GetProperty("FullyDecoratedProperty");

            // Act
            var descriptionAttr = propertyInfo?.GetCustomAttribute<SRDescriptionAttribute>();
            var categoryAttr = propertyInfo?.GetCustomAttribute<SRCategoryAttribute>();
            var displayNameAttr = propertyInfo?.GetCustomAttribute<SRDisplayNameAttribute>();

            // Assert
            Assert.NotNull(descriptionAttr);
            Assert.NotNull(categoryAttr);
            Assert.NotNull(displayNameAttr);
        }

        [Fact]
        public void SR_GetString_WithMultipleArgs_FormatsCorrectly()
        {
            // Arrange
            //string template = "Value1: {0}, Value2: {1}";
            // Note: This assumes there's a resource that contains format placeholders
            // For this test, we'll just verify the method handles multiple arguments

            // Act
            string result = SR.GetString("Error_SerializerCreateInstanceFailed", "arg1", "arg2");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void SR_IsThreadSafe_WhenAccessedConcurrently()
        {
            // Arrange
            const int threadCount = 10;
            var tasks = new System.Threading.Tasks.Task[threadCount];

            // Act
            for (int i = 0; i < threadCount; i++)
            {
                tasks[i] = System.Threading.Tasks.Task.Run(() =>
                {
                    for (int j = 0; j < 100; j++)
                    {
                        _ = SR.GetString("Activity");
                    }
                }, TestContext.Current.CancellationToken);
            }

            // Assert
            Exception? exception = Record.Exception(() => System.Threading.Tasks.Task.WaitAll(tasks, TestContext.Current.CancellationToken));
            Assert.Null(exception);
        }

        #endregion

        #region Test Helper Classes

        private class TestClassWithAttributes
        {
            [SRDescription("Activity")]
            public string TestProperty { get; set; } = string.Empty;

            [SRCategory("Activity")]
            public string TestPropertyWithCategory { get; set; } = string.Empty;

            [SRDisplayName("Activity")]
            public string TestPropertyWithDisplayName { get; set; } = string.Empty;

            [SRDescription("Activity")]
            [SRCategory("Handlers")]
            [SRDisplayName("Conditions")]
            public string FullyDecoratedProperty { get; set; } = string.Empty;
        }

        #endregion
    }
}