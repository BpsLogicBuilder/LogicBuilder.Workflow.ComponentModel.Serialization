using LogicBuilder.ComponentModel.Design.Serialization;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using System;
using System.CodeDom;
using Xunit;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization
{
    public class CodeTypeReferenceSerializerTest
    {
        private readonly CodeTypeReferenceSerializer _serializer;
        private readonly WorkflowMarkupSerializationManager _serializationManager;

        public CodeTypeReferenceSerializerTest()
        {
            _serializer = new CodeTypeReferenceSerializer();
            _serializationManager = new WorkflowMarkupSerializationManager(new DesignerSerializationManager());
        }

        #region CanSerializeToString Tests

        [Fact]
        public void CanSerializeToString_ReturnsTrue_WhenValueIsCodeTypeReference()
        {
            // Arrange
            var codeTypeRef = new CodeTypeReference(typeof(string));

            // Act
            bool result = _serializer.CanSerializeToString(_serializationManager, codeTypeRef);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanSerializeToString_ReturnsFalse_WhenValueIsString()
        {
            // Arrange
            var value = "not a CodeTypeReference";

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

        #endregion

        #region SerializeToString Tests

        [Fact]
        public void SerializeToString_ThrowsArgumentNullException_WhenSerializationManagerIsNull()
        {
            // Arrange
            var codeTypeRef = new CodeTypeReference(typeof(string));

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _serializer.SerializeToString(null, codeTypeRef));
        }

        [Fact]
        public void SerializeToString_ThrowsArgumentNullException_WhenValueIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _serializer.SerializeToString(_serializationManager, null));
        }

        [Fact]
        public void SerializeToString_ReturnsEmptyString_WhenValueIsNotCodeTypeReference()
        {
            // Arrange
            var value = "not a CodeTypeReference";

            // Act
            string result = _serializer.SerializeToString(_serializationManager, value);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void SerializeToString_ReturnsAssemblyQualifiedName_ForSimpleType()
        {
            // Arrange
            var codeTypeRef = new CodeTypeReference(typeof(string));

            // Act
            string result = _serializer.SerializeToString(_serializationManager, codeTypeRef);

            // Assert
            Assert.Contains("System.String", result);
            Assert.Contains("System.Private.CoreLib", result);
        }

        [Fact]
        public void SerializeToString_ReturnsAssemblyQualifiedName_ForIntType()
        {
            // Arrange
            var codeTypeRef = new CodeTypeReference(typeof(int));

            // Act
            string result = _serializer.SerializeToString(_serializationManager, codeTypeRef);

            // Assert
            Assert.Contains("System.Int32", result);
            Assert.Contains("System.Private.CoreLib", result);
        }

        [Fact]
        public void SerializeToString_HandlesArrayType_Correctly()
        {
            // Arrange
            var codeTypeRef = new CodeTypeReference(typeof(string[]));

            // Act
            string result = _serializer.SerializeToString(_serializationManager, codeTypeRef);

            // Assert
            Assert.Contains("System.String[]", result);
        }

        [Fact]
        public void SerializeToString_HandlesMultiDimensionalArray_Correctly()
        {
            // Arrange
            var codeTypeRef = new CodeTypeReference(typeof(string[,]));

            // Act
            string result = _serializer.SerializeToString(_serializationManager, codeTypeRef);

            // Assert
            Assert.Contains("System.String[,]", result);
        }

        [Fact]
        public void SerializeToString_HandlesGenericType_Correctly()
        {
            // Arrange
            var codeTypeRef = new CodeTypeReference(typeof(System.Collections.Generic.List<string>));

            // Act
            string result = _serializer.SerializeToString(_serializationManager, codeTypeRef);

            // Assert
            Assert.Contains("System.Collections.Generic.List", result);
            Assert.Contains("System.String", result);
        }

        [Fact]
        public void SerializeToString_ReturnsTypeName_WhenTypeCannotBeResolved()
        {
            // Arrange
            var codeTypeRef = new CodeTypeReference("NonExistent.Type.Name");

            // Act
            string result = _serializer.SerializeToString(_serializationManager, codeTypeRef);

            // Assert
            Assert.Equal("NonExistent.Type.Name", result);
        }

        #endregion

        #region DeserializeFromString Tests

        [Fact]
        public void DeserializeFromString_ReturnsNull_WhenPropertyTypeIsNotCompatible()
        {
            // Arrange
            var propertyType = typeof(string);
            var value = "System.String";

            // Act
            var result = _serializer.DeserializeFromString(_serializationManager, propertyType, value);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void DeserializeFromString_ReturnsNull_WhenValueIsEmpty()
        {
            // Arrange
            var propertyType = typeof(CodeTypeReference);
            var value = string.Empty;

            // Act
            var result = _serializer.DeserializeFromString(_serializationManager, propertyType, value);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void DeserializeFromString_ReturnsNull_WhenValueIsNull()
        {
            // Arrange
            var propertyType = typeof(CodeTypeReference);
            string value = null!;

            // Act
            var result = _serializer.DeserializeFromString(_serializationManager, propertyType, value);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void DeserializeFromString_ReturnsCodeTypeReference_ForSimpleType()
        {
            // Arrange
            var propertyType = typeof(CodeTypeReference);
            var value = "System.String, System.Private.CoreLib";

            // Act
            var result = _serializer.DeserializeFromString(_serializationManager, propertyType, value) as CodeTypeReference;

            // Assert
            Assert.NotNull(result);
            Assert.Contains("System.String", result.BaseType);
        }

        [Fact]
        public void DeserializeFromString_SetsQualifiedNameInUserData()
        {
            // Arrange
            var propertyType = typeof(CodeTypeReference);
            var value = typeof(string).AssemblyQualifiedName;

            // Act
            var result = _serializer.DeserializeFromString(_serializationManager, propertyType, value) as CodeTypeReference;

            // Assert
            Assert.NotNull(result);
            Assert.True(result.UserData.Contains(CodeTypeReferenceSerializer.QualifiedName));
            Assert.Equal(typeof(string).AssemblyQualifiedName, result.UserData[CodeTypeReferenceSerializer.QualifiedName]);
        }

        [Fact]
        public void DeserializeFromString_HandlesUnknownType_Gracefully()
        {
            // Arrange
            var propertyType = typeof(CodeTypeReference);
            var value = "UnknownType.DoesNotExist";

            // Act
            var result = _serializer.DeserializeFromString(_serializationManager, propertyType, value) as CodeTypeReference;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("UnknownType.DoesNotExist", result.UserData[CodeTypeReferenceSerializer.QualifiedName]);
        }

        [Fact]
        public void DeserializeFromString_HandlesArrayType()
        {
            // Arrange
            var propertyType = typeof(CodeTypeReference);
            var value = typeof(string[]).AssemblyQualifiedName;

            // Act
            var result = _serializer.DeserializeFromString(_serializationManager, propertyType, value) as CodeTypeReference;

            // Assert
            Assert.NotNull(result);
            Assert.Contains("System.String[]", result.UserData[CodeTypeReferenceSerializer.QualifiedName]!.ToString());
        }

        [Fact]
        public void DeserializeFromString_HandlesGenericType()
        {
            // Arrange
            var propertyType = typeof(CodeTypeReference);
            var value = typeof(System.Collections.Generic.List<int>).AssemblyQualifiedName;

            // Act
            var result = _serializer.DeserializeFromString(_serializationManager, propertyType, value) as CodeTypeReference;

            // Assert
            Assert.NotNull(result);
            Assert.True(result.UserData.Contains(CodeTypeReferenceSerializer.QualifiedName));
        }

        #endregion

        #region RoundTrip Tests

        [Fact]
        public void RoundTrip_SerializeAndDeserialize_SimpleType()
        {
            // Arrange
            var originalType = typeof(int);
            var codeTypeRef = new CodeTypeReference(originalType);

            // Act
            string serialized = _serializer.SerializeToString(_serializationManager, codeTypeRef);
            var deserialized = _serializer.DeserializeFromString(_serializationManager, typeof(CodeTypeReference), serialized) as CodeTypeReference;

            // Assert
            Assert.NotNull(deserialized);
            Assert.Equal("System.Int32", deserialized.BaseType);
        }

        [Fact]
        public void RoundTrip_SerializeAndDeserialize_GenericType()
        {
            // Arrange
            var originalType = typeof(System.Collections.Generic.List<string>);
            var codeTypeRef = new CodeTypeReference(originalType);

            // Act
            string serialized = _serializer.SerializeToString(_serializationManager, codeTypeRef);
            var deserialized = _serializer.DeserializeFromString(_serializationManager, typeof(CodeTypeReference), serialized) as CodeTypeReference;

            // Assert
            Assert.NotNull(deserialized);
            Assert.Contains("List", deserialized.BaseType);
        }

        [Fact]
        public void RoundTrip_SerializeAndDeserialize_ArrayType()
        {
            // Arrange
            var originalType = typeof(double[]);
            var codeTypeRef = new CodeTypeReference(originalType);

            // Act
            string serialized = _serializer.SerializeToString(_serializationManager, codeTypeRef);
            var deserialized = _serializer.DeserializeFromString(_serializationManager, typeof(CodeTypeReference), serialized) as CodeTypeReference;

            // Assert
            Assert.NotNull(deserialized);
            Assert.Contains("Double[]", deserialized.UserData[CodeTypeReferenceSerializer.QualifiedName]!.ToString());
        }

        #endregion

        #region QualifiedName Constant Test

        [Fact]
        public void QualifiedName_HasExpectedValue()
        {
            // Assert
            Assert.Equal("QualifiedName", CodeTypeReferenceSerializer.QualifiedName);
        }

        #endregion
    }
}