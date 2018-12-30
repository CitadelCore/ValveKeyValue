using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ValveKeyValue.Deserialization
{
    internal class Kv1TokenReader : IDisposable
    {
        private const char QuotationMark = '"';
        private const char ObjectStart = '{';
        private const char ObjectEnd = '}';
        private const char CommentBegin = '/'; // Although Valve uses the double-slash convention, the KV spec allows for single-slash comments.
        private const char ConditionBegin = '[';
        private const char ConditionEnd = ']';
        private const char InclusionMark = '#';

        public Kv1TokenReader(TextReader textReader, KvSerializerOptions options)
        {
            Require.NotNull(textReader, nameof(textReader));
            Require.NotNull(options, nameof(options));

            _textReader = textReader;
            _options = options;
        }

        private readonly KvSerializerOptions _options;
        private TextReader _textReader;
        private bool _disposed;

        public KvToken ReadNextToken()
        {
            Require.NotDisposed(nameof(Kv1TokenReader), _disposed);
            SwallowWhitespace();

            var nextChar = Peek();
            if (IsEndOfFile(nextChar))
                return new KvToken(KvTokenType.EndOfFile);

            switch (nextChar)
            {
                case ObjectStart:
                    return ReadObjectStart();

                case ObjectEnd:
                    return ReadObjectEnd();

                case CommentBegin:
                    return ReadComment();

                case ConditionBegin:
                    return ReadCondition();

                case InclusionMark:
                    return ReadInclusion();

                default:
                    return ReadString();
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _textReader.Dispose();
            _textReader = null;

            _disposed = true;
        }

        private KvToken ReadString()
        {
            var text = ReadStringRaw();
            return new KvToken(KvTokenType.String, text);
        }

        private KvToken ReadObjectStart()
        {
            ReadChar(ObjectStart);
            return new KvToken(KvTokenType.ObjectStart);
        }

        private KvToken ReadObjectEnd()
        {
            ReadChar(ObjectEnd);
            return new KvToken(KvTokenType.ObjectEnd);
        }

        private KvToken ReadComment()
        {
            ReadChar(CommentBegin);

            if (Peek() == CommentBegin)
                Next();

            var text = _textReader.ReadLine();
            return new KvToken(KvTokenType.Comment, text);
        }

        private KvToken ReadCondition()
        {
            ReadChar(ConditionBegin);
            var text = ReadUntil(ConditionEnd);
            ReadChar(ConditionEnd);

            return new KvToken(KvTokenType.Condition, text);
        }

        private KvToken ReadInclusion()
        {
            ReadChar(InclusionMark);
            var term = ReadUntil(' ', '\t');
            var value = ReadStringRaw();

            if (string.Equals(term, "include", StringComparison.Ordinal))
            {
                return new KvToken(KvTokenType.IncludeAndAppend, value);
            }
            else if (string.Equals(term, "base", StringComparison.Ordinal))
            {
                return new KvToken(KvTokenType.IncludeAndMerge, value);
            }

            throw new InvalidDataException("Unrecognized term after '#' symbol.");
        }

        private char Next()
        {
            var next = _textReader.Read();
            if (next == -1)
                throw new EndOfStreamException();

            return (char)next;
        }

        private int Peek() => _textReader.Peek();

        private void ReadChar(char expectedChar)
        {
            var next = Next();
            if (next != expectedChar)
                throw MakeSyntaxException();
        }

        private string ReadUntil(params char[] terminators)
        {
            var sb = new StringBuilder();
            var escapeNext = false;

            var integerTerminators = new HashSet<int>(terminators.Select(t => (int)t));
            while (!integerTerminators.Contains(Peek()) || escapeNext)
            {
                var next = Next();

                if (next == '\\' && !escapeNext)
                {
                    escapeNext = true;
                    continue;
                }
                else if (escapeNext)
                {
                    escapeNext = false;

                    if (next == '"')
                    {
                        sb.Append('"');
                    }
                    else if (_options.HasEscapeSequences)
                    {
                        switch (next)
                        {
                            case 'r':
                                sb.Append('\r');
                                break;

                            case 'n':
                                sb.Append('\n');
                                break;

                            case 't':
                                sb.Append('\t');
                                break;

                            case '\\':
                                sb.Append('\\');
                                break;

                            default:
                                throw new InvalidDataException($"Unknown escaped character '\\{next}'.");
                        }
                    }
                    else
                    {
                        sb.Append('\\');

                        if (next == '\\')
                        {
                            escapeNext = true;
                        }
                        else
                        {
                            sb.Append(next);
                        }
                    }
                }
                else
                {
                    sb.Append(next);
                }
            }

            return sb.ToString();
        }

        private string ReadUntilWhitespaceOrBracket()
        {
            var sb = new StringBuilder();

            while (true)
            {
                var next = Peek();
                if (next == -1)
                    break;

                // Break if we hit a bracket or a quotation mark.
                // This avoids issues where a key is not properly spaced.
                // TODO: write unit test for this.
                var testChar = (char) next;
                if (char.IsWhiteSpace(testChar) ||
                    testChar == ObjectStart ||
                    testChar == ObjectEnd)
                    break;

                // Un-escaped quotation mark? Break.
                if (testChar == QuotationMark && sb[sb.Length - 1] != '\\')
                    break;

                sb.Append(Next());
            }

            return sb.ToString();
        }

        private void SwallowWhitespace()
        {
            while (PeekWhitespace())
                Next();
        }

        private bool PeekWhitespace()
        {
            var next = Peek();
            return !IsEndOfFile(next) && char.IsWhiteSpace((char)next);
        }

        private string ReadStringRaw()
        {
            SwallowWhitespace();
            return Peek() == '"' ? ReadQuotedStringRaw() : ReadUntilWhitespaceOrBracket();
        }

        private string ReadQuotedStringRaw()
        {
            ReadChar(QuotationMark);
            var text = ReadUntil(QuotationMark);
            ReadChar(QuotationMark);
            return text;
        }

        private static bool IsEndOfFile(int value) => value == -1;

        private static InvalidDataException MakeSyntaxException()
        {
            return new InvalidDataException("The syntax is incorrect.");
        }
    }
}
