using System;
using System.IO;
using System.Text;
using ValveKeyValue.Abstraction;

namespace ValveKeyValue.Serialization
{
    internal sealed class Kv1TextSerializer : IVisitationListener
    {
        public Kv1TextSerializer(Stream stream, KvSerializerOptions options)
        {
            Require.NotNull(stream, nameof(stream));
            Require.NotNull(options, nameof(options));

            _options = options;
            _writer = new StreamWriter(stream, Encoding.UTF8, 1024, true) {NewLine = "\n"};
        }

        private readonly KvSerializerOptions _options;
        private readonly TextWriter _writer;
        private int _indentation;

        public void Dispose()
        {
            _writer.Dispose();
        }

        public void OnObjectStart(string name)
            => WriteStartObject(name);

        public void OnObjectEnd()
            => WriteEndObject();

        public void OnKeyValuePair(string name, KvValue value)
            => WriteKeyValuePair(name, value);

        private void WriteStartObject(string name)
        {
            WriteIndentation();
            WriteText(name);
            WriteLine();
            WriteIndentation();
            _writer.Write('{');
            _indentation++;
            WriteLine();
        }

        private void WriteEndObject()
        {
            _indentation--;
            WriteIndentation();
            _writer.Write('}');
            _writer.WriteLine();
        }

        private void WriteKeyValuePair(string name, IConvertible value)
        {
            WriteIndentation();
            WriteText(name);
            _writer.Write('\t');
            WriteText(value.ToString(null));
            WriteLine();
        }

        private void WriteIndentation()
        {
            if (_indentation == 0)
            {
                return;
            }

            var text = new string('\t', _indentation);
            _writer.Write(text);
        }

        private void WriteText(string text)
        {
            _writer.Write('"');

            foreach (var @char in text)
            {
                switch (@char)
                {
                    case '"':
                        _writer.Write("\\\"");
                        break;

                    case '\\':
                        _writer.Write(_options.HasEscapeSequences ? "\\\\" : "\\");
                        break;

                    default:
                        _writer.Write(@char);
                        break;
                }
            }

            _writer.Write('"');
        }

        private void WriteLine()
        {
            _writer.WriteLine();
        }
    }
}
