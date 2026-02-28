using LogicBuilder.Workflow.ComponentModel.Serialization.Factories;
using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization.Factories
{
    public class AttributesTokenizerFactoryTest
    {
        [Fact]
        public void Create_ReturnsAttributesTokenizerInstance()
        {
            // Act
            IAttributesTokenizer result = AttributesTokenizerFactory.Create();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Create_ReturnsIAttributesTokenizerType()
        {
            // Act
            IAttributesTokenizer result = AttributesTokenizerFactory.Create();

            // Assert
            Assert.IsType<IAttributesTokenizer>(result, exactMatch: false);
        }

        [Fact]
        public void Create_ReturnsNewInstanceEachTime()
        {
            // Act
            IAttributesTokenizer result1 = AttributesTokenizerFactory.Create();
            IAttributesTokenizer result2 = AttributesTokenizerFactory.Create();

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotSame(result1, result2);
        }

        [Fact]
        public void Create_ReturnsWorkingTokenizer()
        {
            // Arrange
            IAttributesTokenizer tokenizer = AttributesTokenizerFactory.Create();
            string input = "value1, value2}";

            // Act
            var result = tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
        }
    }
}