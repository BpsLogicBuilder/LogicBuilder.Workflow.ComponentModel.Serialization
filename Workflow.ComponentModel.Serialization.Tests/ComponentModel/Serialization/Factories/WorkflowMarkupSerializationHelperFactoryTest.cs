using LogicBuilder.Workflow.ComponentModel.Serialization.Factories;
using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization.Factories
{
    public class WorkflowMarkupSerializationHelperFactoryTest
    {
        [Fact]
        public void Create_ReturnsWorkflowMarkupSerializationHelperInstance()
        {
            // Act
            IWorkflowMarkupSerializationHelper result = WorkflowMarkupSerializationHelperFactory.Create();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Create_ReturnsIWorkflowMarkupSerializationHelperType()
        {
            // Act
            IWorkflowMarkupSerializationHelper result = WorkflowMarkupSerializationHelperFactory.Create();

            // Assert
            Assert.IsType<IWorkflowMarkupSerializationHelper>(result, exactMatch: false);
        }

        [Fact]
        public void Create_ReturnsNewInstanceEachTime()
        {
            // Act
            IWorkflowMarkupSerializationHelper result1 = WorkflowMarkupSerializationHelperFactory.Create();
            IWorkflowMarkupSerializationHelper result2 = WorkflowMarkupSerializationHelperFactory.Create();

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotSame(result1, result2);
        }

        [Fact]
        public void Create_ReturnsWorkingHelper()
        {
            // Arrange
            IWorkflowMarkupSerializationHelper helper = WorkflowMarkupSerializationHelperFactory.Create();
            string validCompactFormat = "{x:Type sys:String}";

            // Act
            bool result = helper.IsValidCompactAttributeFormat(validCompactFormat);

            // Assert
            Assert.True(result);
        }
    }
}