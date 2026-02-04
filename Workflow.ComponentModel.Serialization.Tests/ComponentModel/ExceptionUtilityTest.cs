using LogicBuilder.Workflow.ComponentModel;
using System;
using System.Threading;
using Xunit;

namespace LogicBuilder.Workflow.Tests.ComponentModel
{
    public class ExceptionUtilityTest
    {
        #region IsCriticalException Tests

        [Fact]
        public void IsCriticalException_WithOutOfMemoryException_ReturnsTrue()
        {
            // Arrange
            var exception = new OutOfMemoryException();

            // Act
            var result = ExceptionUtility.IsCriticalException(exception);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsCriticalException_WithStackOverflowException_ReturnsTrue()
        {
            // Arrange
            var exception = new StackOverflowException();

            // Act
            var result = ExceptionUtility.IsCriticalException(exception);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsCriticalException_WithThreadInterruptedException_ReturnsTrue()
        {
            // Arrange
            var exception = new ThreadInterruptedException();

            // Act
            var result = ExceptionUtility.IsCriticalException(exception);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsCriticalException_WithArgumentNullException_ReturnsFalse()
        {
            // Arrange
            var exception = new ArgumentNullException();

            // Act
            var result = ExceptionUtility.IsCriticalException(exception);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsCriticalException_WithInvalidOperationException_ReturnsFalse()
        {
            // Arrange
            var exception = new InvalidOperationException();

            // Act
            var result = ExceptionUtility.IsCriticalException(exception);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsCriticalException_WithArgumentException_ReturnsFalse()
        {
            // Arrange
            var exception = new ArgumentException();

            // Act
            var result = ExceptionUtility.IsCriticalException(exception);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsCriticalException_WithNotSupportedException_ReturnsFalse()
        {
            // Arrange
            var exception = new NotSupportedException();

            // Act
            var result = ExceptionUtility.IsCriticalException(exception);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsCriticalException_WithNullReferenceException_ReturnsFalse()
        {
            // Arrange
            var exception = new NullReferenceException();

            // Act
            var result = ExceptionUtility.IsCriticalException(exception);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsCriticalException_WithIOException_ReturnsFalse()
        {
            // Arrange
            var exception = new System.IO.IOException();

            // Act
            var result = ExceptionUtility.IsCriticalException(exception);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsCriticalException_WithGenericException_ReturnsFalse()
        {
            // Arrange
            var exception = new Exception();

            // Act
            var result = ExceptionUtility.IsCriticalException(exception);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsCriticalException_WithCustomException_ReturnsFalse()
        {
            // Arrange
            var exception = new CustomTestException();

            // Act
            var result = ExceptionUtility.IsCriticalException(exception);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsCriticalException_WithDerivedFromOutOfMemoryException_ReturnsTrue()
        {
            // Arrange
            var exception = new InsufficientMemoryException();

            // Act
            var result = ExceptionUtility.IsCriticalException(exception);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Helper Classes

        private class CustomTestException : Exception
        {
            public CustomTestException() : base("Custom test exception")
            {
            }
        }

        private class InsufficientMemoryException : OutOfMemoryException
        {
            public InsufficientMemoryException() : base("Insufficient memory")
            {
            }
        }

        #endregion
    }
}