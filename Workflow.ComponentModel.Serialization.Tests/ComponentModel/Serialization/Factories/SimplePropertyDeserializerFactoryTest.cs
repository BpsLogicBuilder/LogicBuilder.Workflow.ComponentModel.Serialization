using LogicBuilder.Workflow.ComponentModel.Serialization.Factories;
using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization.Factories
{
    public class SimplePropertyDeserializerFactoryTest
    {
        [Fact]
        public void Create_ReturnsSimplePropertyDeserializerInstance()
        {
            // Act
            ISimplePropertyDeserializer result = SimplePropertyDeserializerFactory.Create();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Create_ReturnsISimplePropertyDeserializerType()
        {
            // Act
            ISimplePropertyDeserializer result = SimplePropertyDeserializerFactory.Create();

            // Assert
            Assert.IsType<ISimplePropertyDeserializer>(result, exactMatch: false);
        }

        [Fact]
        public void Create_ReturnsNewInstanceEachTime()
        {
            // Act
            ISimplePropertyDeserializer result1 = SimplePropertyDeserializerFactory.Create();
            ISimplePropertyDeserializer result2 = SimplePropertyDeserializerFactory.Create();

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotSame(result1, result2);
        }
    }
}