using LogicBuilder.Workflow.ComponentModel.Serialization.Factories;
using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization.Factories
{
    public class ObjectDeserializerFactoryTest
    {
        [Fact]
        public void Create_ReturnsNonNullInstance()
        {
            // Act
            var result = ObjectDeserializerFactory.Create();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Create_ReturnsIObjectDeserializerInstance()
        {
            // Act
            var result = ObjectDeserializerFactory.Create();

            // Assert
            Assert.IsType<IObjectDeserializer>(result, exactMatch: false);
        }

        [Fact]
        public void Create_ReturnsFunctionalInstance()
        {
            // Act
            var result = ObjectDeserializerFactory.Create();

            // Assert
            Assert.NotNull(result);
            // Verify the instance has the expected interface methods available
            Assert.NotNull(result.GetType().GetMethod("DeserializeObject"));
            Assert.NotNull(result.GetType().GetMethod("CreateInstance"));
            Assert.NotNull(result.GetType().GetMethod("DeserializeContents"));
            Assert.NotNull(result.GetType().GetMethod("DeserializeCompoundProperty"));
        }

        [Fact]
        public void Create_ReturnsNewInstanceEachTime()
        {
            // Act
            var result1 = ObjectDeserializerFactory.Create();
            var result2 = ObjectDeserializerFactory.Create();

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotSame(result1, result2);
        }
    }
}