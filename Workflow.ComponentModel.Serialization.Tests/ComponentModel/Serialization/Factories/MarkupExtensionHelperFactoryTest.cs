using LogicBuilder.Workflow.ComponentModel.Serialization.Factories;
using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization.Factories
{
    public class MarkupExtensionHelperFactoryTest
    {
        [Fact]
        public void Create_ReturnsMarkupExtensionHelperInstance()
        {
            // Act
            IMarkupExtensionHelper result = MarkupExtensionHelperFactory.Create();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Create_ReturnsIMarkupExtensionHelperType()
        {
            // Act
            IMarkupExtensionHelper result = MarkupExtensionHelperFactory.Create();

            // Assert
            Assert.IsType<IMarkupExtensionHelper>(result, exactMatch: false);
        }

        [Fact]
        public void Create_ReturnsNewInstanceEachTime()
        {
            // Act
            IMarkupExtensionHelper result1 = MarkupExtensionHelperFactory.Create();
            IMarkupExtensionHelper result2 = MarkupExtensionHelperFactory.Create();

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotSame(result1, result2);
        }
    }
}