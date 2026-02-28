using LogicBuilder.Workflow.ComponentModel.Serialization.Factories;
using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization.Factories
{
    public class FromCompactFormatDeserializerFactoryTest
    {
        [Fact]
        public void Create_ReturnsFromCompactFormatDeserializerInstance()
        {
            // Arrange
            IDependencyHelper dependencyHelper = DependencyHelperFactory.Create();

            // Act
            IFromCompactFormatDeserializer result = FromCompactFormatDeserializerFactory.Create(dependencyHelper);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Create_ReturnsIFromCompactFormatDeserializerType()
        {
            // Arrange
            IDependencyHelper dependencyHelper = DependencyHelperFactory.Create();

            // Act
            IFromCompactFormatDeserializer result = FromCompactFormatDeserializerFactory.Create(dependencyHelper);

            // Assert
            Assert.IsType<IFromCompactFormatDeserializer>(result, exactMatch: false);
        }

        [Fact]
        public void Create_ReturnsNewInstanceEachTime()
        {
            // Arrange
            IDependencyHelper dependencyHelper = DependencyHelperFactory.Create();

            // Act
            IFromCompactFormatDeserializer result1 = FromCompactFormatDeserializerFactory.Create(dependencyHelper);
            IFromCompactFormatDeserializer result2 = FromCompactFormatDeserializerFactory.Create(dependencyHelper);

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
            Assert.Throws<System.ArgumentNullException>(() => FromCompactFormatDeserializerFactory.Create(dependencyHelper));
        }
    }
}