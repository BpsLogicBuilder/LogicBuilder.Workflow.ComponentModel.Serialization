using System;
using System.Collections;
using System.Text;

namespace LogicBuilder.Workflow.ComponentModel.Serialization
{
    internal class AttributesTokenizer : IAttributesTokenizer
    {
        private ArrayList list = null;
        private int length = 0;
        private bool inQuotes = false;
        private bool gotEscape = false;
        private bool nonWhitespaceFound = false;
        private char quoteChar = '\'';
        private int leftCurlies = 0;
        private bool collectionIndexer = false;
        private StringBuilder stringBuilder = null;
        private int i = 0;
        private enum LoopControl
        {
            Continue,
            Break,
            Error
        }

        // This function splits the argument string into an array of tokens.
        // For example: ID=Workflow1, Path=error1} would become an array that contains the following elements
        // {ID} {=} {Workflwo1} {,} {Path} {=} {error1}
        // Note that the input string should start with the first argument and end with '}'.
        public ArrayList TokenizeAttributes(string args)
        {
            length = args.Length;

            // Loop through the args, creating a list of arguments and known delimiters.
            // This loop does limited syntax checking, and serves to tokenize the argument
            // string into chunks that are validated in greater detail in the next phase.
            for (; i < length; i++)
            {
                // Escape character is always in effect for everything inside
                // a MarkupExtension.  We have to remember that the next character is 
                // escaped, and is not treated as a quote or delimiter.
                if (CanSetGotEscapeToTrue(args))
                {
                    gotEscape = true;
                    continue;
                }

                if (CanSetNonWhitespaceFoundToTrue(args))
                    nonWhitespaceFound = true;

                // Process all characters that are not whitespace or are between quotes
                if (NotCharacterForProcessing())
                    continue;

                // We have a non-whitespace character, so ensure we have
                // a string builder to accumulate characters and a list to collect
                // attributes and delimiters.  These are lazily
                // created so that simple cases that have no parameters do not
                // create any extra objects.
                if (stringBuilder == null)
                {
                    stringBuilder = new StringBuilder(length);
                    list = new ArrayList(1);
                }

                var loopControl = ProcessChar(args);
                if (loopControl == LoopControl.Break)
                    break;
                else if (loopControl == LoopControl.Error)
                    return [];
            }

            ValidateTheStringBuilder(args);

            return list;
        }

        private LoopControl ProcessChar(string args)
        {
            // If the character is escaped, then it is part of the attribute
            // being collected, regardless of its value and is not treated as
            // a delimiter or special character.  Write back the escape
            // character since downstream processing will need it to determine
            // whether the value is a MarkupExtension or not, and to prevent
            // multiple escapes from being lost by recursive processing.
            if (gotEscape)
            {
                stringBuilder.Append('\\');
                stringBuilder.Append(args[i]);
                gotEscape = false;
                return LoopControl.Continue;
            }

            // If this characters is not escaped, then look for quotes and
            // delimiters.
            if (InsideQuotesOrCurlies())
            {
                ProcessInQuotesAndCurlyBraces(args);
                return LoopControl.Continue;
            }

            if (IsQuoteCharacter(args))
            {
                if (!ProcessQuoteChars(args))
                    return LoopControl.Error;
            }
            else if (IsDelimiter(args))
            {
                if (!ProcessDelimiters(args))
                    return LoopControl.Error;
            }
            else if (args[i] == '}')
            {
                if (!ProcessRightCurly())
                    return LoopControl.Error;

                return LoopControl.Break;
            }
            else
            {
                ProcessPlainCharacter(args);
            }

            return LoopControl.Continue;
        }

        private bool CanSetNonWhitespaceFoundToTrue(string args)
        {
            return !nonWhitespaceFound && !char.IsWhiteSpace(args[i]);
        }

        private bool CanSetGotEscapeToTrue(string args)
        {
            return !gotEscape && args[i] == '\\';
        }

        private bool IsDelimiter(string args)
        {
            return args[i] == ',' || args[i] == '=';
        }

        private bool IsQuoteCharacter(string args)
        {
            return (args[i] == '"' || args[i] == '\'');
        }

        private bool InsideQuotesOrCurlies()
        {
            return inQuotes || leftCurlies > 0;
        }

        private bool NotCharacterForProcessing()
        {
            return !inQuotes && leftCurlies <= 0 && !nonWhitespaceFound;
        }

        private void ValidateTheStringBuilder(string args)
        {
            // If we've accumulated content but haven't hit a terminating '}' then the
            // format is bad, so complain.
            if (stringBuilder != null && stringBuilder.Length > 0)
                throw new InvalidOperationException(SR.GetString(SR.Error_MarkupExtensionMissingTerminatingCharacter));
            else if (i < length)
            {
                // If there is non-whitespace text left that we haven't processes yet, 
                // then there is junk after the closing '}', so complain
                for (++i; i < length; i++)
                {
                    if (!Char.IsWhiteSpace(args[i]))
                        throw new InvalidOperationException(SR.GetString(SR.Error_ExtraCharacterFoundAtEnd));
                }
            }
        }

        private void ProcessPlainCharacter(string args)
        {
            if (args[i] == '{')
            {
                leftCurlies++;
            }
            // Must just be a plain old character, so add it to the stringbuilder
            stringBuilder.Append(args[i]);
        }

        private bool ProcessRightCurly()
        {
            // If we hit the outside right curly brace, then end processing.  If
            // there is a delimiter on the top of the stack and we haven't
            // hit another non-whitespace character, then its an error
            if (stringBuilder != null)
            {
                if (stringBuilder.Length > 0)
                {
                    list.Add(stringBuilder.ToString().Trim());
                    stringBuilder.Length = 0;
                }
                else if (list.Count > 0 && (list[list.Count - 1] is char))
                    return false;
            }

            return true;
        }

        private bool ProcessDelimiters(string args)
        {
            // If there is something in the stringbuilder, then store it
            if (stringBuilder != null && stringBuilder.Length > 0)
            {
                list.Add(stringBuilder.ToString().Trim());
                stringBuilder.Length = 0;
            }
            else if (list.Count == 0 || list[list.Count - 1] is Char)
            {
                // Can't have two delimiters in a row, so check what is on
                // the list and complain if the last item is a character, or if
                // a delimiter is the first item.
                return false;
            }

            // Append known delimiters.
            list.Add(args[i]);
            nonWhitespaceFound = false;
            return true;
        }

        private bool ProcessQuoteChars(string args)
        {
            // If we're not inside quotes, then a start quote can only
            // occur as the first non-whitespace character in a name or value.
            if (collectionIndexer && i < args.Length - 1 && args[i + 1] == ']')
            {
                collectionIndexer = false;
                stringBuilder.Append(args[i]);
            }
            else if (i > 0 && args[i - 1] == '[')
            {
                collectionIndexer = true;
                stringBuilder.Append(args[i]);
            }
            else
            {
                if (stringBuilder.Length != 0)
                    return false;

                inQuotes = true;
                quoteChar = args[i];
            }

            return true;
        }

        private void ProcessInQuotesAndCurlyBraces(string args)
        {
            if (inQuotes && args[i] == quoteChar)
            {
                // If we're inside quotes, then only an end quote that is not
                // escaped is special, and will act as a delimiter.
                inQuotes = false;
                list.Add(stringBuilder.ToString());
                stringBuilder.Length = 0;
                nonWhitespaceFound = false;
            }
            else
            {
                if (leftCurlies > 0 && args[i] == '}')
                {
                    leftCurlies--;
                }
                else if (args[i] == '{')
                {
                    leftCurlies++;
                }
                stringBuilder.Append(args[i]);
            }
        }
    }
}
