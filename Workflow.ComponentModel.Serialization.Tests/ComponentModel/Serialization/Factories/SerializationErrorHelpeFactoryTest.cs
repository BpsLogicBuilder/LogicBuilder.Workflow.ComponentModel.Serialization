using LogicBuilder.Workflow.ComponentModel.Serialization.Factories;
using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization.Factories
{
    public class SerializationErrorHelpeFactoryTest
    {
        [Fact]
        public void Create_ReturnsSerializationErrorHelperInstance()
        {
            // Act
            ISerializationErrorHelper result = SerializationErrorHelperFactory.Create();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Create_ReturnsISerializationErrorHelperType()
        {
            // Act
            ISerializationErrorHelper result = SerializationErrorHelperFactory.Create();

            // Assert
            Assert.IsType<ISerializationErrorHelper>(result, exactMatch: false);
        }

        [Fact]
        public void Create_ReturnsNewInstanceEachTime()
        {
            // Act
            ISerializationErrorHelper result1 = SerializationErrorHelperFactory.Create();
            ISerializationErrorHelper result2 = SerializationErrorHelperFactory.Create();

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotSame(result1, result2);
        }
    }
}