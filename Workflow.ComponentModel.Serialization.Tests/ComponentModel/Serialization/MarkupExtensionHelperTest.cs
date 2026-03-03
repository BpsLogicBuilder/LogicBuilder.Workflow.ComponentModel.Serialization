using LogicBuilder.ComponentModel.Design.Serialization;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using LogicBuilder.Workflow.ComponentModel.Serialization.Factories;
using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;
using System;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization
{
    public class MarkupExtensionHelperTest
    {
        private readonly IMarkupExtensionHelper _markupExtensionHelper;
        private readonly WorkflowMarkupSerializationManager _serializationManager;

        public MarkupExtensionHelperTest()
        {
            _markupExtensionHelper = MarkupExtensionHelperFactory.Create();
            _serializationManager = new WorkflowMarkupSerializationManager(new DesignerSerializationManager());
        }

        #region GetValueFromMarkupExtension Tests

        [Fact]
        public void GetValueFromMarkupExtension_ReturnsNull_WhenExtensionIsNull()
        {
            // Act
            var result = _markupExtensionHelper.GetValueFromMarkupExtension(_serializationManager, null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetValueFromMarkupExtension_ReturnsOriginalValue_WhenExtensionIsNotMarkupExtension()
        {
            // Arrange
            var extension = "test string";

            // Act
            var result = _markupExtensionHelper.GetValueFromMarkupExtension(_serializationManager, extension);

            // Assert
            Assert.Equal(extension, result);
        }

        [Fact]
        public void GetValueFromMarkupExtension_ReturnsOriginalValue_WhenExtensionIsInt()
        {
            // Arrange
            var extension = 42;

            // Act
            var result = _markupExtensionHelper.GetValueFromMarkupExtension(_serializationManager, extension);

            // Assert
            Assert.Equal(extension, result);
        }

        [Fact]
        public void GetValueFromMarkupExtension_ReturnsOriginalValue_WhenExtensionIsObject()
        {
            // Arrange
            var extension = new object();

            // Act
            var result = _markupExtensionHelper.GetValueFromMarkupExtension(_serializationManager, extension);

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
            var result = _markupExtensionHelper.GetValueFromMarkupExtension(_serializationManager, extension);

            // Assert
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void GetValueFromMarkupExtension_ReturnsProvideValueResult_WhenMarkupExtensionReturnsNull()
        {
            // Arrange
            var extension = new TestMarkupExtension(null!);

            // Act
            var result = _markupExtensionHelper.GetValueFromMarkupExtension(_serializationManager, extension);

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
            var result = _markupExtensionHelper.GetValueFromMarkupExtension(_serializationManager, extension);

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
            _markupExtensionHelper.GetValueFromMarkupExtension(_serializationManager, extension);

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
            var result = _markupExtensionHelper.GetValueFromMarkupExtension(_serializationManager, extension);

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
            var result = _markupExtensionHelper.GetValueFromMarkupExtension(_serializationManager, outerExtension);

            // Assert
            // The helper doesn't recursively resolve, so it returns the inner extension
            Assert.IsType<TestMarkupExtension>(result);
        }

        #endregion

        #region GetMarkupExtensionFromValue Tests

        [Fact]
        public void GetMarkupExtensionFromValue_ReturnsNullExtension_WhenValueIsNull()
        {
            // Arrange
            object? value = null;

            // Act
            var result = _markupExtensionHelper.GetMarkupExtensionFromValue(value);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NullExtension>(result);
        }

        [Fact]
        public void GetMarkupExtensionFromValue_ReturnsTypeExtension_WhenValueIsType()
        {
            // Arrange
            Type value = typeof(string);

            // Act
            var result = _markupExtensionHelper.GetMarkupExtensionFromValue(value);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TypeExtension>(result);
            TypeExtension typeExtension = (TypeExtension)result;
            Assert.Equal(typeof(string), typeExtension.Type);
        }

        [Fact]
        public void GetMarkupExtensionFromValue_ReturnsTypeExtension_WhenValueIsCustomType()
        {
            // Arrange
            Type value = typeof(MarkupExtensionHelperTest);

            // Act
            var result = _markupExtensionHelper.GetMarkupExtensionFromValue(value);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TypeExtension>(result);
            TypeExtension typeExtension = (TypeExtension)result;
            Assert.Equal(typeof(MarkupExtensionHelperTest), typeExtension.Type);
        }

        [Fact]
        public void GetMarkupExtensionFromValue_ReturnsArrayExtension_WhenValueIsArray()
        {
            // Arrange
            int[] value = [1, 2, 3, 4, 5];

            // Act
            var result = _markupExtensionHelper.GetMarkupExtensionFromValue(value);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ArrayExtension>(result);
            ArrayExtension arrayExtension = (ArrayExtension)result;
            Assert.Equal(typeof(int), arrayExtension.Type);
            Assert.Equal(5, arrayExtension.Items.Count);
        }

        [Fact]
        public void GetMarkupExtensionFromValue_ReturnsArrayExtension_WhenValueIsStringArray()
        {
            // Arrange
            string[] value = ["one", "two", "three"];

            // Act
            var result = _markupExtensionHelper.GetMarkupExtensionFromValue(value);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ArrayExtension>(result);
            ArrayExtension arrayExtension = (ArrayExtension)result;
            Assert.Equal(typeof(string), arrayExtension.Type);
            Assert.Equal(3, arrayExtension.Items.Count);
        }

        [Fact]
        public void GetMarkupExtensionFromValue_ReturnsArrayExtension_WhenValueIsEmptyArray()
        {
            // Arrange
            int[] value = [];

            // Act
            var result = _markupExtensionHelper.GetMarkupExtensionFromValue(value);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ArrayExtension>(result);
            ArrayExtension arrayExtension = (ArrayExtension)result;
            Assert.Equal(typeof(int), arrayExtension.Type);
            Assert.Empty(arrayExtension.Items);
        }

        [Fact]
        public void GetMarkupExtensionFromValue_ReturnsValueAsIs_WhenValueIsString()
        {
            // Arrange
            string value = "test string";

            // Act
            var result = _markupExtensionHelper.GetMarkupExtensionFromValue(value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(value, result);
            Assert.IsType<string>(result);
        }

        [Fact]
        public void GetMarkupExtensionFromValue_ReturnsValueAsIs_WhenValueIsInteger()
        {
            // Arrange
            int value = 42;

            // Act
            var result = _markupExtensionHelper.GetMarkupExtensionFromValue(value);

            // Assert
            Assert.Equal(value, result);
            Assert.IsType<int>(result);
        }

        [Fact]
        public void GetMarkupExtensionFromValue_ReturnsValueAsIs_WhenValueIsCustomObject()
        {
            // Arrange
            var value = new TestClass { Name = "Test", Value = 123 };

            // Act
            var result = _markupExtensionHelper.GetMarkupExtensionFromValue(value);

            // Assert
            Assert.NotNull(result);
            Assert.Same(value, result);
            Assert.IsType<TestClass>(result);
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

        private class TestClass
        {
            public string? Name { get; set; }
            public int Value { get; set; }
        }

        #endregion
    }
}