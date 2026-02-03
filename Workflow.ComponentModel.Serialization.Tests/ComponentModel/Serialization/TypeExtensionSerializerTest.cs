using LogicBuilder.ComponentModel.Design.Serialization;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using System;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Reflection;
using Xunit;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization
{
    public class TypeExtensionSerializerTest
    {
        private readonly TypeExtensionSerializer _serializer;
        private readonly WorkflowMarkupSerializationManager _serializationManager;

        public TypeExtensionSerializerTest()
        {
            _serializer = new TypeExtensionSerializer();
            _serializationManager = new WorkflowMarkupSerializationManager(new DesignerSerializationManager());
        }

        [Fact]
        public void GetInstanceDescriptor_ThrowsArgumentException_WhenValueIsNotTypeExtension()
        {
            // Arrange
            var value = "not a type extension";

            // Act & Assert
            var ex = Assert.Throws<System.Reflection.TargetInvocationException>(() => 
                _serializer.GetType()
                    .GetMethod("GetInstanceDescriptor", BindingFlags.NonPublic | BindingFlags.Instance)!
                    .Invoke(_serializer, [_serializationManager, value])
            );

            Assert.IsType<ArgumentException>(ex.InnerException);
            Assert.Contains("TypeExtension", ex.InnerException.Message);
        }

        [Fact]
        public void GetInstanceDescriptor_ReturnsInstanceDescriptor_WithTypeConstructor()
        {
            // Arrange
            var type = typeof(int);
            var typeExtension = new TypeExtension(type);

            // Act
            var result = (InstanceDescriptor)_serializer
                .GetType()
                .GetMethod("GetInstanceDescriptor", BindingFlags.NonPublic | BindingFlags.Instance)!
                .Invoke(_serializer, [_serializationManager, typeExtension])!;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<InstanceDescriptor>(result);
            var ctor = result.MemberInfo as ConstructorInfo;
            Assert.NotNull(ctor);
            Assert.Equal([typeof(Type)], Array.ConvertAll(ctor.GetParameters(), p => p.ParameterType));
            Assert.Single(result.Arguments);
            Assert.Equal(type, result.Arguments.OfType<Type>().First());
        }

        [Fact]
        public void GetInstanceDescriptor_ReturnsInstanceDescriptor_WithStringConstructor()
        {
            // Arrange
            var typeName = "System.String";
            var typeExtension = new TypeExtension(typeName);

            // Act
            var result = (InstanceDescriptor)_serializer
                .GetType()
                .GetMethod("GetInstanceDescriptor", BindingFlags.NonPublic | BindingFlags.Instance)!
                .Invoke(_serializer, [_serializationManager, typeExtension])!;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<InstanceDescriptor>(result);
            var ctor = result.MemberInfo as ConstructorInfo;
            Assert.NotNull(ctor);
            Assert.Equal([typeof(string)], Array.ConvertAll(ctor.GetParameters(), p => p.ParameterType));
            Assert.Single(result.Arguments);
            Assert.Equal(typeName, result.Arguments.OfType<string>().First());
        }
    }
}