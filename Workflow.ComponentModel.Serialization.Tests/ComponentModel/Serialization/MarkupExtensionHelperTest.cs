using LogicBuilder.ComponentModel.Design.Serialization;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using System;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization
{
    public class MarkupExtensionHelperTest
    {
        private readonly MarkupExtensionHelper _helper;
        private readonly WorkflowMarkupSerializationManager _serializationManager;

        public MarkupExtensionHelperTest()
        {
            _helper = new MarkupExtensionHelper();
            _serializationManager = new WorkflowMarkupSerializationManager(new DesignerSerializationManager());
        }

        #region GetValueFromMarkupExtension Tests

        [Fact]
        public void GetValueFromMarkupExtension_ReturnsNull_WhenExtensionIsNull()
        {
            // Act
            var result = _helper.GetValueFromMarkupExtension(_serializationManager, null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetValueFromMarkupExtension_ReturnsOriginalValue_WhenExtensionIsNotMarkupExtension()
        {
            // Arrange
            var extension = "test string";

            // Act
            var result = _helper.GetValueFromMarkupExtension(_serializationManager, extension);

            // Assert
            Assert.Equal(extension, result);
        }

        [Fact]
        public void GetValueFromMarkupExtension_ReturnsOriginalValue_WhenExtensionIsInt()
        {
            // Arrange
            var extension = 42;

            // Act
            var result = _helper.GetValueFromMarkupExtension(_serializationManager, extension);

            // Assert
            Assert.Equal(extension, result);
        }

        [Fact]
        public void GetValueFromMarkupExtension_ReturnsOriginalValue_WhenExtensionIsObject()
        {
            // Arrange
            var extension = new object();

            // Act
            var result = _helper.GetValueFromMarkupExtension(_serializationManager, extension);

            // Assert
            Assert.Same(extension, result);
        }

        [Fact]
        public void GetValueFromMarkupExtension_CallsProvideValue_WhenExtensionIsMarkupExtension()
        {
            // Arrange
            var expectedValue = "provided value";
            var extension = new TestMarkupExtension(expectedValue);

            // Act
            var result = _helper.GetValueFromMarkupExtension(_serializationManager, extension);

            // Assert
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void GetValueFromMarkupExtension_ReturnsProvideValueResult_WhenMarkupExtensionReturnsNull()
        {
            // Arrange
            var extension = new TestMarkupExtension(null!);

            // Act
            var result = _helper.GetValueFromMarkupExtension(_serializationManager, extension);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetValueFromMarkupExtension_ReturnsProvideValueResult_WhenMarkupExtensionReturnsComplexObject()
        {
            // Arrange
            var expectedValue = new { Property = "value", Number = 123 };
            var extension = new TestMarkupExtension(expectedValue);

            // Act
            var result = _helper.GetValueFromMarkupExtension(_serializationManager, extension);

            // Assert
            Assert.Same(expectedValue, result);
        }

        [Fact]
        public void GetValueFromMarkupExtension_PassesSerializationManager_ToProvideValue()
        {
            // Arrange
            IServiceProvider? capturedProvider = null;
            var extension = new TestMarkupExtensionWithCallback(provider =>
            {
                capturedProvider = provider;
                return "result";
            });

            // Act
            _helper.GetValueFromMarkupExtension(_serializationManager, extension);

            // Assert
            Assert.NotNull(capturedProvider);
            Assert.Same(_serializationManager, capturedProvider);
        }

        [Fact]
        public void GetValueFromMarkupExtension_ReturnsMarkupExtensionItself_WhenProvideValueReturnsThis()
        {
            // Arrange
            var extension = new TestMarkupExtensionReturningSelf();

            // Act
            var result = _helper.GetValueFromMarkupExtension(_serializationManager, extension);

            // Assert
            Assert.Same(extension, result);
        }

        [Fact]
        public void GetValueFromMarkupExtension_HandlesNestedMarkupExtension()
        {
            // Arrange
            var innerValue = "final value";
            var innerExtension = new TestMarkupExtension(innerValue);
            var outerExtension = new TestMarkupExtension(innerExtension);

            // Act
            var result = _helper.GetValueFromMarkupExtension(_serializationManager, outerExtension);

            // Assert
            // The helper doesn't recursively resolve, so it returns the inner extension
            Assert.IsType<TestMarkupExtension>(result);
        }

        #endregion

        #region Test Helper Classes

        // Test markup extension that returns a specified value
        private class TestMarkupExtension(object value) : MarkupExtension
        {
            private readonly object _value = value;

            public override object ProvideValue(IServiceProvider provider)
            {
                return _value;
            }
        }

        // Test markup extension that returns itself
        private class TestMarkupExtensionReturningSelf : MarkupExtension
        {
            public override object ProvideValue(IServiceProvider provider)
            {
                return this;
            }
        }

        // Test markup extension with callback to verify provider
        private class TestMarkupExtensionWithCallback(Func<IServiceProvider, object> callback) : MarkupExtension
        {
            private readonly Func<IServiceProvider, object> _callback = callback;

            public override object ProvideValue(IServiceProvider provider)
            {
                return _callback(provider);
            }
        }

        #endregion
    }
}