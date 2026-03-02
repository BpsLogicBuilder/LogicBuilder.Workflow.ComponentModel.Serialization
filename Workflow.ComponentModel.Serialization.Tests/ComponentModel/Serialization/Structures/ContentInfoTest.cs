using LogicBuilder.Workflow.ComponentModel.Serialization.Structures;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization.Structures
{
    public class ContentInfoTest
    {
        [Fact]
        public void Constructor_InitializesAllFields()
        {
            // Arrange
            object content = "test content";
            int lineNumber = 10;
            int linePosition = 5;

            // Act
            var contentInfo = new ContentInfo(content, lineNumber, linePosition);

            // Assert
            Assert.Equal(content, contentInfo.Content);
            Assert.Equal(lineNumber, contentInfo.LineNumber);
            Assert.Equal(linePosition, contentInfo.LinePosition);
        }

        [Fact]
        public void Constructor_WithNullContent_StoresNull()
        {
            // Arrange
            object? content = null;
            int lineNumber = 1;
            int linePosition = 1;

            // Act
            var contentInfo = new ContentInfo(content, lineNumber, linePosition);

            // Assert
            Assert.Null(contentInfo.Content);
            Assert.Equal(lineNumber, contentInfo.LineNumber);
            Assert.Equal(linePosition, contentInfo.LinePosition);
        }

        [Fact]
        public void Constructor_WithZeroLineNumber_StoresZero()
        {
            // Arrange
            object content = "content";
            int lineNumber = 0;
            int linePosition = 0;

            // Act
            var contentInfo = new ContentInfo(content, lineNumber, linePosition);

            // Assert
            Assert.Equal(content, contentInfo.Content);
            Assert.Equal(0, contentInfo.LineNumber);
            Assert.Equal(0, contentInfo.LinePosition);
        }

        [Fact]
        public void Constructor_WithNegativeLineNumbers_StoresNegativeValues()
        {
            // Arrange
            object content = "content";
            int lineNumber = -1;
            int linePosition = -5;

            // Act
            var contentInfo = new ContentInfo(content, lineNumber, linePosition);

            // Assert
            Assert.Equal(content, contentInfo.Content);
            Assert.Equal(-1, contentInfo.LineNumber);
            Assert.Equal(-5, contentInfo.LinePosition);
        }

        [Fact]
        public void Constructor_WithStringContent_StoresString()
        {
            // Arrange
            string content = "test string";
            int lineNumber = 15;
            int linePosition = 20;

            // Act
            var contentInfo = new ContentInfo(content, lineNumber, linePosition);

            // Assert
            Assert.Equal(content, contentInfo.Content);
            Assert.IsType<string>(contentInfo.Content);
        }

        [Fact]
        public void Constructor_WithIntContent_StoresInt()
        {
            // Arrange
            object content = 42;
            int lineNumber = 5;
            int linePosition = 10;

            // Act
            var contentInfo = new ContentInfo(content, lineNumber, linePosition);

            // Assert
            Assert.Equal(content, contentInfo.Content);
            Assert.IsType<int>(contentInfo.Content);
        }

        [Fact]
        public void Constructor_WithComplexObjectContent_StoresObject()
        {
            // Arrange
            var content = new { Name = "Test", Value = 123 };
            int lineNumber = 8;
            int linePosition = 3;

            // Act
            var contentInfo = new ContentInfo(content, lineNumber, linePosition);

            // Assert
            Assert.Equal(content, contentInfo.Content);
            Assert.Same(content, contentInfo.Content);
        }

        [Fact]
        public void Struct_WithSameValues_AreEqual()
        {
            // Arrange
            object content = "same content";
            var contentInfo1 = new ContentInfo(content, 10, 5);
            var contentInfo2 = new ContentInfo(content, 10, 5);

            // Act & Assert
            Assert.Equal(contentInfo1, contentInfo2);
        }

        [Fact]
        public void Struct_WithDifferentLineNumber_AreNotEqual()
        {
            // Arrange
            object content = "content";
            var contentInfo1 = new ContentInfo(content, 10, 5);
            var contentInfo2 = new ContentInfo(content, 11, 5);

            // Act & Assert
            Assert.NotEqual(contentInfo1, contentInfo2);
        }

        [Fact]
        public void Struct_WithDifferentLinePosition_AreNotEqual()
        {
            // Arrange
            object content = "content";
            var contentInfo1 = new ContentInfo(content, 10, 5);
            var contentInfo2 = new ContentInfo(content, 10, 6);

            // Act & Assert
            Assert.NotEqual(contentInfo1, contentInfo2);
        }

        [Fact]
        public void Struct_WithDifferentContent_AreNotEqual()
        {
            // Arrange
            var contentInfo1 = new ContentInfo("content1", 10, 5);
            var contentInfo2 = new ContentInfo("content2", 10, 5);

            // Act & Assert
            Assert.NotEqual(contentInfo1, contentInfo2);
        }

        [Fact]
        public void Fields_RetainValues_AfterConstruction()
        {
            // Arrange
            object content = "persistent content";
            int lineNumber = 25;
            int linePosition = 30;
            var contentInfo = new ContentInfo(content, lineNumber, linePosition);

            // Act - Read fields multiple times
            var content1 = contentInfo.Content;
            var content2 = contentInfo.Content;
            var lineNumber1 = contentInfo.LineNumber;
            var lineNumber2 = contentInfo.LineNumber;
            var linePosition1 = contentInfo.LinePosition;
            var linePosition2 = contentInfo.LinePosition;

            // Assert
            Assert.Same(content1, content2);
            Assert.Equal(lineNumber1, lineNumber2);
            Assert.Equal(linePosition1, linePosition2);
        }

        [Fact]
        public void Constructor_WithLargeLineNumbers_StoresCorrectly()
        {
            // Arrange
            object content = "content";
            int lineNumber = int.MaxValue;
            int linePosition = int.MaxValue - 1;

            // Act
            var contentInfo = new ContentInfo(content, lineNumber, linePosition);

            // Assert
            Assert.Equal(content, contentInfo.Content);
            Assert.Equal(int.MaxValue, contentInfo.LineNumber);
            Assert.Equal(int.MaxValue - 1, contentInfo.LinePosition);
        }
    }
}