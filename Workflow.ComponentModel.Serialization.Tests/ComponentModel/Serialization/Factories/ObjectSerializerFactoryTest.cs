using LogicBuilder.Workflow.ComponentModel.Serialization;
using LogicBuilder.Workflow.ComponentModel.Serialization.Factories;
using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization.Factories
{
    public class ObjectSerializerFactoryTest
    {
        [Fact]
        public void Create_ReturnsNonNullInstance()
        {
            // Act
            var result = ObjectSerializerFactory.Create();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Create_ReturnsIObjectSerializerInstance()
        {
            // Act
            var result = ObjectSerializerFactory.Create();

            // Assert
            Assert.IsAssignableFrom<IObjectSerializer>(result);
        }

        [Fact]
        public void Create_ReturnsObjectSerializerInstance()
        {
            // Act
            var result = ObjectSerializerFactory.Create();

            // Assert
            Assert.IsType<ObjectSerializer>(result);
        }

        [Fact]
        public void Create_ReturnsNewInstanceOnEachCall()
        {
            // Act
            var result1 = ObjectSerializerFactory.Create();
            var result2 = ObjectSerializerFactory.Create();

            // Assert
            Assert.NotSame(result1, result2);
        }
    }
}