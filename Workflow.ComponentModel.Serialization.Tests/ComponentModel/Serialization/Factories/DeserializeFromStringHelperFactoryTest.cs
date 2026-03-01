using LogicBuilder.Workflow.ComponentModel.Serialization.Factories;
using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization.Factories
{
    public class DeserializeFromStringHelperFactoryTest
    {
        [Fact]
        public void Create_ReturnsDeserializeFromStringHelperInstance()
        {
            // Arrange
            IDependencyHelper dependencyHelper = DependencyHelperFactory.Create();

            // Act
            IDeserializeFromStringHelper result = DeserializeFromStringHelperFactory.Create(dependencyHelper);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Create_ReturnsIDeserializeFromStringHelperType()
        {
            // Arrange
            IDependencyHelper dependencyHelper = DependencyHelperFactory.Create();

            // Act
            IDeserializeFromStringHelper result = DeserializeFromStringHelperFactory.Create(dependencyHelper);

            // Assert
            Assert.IsType<IDeserializeFromStringHelper>(result, exactMatch: false);
        }

        [Fact]
        public void Create_ReturnsNewInstanceEachTime()
        {
            // Arrange
            IDependencyHelper dependencyHelper = DependencyHelperFactory.Create();

            // Act
            IDeserializeFromStringHelper result1 = DeserializeFromStringHelperFactory.Create(dependencyHelper);
            IDeserializeFromStringHelper result2 = DeserializeFromStringHelperFactory.Create(dependencyHelper);

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotSame(result1, result2);
        }

        [Fact]
        public void Create_ThrowsArgumentNullException_WhenDependencyHelperIsNull()
        {
            // Arrange
            IDependencyHelper dependencyHelper = null!;

            // Act & Assert
            Assert.Throws<System.ArgumentNullException>(() => DeserializeFromStringHelperFactory.Create(dependencyHelper));
        }
    }
}