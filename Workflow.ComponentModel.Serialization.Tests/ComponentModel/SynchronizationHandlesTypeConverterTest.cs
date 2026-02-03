using LogicBuilder.Workflow.ComponentModel;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace LogicBuilder.Workflow.Tests.ComponentModel
{
    public class SynchronizationHandlesTypeConverterTest
    {
        private readonly SynchronizationHandlesTypeConverter _converter;

        public SynchronizationHandlesTypeConverterTest()
        {
            _converter = new SynchronizationHandlesTypeConverter();
        }

        #region CanConvertTo Tests

        [Fact]
        public void CanConvertTo_ReturnsTrue_WhenDestinationTypeIsString()
        {
            // Act
            bool result = _converter.CanConvertTo(null, typeof(string));

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanConvertTo_ReturnsFalse_WhenDestinationTypeIsInt()
        {
            // Act
            bool result = _converter.CanConvertTo(null, typeof(int));

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanConvertTo_ReturnsFalse_WhenDestinationTypeIsNull()
        {
            // Act
            bool result = _converter.CanConvertTo(null, null);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region ConvertTo Tests

        [Fact]
        public void ConvertTo_ReturnsStringifiedValue_WhenValueIsStringCollection()
        {
            // Arrange
            var handles = new List<string> { "handle1", "handle2", "handle3" };

            // Act
            object result = _converter.ConvertTo(null, CultureInfo.InvariantCulture, handles, typeof(string));

            // Assert
            Assert.Equal("handle1, handle2, handle3", result);
        }

        [Fact]
        public void ConvertTo_ReturnsEmptyString_WhenCollectionIsEmpty()
        {
            // Arrange
            var handles = new List<string>();

            // Act
            object result = _converter.ConvertTo(null, CultureInfo.InvariantCulture, handles, typeof(string));

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ConvertTo_ReturnsEmptyString_WhenCollectionIsNull()
        {
            // Act
            object result = _converter.ConvertTo(null, CultureInfo.InvariantCulture, null, typeof(string));

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ConvertTo_EscapesCommasInHandles()
        {
            // Arrange
            var handles = new List<string> { "handle,with,commas", "normalHandle" };

            // Act
            object result = _converter.ConvertTo(null, CultureInfo.InvariantCulture, handles, typeof(string));

            // Assert
            Assert.Equal("handle\\,with\\,commas, normalHandle", result);
        }

        [Fact]
        public void ConvertTo_SkipsNullHandles()
        {
            // Arrange
            var handles = new List<string> { "handle1", null!, "handle2" };

            // Act
            object result = _converter.ConvertTo(null, CultureInfo.InvariantCulture, handles, typeof(string));

            // Assert
            Assert.Equal("handle1, handle2", result);
        }

        [Fact]
        public void ConvertTo_ThrowsNotSupportedException_WhenDestinationTypeIsNotString()
        {
            // Arrange
            var handles = new List<string> { "handle1" };

            // Act & Assert
            Assert.Throws<NotSupportedException>(() =>
                _converter.ConvertTo(null, CultureInfo.InvariantCulture, handles, typeof(int)));
        }

        #endregion

        #region CanConvertFrom Tests

        [Fact]
        public void CanConvertFrom_ReturnsTrue_WhenSourceTypeIsString()
        {
            // Act
            bool result = _converter.CanConvertFrom(null, typeof(string));

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanConvertFrom_ReturnsFalse_WhenSourceTypeIsInt()
        {
            // Act
            bool result = _converter.CanConvertFrom(null, typeof(int));

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanConvertFrom_ReturnsFalse_WhenSourceTypeIsNull()
        {
            // Act
            bool result = _converter.CanConvertFrom(null, null);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region ConvertFrom Tests

        [Fact]
        public void ConvertFrom_ReturnsCollectionOfHandles_WhenValueIsCommaSeparatedString()
        {
            // Arrange
            string input = "handle1, handle2, handle3";

            // Act
            var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, input) as ICollection<string>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains("handle1", result);
            Assert.Contains("handle2", result);
            Assert.Contains("handle3", result);
        }

        [Fact]
        public void ConvertFrom_UnescapesCommas()
        {
            // Arrange
            string input = "handle\\,with\\,commas, normalHandle";

            // Act
            var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, input) as ICollection<string>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains("handle,with,commas", result);
            Assert.Contains("normalHandle", result);
        }

        [Fact]
        public void ConvertFrom_TrimsWhitespace()
        {
            // Arrange
            string input = "  handle1  ,  handle2  ";

            // Act
            var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, input) as ICollection<string>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains("handle1", result);
            Assert.Contains("handle2", result);
        }

        [Fact]
        public void ConvertFrom_HandlesNewlineDelimiters()
        {
            // Arrange
            string input = "handle1\r\nhandle2\nhandle3";

            // Act
            var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, input) as ICollection<string>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains("handle1", result);
            Assert.Contains("handle2", result);
            Assert.Contains("handle3", result);
        }

        [Fact]
        public void ConvertFrom_RemovesDuplicates()
        {
            // Arrange
            string input = "handle1, handle2, handle1, handle3";

            // Act
            var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, input) as ICollection<string>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void ConvertFrom_SkipsEmptyEntries()
        {
            // Arrange
            string input = "handle1,  ,handle2,   ";

            // Act
            var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, input) as ICollection<string>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains("handle1", result);
            Assert.Contains("handle2", result);
        }

        [Fact]
        public void ConvertFrom_ReturnsEmptyCollection_WhenValueIsEmptyString()
        {
            // Act
            var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, string.Empty) as ICollection<string>;

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void ConvertFrom_ThrowsNotSupportedException_WhenValueIsNotString()
        {
            // Act & Assert
            Assert.Throws<NotSupportedException>(() =>
                _converter.ConvertFrom(null, CultureInfo.InvariantCulture, 123));
        }

        #endregion

        #region Stringify Tests

        [Fact]
        public void Stringify_ReturnsCommaSeparatedString()
        {
            // Arrange
            var handles = new List<string> { "handle1", "handle2", "handle3" };

            // Act
            string result = SynchronizationHandlesTypeConverter.Stringify(handles);

            // Assert
            Assert.Equal("handle1, handle2, handle3", result);
        }

        [Fact]
        public void Stringify_ReturnsEmptyString_WhenCollectionIsNull()
        {
            // Act
            string result = SynchronizationHandlesTypeConverter.Stringify(null);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Stringify_ReturnsEmptyString_WhenCollectionIsEmpty()
        {
            // Arrange
            var handles = new List<string>();

            // Act
            string result = SynchronizationHandlesTypeConverter.Stringify(handles);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Stringify_EscapesCommas()
        {
            // Arrange
            var handles = new List<string> { "handle,1", "handle,2" };

            // Act
            string result = SynchronizationHandlesTypeConverter.Stringify(handles);

            // Assert
            Assert.Equal("handle\\,1, handle\\,2", result);
        }

        [Fact]
        public void Stringify_SkipsNullHandles()
        {
            // Arrange
            var handles = new List<string> { "handle1", null!, "handle2", null! };

            // Act
            string result = SynchronizationHandlesTypeConverter.Stringify(handles);

            // Assert
            Assert.Equal("handle1, handle2", result);
        }

        [Fact]
        public void Stringify_HandlesOnlyNullHandles()
        {
            // Arrange
            var handles = new List<string> { null!, null! };

            // Act
            string result = SynchronizationHandlesTypeConverter.Stringify(handles);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        #endregion

        #region UnStringify Tests

        [Fact]
        public void UnStringify_ParsesCommaSeparatedString()
        {
            // Arrange
            string input = "handle1, handle2, handle3";

            // Act
            var result = SynchronizationHandlesTypeConverter.UnStringify(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains("handle1", result);
            Assert.Contains("handle2", result);
            Assert.Contains("handle3", result);
        }

        [Fact]
        public void UnStringify_UnescapesCommas()
        {
            // Arrange
            string input = "handle\\,1, handle\\,2";

            // Act
            var result = SynchronizationHandlesTypeConverter.UnStringify(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains("handle,1", result);
            Assert.Contains("handle,2", result);
        }

        [Fact]
        public void UnStringify_TrimsWhitespace()
        {
            // Arrange
            string input = "  handle1  ,  handle2  ";

            // Act
            var result = SynchronizationHandlesTypeConverter.UnStringify(input);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("handle1", result);
            Assert.Contains("handle2", result);
        }

        [Fact]
        public void UnStringify_HandlesNewlines()
        {
            // Arrange
            string input = "handle1\r\nhandle2\nhandle3";

            // Act
            var result = SynchronizationHandlesTypeConverter.UnStringify(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void UnStringify_RemovesDuplicates()
        {
            // Arrange
            string input = "handle1, handle2, handle1";

            // Act
            var result = SynchronizationHandlesTypeConverter.UnStringify(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void UnStringify_ReturnsEmptyCollection_WhenInputIsEmpty()
        {
            // Act
            var result = SynchronizationHandlesTypeConverter.UnStringify(string.Empty);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void UnStringify_SkipsEmptyEntries()
        {
            // Arrange
            string input = "handle1, , handle2,   ,";

            // Act
            var result = SynchronizationHandlesTypeConverter.UnStringify(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains("handle1", result);
            Assert.Contains("handle2", result);
        }

        #endregion

        #region Round-trip Tests

        [Fact]
        public void RoundTrip_PreservesHandles()
        {
            // Arrange
            var originalHandles = new List<string> { "handle1", "handle2", "handle3" };

            // Act
            string stringified = SynchronizationHandlesTypeConverter.Stringify(originalHandles);
            var result = SynchronizationHandlesTypeConverter.UnStringify(stringified);

            // Assert
            Assert.Equal(originalHandles.Count, result.Count);
            foreach (var handle in originalHandles)
            {
                Assert.Contains(handle, result);
            }
        }

        [Fact]
        public void RoundTrip_PreservesHandlesWithCommas()
        {
            // Arrange
            var originalHandles = new List<string> { "handle,1", "handle,2,3", "normalHandle" };

            // Act
            string stringified = SynchronizationHandlesTypeConverter.Stringify(originalHandles);
            var result = SynchronizationHandlesTypeConverter.UnStringify(stringified);

            // Assert
            Assert.Equal(originalHandles.Count, result.Count);
            foreach (var handle in originalHandles)
            {
                Assert.Contains(handle, result);
            }
        }

        [Fact]
        public void RoundTrip_UsingConverter_PreservesHandles()
        {
            // Arrange
            var originalHandles = new List<string> { "handle1", "handle2", "handle3" };

            // Act
            var stringified = _converter.ConvertTo(null, CultureInfo.InvariantCulture, originalHandles, typeof(string)) as string;
            var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, stringified) as ICollection<string>;

            // Assert
            Assert.Equal(originalHandles.Count, result!.Count);
            foreach (var handle in originalHandles)
            {
                Assert.Contains(handle, result);
            }
        }

        #endregion
    }
}