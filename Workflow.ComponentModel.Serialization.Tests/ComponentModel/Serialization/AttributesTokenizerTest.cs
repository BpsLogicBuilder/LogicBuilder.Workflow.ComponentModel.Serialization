using LogicBuilder.Workflow.ComponentModel.Serialization.Factories;
using LogicBuilder.Workflow.ComponentModel.Serialization.Interfaces;
using System;
using System.Collections;

namespace LogicBuilder.Workflow.Tests.ComponentModel.Serialization
{
    public class AttributesTokenizerTest
    {
        private readonly IAttributesTokenizer _tokenizer;

        public AttributesTokenizerTest()
        {
            _tokenizer = AttributesTokenizerFactory.Create();
        }

        #region Basic Tokenization Tests

        [Fact]
        public void TokenizeAttributes_ReturnsNull_WhenInputIsEmpty()
        {
            // Arrange
            string input = "";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void TokenizeAttributes_ReturnsNull_WhenInputIsWhitespaceOnly()
        {
            // Arrange
            string input = "   ";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void TokenizeAttributes_ReturnsSingleToken_WhenInputIsSingleValue()
        {
            // Arrange
            string input = "value}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("value", result[0]);
        }

        [Fact]
        public void TokenizeAttributes_TokenizesMultipleValues_WithCommaDelimiter()
        {
            // Arrange
            string input = "value1,value2,value3}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.Count);
            Assert.Equal("value1", result[0]);
            Assert.Equal(',', result[1]);
            Assert.Equal("value2", result[2]);
            Assert.Equal(',', result[3]);
            Assert.Equal("value3", result[4]);
        }

        [Fact]
        public void TokenizeAttributes_TokenizesKeyValuePair_WithEqualsDelimiter()
        {
            // Arrange
            string input = "key=value}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("key", result[0]);
            Assert.Equal('=', result[1]);
            Assert.Equal("value", result[2]);
        }

        #endregion

        #region Quote Handling Tests

        [Fact]
        public void TokenizeAttributes_HandlesDoubleQuotedString()
        {
            // Arrange
            string input = "\"quoted value\"}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("quoted value", result[0]);
        }

        [Fact]
        public void TokenizeAttributes_HandlesSingleQuotedString()
        {
            // Arrange
            string input = "'quoted value'}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("quoted value", result[0]);
        }

        [Fact]
        public void TokenizeAttributes_HandlesQuotedStringWithComma()
        {
            // Arrange
            string input = "\"value,with,commas\"}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("value,with,commas", result[0]);
        }

        [Fact]
        public void TokenizeAttributes_HandlesQuotedStringWithEquals()
        {
            // Arrange
            string input = "\"value=with=equals\"}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("value=with=equals", result[0]);
        }

        [Fact]
        public void TokenizeAttributes_HandlesMultipleQuotedStrings()
        {
            // Arrange
            string input = "\"first\",\"second\",\"third\"}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.Count);
            Assert.Equal("first", result[0]);
            Assert.Equal(',', result[1]);
            Assert.Equal("second", result[2]);
            Assert.Equal(',', result[3]);
            Assert.Equal("third", result[4]);
        }

        [Fact]
        public void TokenizeAttributes_ReturnsEmptyList_WhenQuoteNotAtBeginning()
        {
            // Arrange
            string input = "value\"quoted\"}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region Escape Character Tests

        [Fact]
        public void TokenizeAttributes_HandlesEscapedBackslash()
        {
            // Arrange
            string input = "value\\\\test}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("value\\\\test", result[0]);
        }

        [Fact]
        public void TokenizeAttributes_HandlesEscapedComma()
        {
            // Arrange
            string input = "value\\,test}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("value\\,test", result[0]);
        }

        [Fact]
        public void TokenizeAttributes_HandlesEscapedEquals()
        {
            // Arrange
            string input = "value\\=test}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("value\\=test", result[0]);
        }

        [Fact]
        public void TokenizeAttributes_HandlesEscapedQuote()
        {
            // Arrange
            string input = "\\\"value}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("\\\"value", result[0]);
        }

        #endregion

        #region Curly Brace Tests

        [Fact]
        public void TokenizeAttributes_HandlesNestedCurlyBraces()
        {
            // Arrange
            string input = "Stuff {nested {content}}}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Stuff {nested {content}}", result[0]);
        }

        [Fact]
        public void TokenizeAttributes_HandlesMultipleLevelsOfNesting()
        {
            // Arrange
            string input = "Stuff {level1 {level2 {level3}}}}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Stuff {level1 {level2 {level3}}}", result[0]);
        }

        [Fact]
        public void TokenizeAttributes_HandlesNestedBracesWithCommas()
        {
            // Arrange
            string input = "{nested,content},value}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("{nested,content}", result[0]);
            Assert.Equal(',', result[1]);
            Assert.Equal("value", result[2]);
        }

        #endregion

        #region Collection Indexer Tests

        [Fact]
        public void TokenizeAttributes_HandlesCollectionIndexerWithSingleQuotedIndex()
        {
            // Arrange
            string input = "collection['someName']}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("collection['someName']", result[0]);
        }

        [Fact]
        public void TokenizeAttributes_HandlesCollectionIndexerWithDoubleQuotedIndex()
        {
            // Arrange
            string input = "collection[\"someName\"]}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("collection[\"someName\"]", result[0]);
        }

        [Fact]
        public void TokenizeAttributes_HandlesCollectionIndexerWithSingleQuote()
        {
            // Arrange
            string input = "collection[']']}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("collection[']']", result[0]);
        }

        [Fact]
        public void TokenizeAttributes_HandlesCollectionIndexerWithDoubleQuote()
        {
            // Arrange
            string input = "collection[\"]\"]}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("collection[\"]\"]", result[0]);
        }

        [Fact]
        public void TokenizeAttributes_ReturnsEmptyCollectionUsingSingleQuote_WithMissingClosingSquareBracket()
        {
            // Arrange
            string input = "collection['A'}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void TokenizeAttributes_ReturnsEmptyCollectionUsingDoubleQuote_WithMissingClosingSquareBracket()
        {
            // Arrange
            string input = "collection[\"A\"}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void TokenizeAttributes_HandlesQuoteAfterOpenBracket()
        {
            // Arrange
            string input = "array['key']}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("array['key']", result[0]);
        }

        #endregion

        #region Whitespace Handling Tests

        [Fact]
        public void TokenizeAttributes_TrimsWhitespaceFromValues()
        {
            // Arrange
            string input = "  value1  ,  value2  }";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("value1", result[0]);
            Assert.Equal(',', result[1]);
            Assert.Equal("value2", result[2]);
        }

        [Fact]
        public void TokenizeAttributes_PreservesWhitespaceInQuotes()
        {
            // Arrange
            string input = "\"  value with spaces  \"}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("  value with spaces  ", result[0]);
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public void TokenizeAttributes_ReturnsEmptyList_WhenTwoDelimitersInRow()
        {
            // Arrange
            string input = "value1,,value2}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void TokenizeAttributes_ReturnsEmptyList_WhenDelimiterIsFirst()
        {
            // Arrange
            string input = ",value}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void TokenizeAttributes_ReturnsEmptyList_WhenDelimiterBeforeClosingBrace()
        {
            // Arrange
            string input = "value,}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void TokenizeAttributes_ThrowsException_WhenMissingTerminatingCharacter()
        {
            // Arrange
            string input = "value";

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _tokenizer.TokenizeAttributes(input));
        }

        [Fact]
        public void TokenizeAttributes_ThrowsException_WhenExtraCharactersAfterClosingBrace()
        {
            // Arrange
            string input = "value}extra";

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _tokenizer.TokenizeAttributes(input));
        }

        [Fact]
        public void TokenizeAttributes_DoesNotThrow_WhenWhitespaceAfterClosingBrace()
        {
            // Arrange
            string input = "value}   ";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("value", result[0]);
        }

        #endregion

        #region Complex Scenario Tests

        [Fact]
        public void TokenizeAttributes_HandlesComplexMarkupExtension()
        {
            // Arrange
            string input = "PropertyName={StaticResource Key},PropertyValue=\"test\"}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(7, result.Count);
            Assert.Equal("PropertyName", result[0]);
            Assert.Equal('=', result[1]);
            Assert.Equal("{StaticResource Key}", result[2]);
            Assert.Equal(',', result[3]);
            Assert.Equal("PropertyValue", result[4]);
            Assert.Equal('=', result[5]);
            Assert.Equal("test", result[6]);
        }

        [Fact]
        public void TokenizeAttributes_HandlesMixedQuotesAndBraces()
        {
            // Arrange
            string input = "\"quoted\",{nested},unquoted}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.Count);
            Assert.Equal("quoted", result[0]);
            Assert.Equal(',', result[1]);
            Assert.Equal("{nested}", result[2]);
            Assert.Equal(',', result[3]);
            Assert.Equal("unquoted", result[4]);
        }

        [Fact]
        public void TokenizeAttributes_HandlesMultipleKeyValuePairs()
        {
            // Arrange
            string input = "key1=value1,key2=value2,key3=value3}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(11, result.Count);
            Assert.Equal("key1", result[0]);
            Assert.Equal('=', result[1]);
            Assert.Equal("value1", result[2]);
            Assert.Equal(',', result[3]);
            Assert.Equal("key2", result[4]);
            Assert.Equal('=', result[5]);
            Assert.Equal("value2", result[6]);
            Assert.Equal(',', result[7]);
            Assert.Equal("key3", result[8]);
            Assert.Equal('=', result[9]);
            Assert.Equal("value3", result[10]);
        }

        [Fact]
        public void TokenizeAttributes_HandlesEscapedCharactersInQuotedString()
        {
            // Arrange
            string input = "\"{nested\\}} content\"}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("{nested\\}} content", result[0]);
        }

        [Fact]
        public void TokenizeAttributes_HandlesNestedMarkupExtensionsWithMultipleParameters()
        {
            // Arrange
            string input = "Stuff {Binding Path=Property,Source={StaticResource ResourceKey}}}";

            // Act
            ArrayList result = _tokenizer.TokenizeAttributes(input);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Stuff {Binding Path=Property,Source={StaticResource ResourceKey}}", result[0]);
        }

        #endregion
    }
}