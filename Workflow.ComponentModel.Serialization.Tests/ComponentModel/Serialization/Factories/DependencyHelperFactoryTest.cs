using LogicBuilder.Workflow.ComponentModel.Serialization.Factories;
using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization.Factories
{
    public class DependencyHelperFactoryTest
    {
        [Fact]
        public void Create_ReturnsDependencyHelperInstance()
        {
            // Act
            IDependencyHelper result = DependencyHelperFactory.Create();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Create_ReturnsIDependencyHelperType()
        {
            // Act
            IDependencyHelper result = DependencyHelperFactory.Create();

            // Assert
            Assert.IsType<IDependencyHelper>(result, exactMatch: false);
        }

        [Fact]
        public void Create_ReturnsNewInstanceEachTime()
        {
            // Act
            IDependencyHelper result1 = DependencyHelperFactory.Create();
            IDependencyHelper result2 = DependencyHelperFactory.Create();

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotSame(result1, result2);
        }

        [Fact]
        public void Create_ReturnsHelperWithInitializedDeserializeFromStringHelper()
        {
            // Act
            IDependencyHelper result = DependencyHelperFactory.Create();

            // Assert
            Assert.NotNull(result.DeserializeFromStringHelper);
        }

        [Fact]
        public void Create_ReturnsHelperWithInitializedFromCompactFormatDeserializer()
        {
            // Act
            IDependencyHelper result = DependencyHelperFactory.Create();

            // Assert
            Assert.NotNull(result.FromCompactFormatDeserializer);
        }
    }
}