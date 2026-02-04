using LogicBuilder.Workflow.ComponentModel.Design;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Design
{
    public class HelpersTest
    {
        #region CreateXmlWriter Tests

        [Fact]
        public void CreateXmlWriter_WithStringPath_CreatesXmlWriter()
        {
            // Arrange
            string tempFilePath = Path.GetTempFileName();

            try
            {
                // Act
                XmlWriter writer = Helpers.CreateXmlWriter(tempFilePath);

                // Assert
                Assert.NotNull(writer);

                // Write some content to verify it works
                writer.WriteStartElement("root");
                writer.WriteElementString("test", "value");
                writer.WriteEndElement();
                writer.Close();

                // Verify file was created and contains expected content
                Assert.True(File.Exists(tempFilePath));
                string content = File.ReadAllText(tempFilePath);
                Assert.Contains("<root>", content);
                Assert.Contains("<test>value</test>", content);
            }
            finally
            {
                // Cleanup
                if (File.Exists(tempFilePath))
                    File.Delete(tempFilePath);
            }
        }

        [Fact]
        public void CreateXmlWriter_WithTextWriter_CreatesXmlWriter()
        {
            // Arrange
            using StringWriter stringWriter = new();
            // Act
            XmlWriter writer = Helpers.CreateXmlWriter(stringWriter);

            // Assert
            Assert.NotNull(writer);

            // Write some content to verify it works
            writer.WriteStartElement("root");
            writer.WriteElementString("test", "value");
            writer.WriteEndElement();
            writer.Flush();

            // Verify content
            string result = stringWriter.ToString();
            Assert.Contains("<root>", result);
            Assert.Contains("<test>value</test>", result);
        }

        [Fact]
        public void CreateXmlWriter_SettingsApplied_IndentsContent()
        {
            // Arrange
            using StringWriter stringWriter = new();
            // Act
            XmlWriter writer = Helpers.CreateXmlWriter(stringWriter);
            writer.WriteStartElement("root");
            writer.WriteStartElement("child");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Flush();

            // Assert
            string result = stringWriter.ToString();
            // Should contain indentation (tabs)
            Assert.Contains("\t", result);
        }

        [Fact]
        public void CreateXmlWriter_SettingsApplied_OmitsXmlDeclaration()
        {
            // Arrange
            using StringWriter stringWriter = new();
            // Act
            XmlWriter writer = Helpers.CreateXmlWriter(stringWriter);
            writer.WriteStartElement("root");
            writer.WriteEndElement();
            writer.Flush();

            // Assert
            string result = stringWriter.ToString();
            // Should not contain XML declaration
            Assert.DoesNotContain("<?xml", result);
        }

        [Fact]
        public void CreateXmlWriter_WithInvalidType_ReturnsNull()
        {
            // Arrange
            object invalidOutput = 12345; // Neither string nor TextWriter

            // Act
            XmlWriter writer = Helpers.CreateXmlWriter(invalidOutput);

            // Assert
            Assert.Null(writer);
        }

        [Fact]
        public void CreateXmlWriter_WithNull_ReturnsNull()
        {
            // Arrange
            System.Diagnostics.Trace.Listeners.Clear();
            object? nullOutput = null;

            // Act
            XmlWriter writer = Helpers.CreateXmlWriter(nullOutput);

            // Assert
            Assert.Null(writer);
        }

        #endregion

        #region GetSerializationVisibility Tests

        [Fact]
        public void GetSerializationVisibility_WithNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => Helpers.GetSerializationVisibility(null));
            Assert.Equal("memberInfo", exception.ParamName);
        }

        [Fact]
        public void GetSerializationVisibility_WithNoAttribute_ReturnsVisible()
        {
            // Arrange
            var propertyInfo = typeof(TestClassWithoutAttributes).GetProperty(nameof(TestClassWithoutAttributes.NormalProperty));

            // Act
            var visibility = Helpers.GetSerializationVisibility(propertyInfo);

            // Assert
            Assert.Equal(DesignerSerializationVisibility.Visible, visibility);
        }

        [Fact]
        public void GetSerializationVisibility_WithVisibleAttribute_ReturnsVisible()
        {
            // Arrange
            var propertyInfo = typeof(TestClassWithAttributes).GetProperty(nameof(TestClassWithAttributes.VisibleProperty));

            // Act
            var visibility = Helpers.GetSerializationVisibility(propertyInfo);

            // Assert
            Assert.Equal(DesignerSerializationVisibility.Visible, visibility);
        }

        [Fact]
        public void GetSerializationVisibility_WithHiddenAttribute_ReturnsHidden()
        {
            // Arrange
            var propertyInfo = typeof(TestClassWithAttributes).GetProperty(nameof(TestClassWithAttributes.HiddenProperty));

            // Act
            var visibility = Helpers.GetSerializationVisibility(propertyInfo);

            // Assert
            Assert.Equal(DesignerSerializationVisibility.Hidden, visibility);
        }

        [Fact]
        public void GetSerializationVisibility_WithContentAttribute_ReturnsContent()
        {
            // Arrange
            var propertyInfo = typeof(TestClassWithAttributes).GetProperty(nameof(TestClassWithAttributes.ContentProperty));

            // Act
            var visibility = Helpers.GetSerializationVisibility(propertyInfo);

            // Assert
            Assert.Equal(DesignerSerializationVisibility.Content, visibility);
        }

        [Fact]
        public void GetSerializationVisibility_WithFieldMember_ReturnsCorrectVisibility()
        {
            // Arrange
            var fieldInfo = typeof(TestClassWithAttributes).GetField(nameof(TestClassWithAttributes.HiddenField));

            // Act
            var visibility = Helpers.GetSerializationVisibility(fieldInfo);

            // Assert
            Assert.Equal(DesignerSerializationVisibility.Hidden, visibility);
        }

        [Fact]
        public void GetSerializationVisibility_WithMethodMember_ReturnsVisible()
        {
            // Arrange
            var methodInfo = typeof(TestClassWithAttributes).GetMethod(nameof(TestClassWithAttributes.NormalMethod));

            // Act
            var visibility = Helpers.GetSerializationVisibility(methodInfo);

            // Assert
            Assert.Equal(DesignerSerializationVisibility.Visible, visibility);
        }

        #endregion

        #region Test Helper Classes

        private class TestClassWithoutAttributes
        {
            public string NormalProperty { get; set; } = string.Empty;
        }

        private class TestClassWithAttributes
        {
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
            public string VisibleProperty { get; set; } = string.Empty;

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public string HiddenProperty { get; set; } = string.Empty;

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
            public string ContentProperty { get; set; } = string.Empty;

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public readonly string HiddenField = string.Empty;

            public static void NormalMethod() { }
        }

        #endregion
    }
}