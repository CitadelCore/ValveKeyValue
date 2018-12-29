using System;
using System.Globalization;
using System.IO;
using ValveKeyValue.Abstraction;

namespace ValveKeyValue.Deserialization
{
    internal sealed class Kv1TextReader : IVisitingReader
    {
        public Kv1TextReader(TextReader textReader, IParsingVisitationListener listener, KvSerializerOptions options)
        {
            Require.NotNull(textReader, nameof(textReader));
            Require.NotNull(listener, nameof(listener));
            Require.NotNull(options, nameof(options));

            _listener = listener;
            _options = options;

            _conditionEvaluator = new KvConditionEvaluator(options.Conditions);
            _tokenReader = new Kv1TokenReader(textReader, options);
            _stateMachine = new Kv1TextReaderStateMachine();
        }

        private readonly IParsingVisitationListener _listener;
        private readonly KvSerializerOptions _options;

        private readonly KvConditionEvaluator _conditionEvaluator;
        private readonly Kv1TokenReader _tokenReader;
        private readonly Kv1TextReaderStateMachine _stateMachine;
        private bool _disposed;

        public void ReadObject()
        {
            Require.NotDisposed(nameof(Kv1TextReader), _disposed);

            while (_stateMachine.IsInObject)
            {
                KvToken token;

                try
                {
                    token = _tokenReader.ReadNextToken();
                }
                catch (InvalidDataException ex)
                {
                    throw new KeyValueException(ex.Message, ex);
                }
                catch (EndOfStreamException ex)
                {
                    throw new KeyValueException("Found end of file while trying to read token.", ex);
                }

                switch (token.TokenType)
                {
                    case KvTokenType.String:
                        ReadText(token.Value);
                        break;

                    case KvTokenType.ObjectStart:
                        BeginNewObject();
                        break;

                    case KvTokenType.ObjectEnd:
                        FinalizeCurrentObject(true);
                        break;

                    case KvTokenType.Condition:
                        HandleCondition(token.Value);
                        break;

                    case KvTokenType.EndOfFile:
                        try
                        {
                            FinalizeDocument();
                        }
                        catch (InvalidOperationException ex)
                        {
                            throw new KeyValueException("Found end of file when another token type was expected.", ex);
                        }

                        break;

                    case KvTokenType.Comment:
                        break;

                    case KvTokenType.IncludeAndMerge:
                        if (!_stateMachine.IsAtStart)
                        {
                            throw new KeyValueException("Inclusions are only valid at the beginning of a file.");
                        }

                        _stateMachine.AddItemForMerging(token.Value);
                        break;

                    case KvTokenType.IncludeAndAppend:
                        if (!_stateMachine.IsAtStart)
                        {
                            throw new KeyValueException("Inclusions are only valid at the beginning of a file.");
                        }

                        _stateMachine.AddItemForAppending(token.Value);
                        break;

                    default:
                        throw new NotImplementedException("The developer forgot to handle a KVTokenType.");
                }
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _tokenReader.Dispose();
            _disposed = true;
        }

        private void ReadText(string text)
        {
            switch (_stateMachine.Current)
            {
                // If we're after a value when we find more text, then we must be starting a new key/value pair.
                case Kv1TextReaderState.InObjectAfterValue:
                    FinalizeCurrentObject(@explicit: false);
                    _stateMachine.PushObject();
                    SetObjectKey(text);
                    break;

                case Kv1TextReaderState.InObjectBeforeKey:
                    SetObjectKey(text);
                    break;

                case Kv1TextReaderState.InObjectBetweenKeyAndValue:
                    var value = ParseValue(text);
                    var name = _stateMachine.CurrentName;
                    _listener.OnKeyValuePair(name, value);

                    _stateMachine.Push(Kv1TextReaderState.InObjectAfterValue);
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        private void SetObjectKey(string name)
        {
            _stateMachine.SetName(name);
            _stateMachine.Push(Kv1TextReaderState.InObjectBetweenKeyAndValue);
        }

        private void BeginNewObject()
        {
            if (_stateMachine.Current != Kv1TextReaderState.InObjectBetweenKeyAndValue)
            {
                throw new InvalidOperationException();
            }

            _listener.OnObjectStart(_stateMachine.CurrentName);

            _stateMachine.PushObject();
            _stateMachine.Push(Kv1TextReaderState.InObjectBeforeKey);
        }

        private void FinalizeCurrentObject(bool @explicit)
        {
            if (_stateMachine.Current != Kv1TextReaderState.InObjectBeforeKey && _stateMachine.Current != Kv1TextReaderState.InObjectAfterValue)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Attempted to finalize object while in state {0}.",
                        _stateMachine.Current));
            }

            _stateMachine.PopObject(out var discard);

            if (_stateMachine.IsInObject)
            {
                _stateMachine.Push(Kv1TextReaderState.InObjectAfterValue);
            }

            if (discard)
            {
                _listener.DiscardCurrentObject();
            }
            else if (@explicit)
            {
                _listener.OnObjectEnd();
            }
        }

        private void FinalizeDocument()
        {
            FinalizeCurrentObject(true);

            if (_stateMachine.IsInObject)
            {
                throw new InvalidOperationException("Inconsistent state - at end of file whilst inside an object.");
            }

            foreach (var includedForMerge in _stateMachine.ItemsForMerging)
            {
                DoIncludeAndMerge(includedForMerge);
            }

            foreach (var includedDocument in _stateMachine.ItemsForAppending)
            {
                DoIncludeAndAppend(includedDocument);
            }
        }

        private void HandleCondition(string text)
        {
            if (_stateMachine.Current != Kv1TextReaderState.InObjectAfterValue)
            {
                throw new InvalidDataException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Found conditional while in state {0}.",
                        _stateMachine.Current));
            }

            if (!_conditionEvaluator.Evalute(text))
            {
                _stateMachine.SetDiscardCurrent();
            }
        }

        private void DoIncludeAndMerge(string filePath)
        {
            var mergeListener = _listener.GetMergeListener();

            using (var stream = OpenFileForInclude(filePath))
            using (var reader = new Kv1TextReader(new StreamReader(stream), mergeListener, _options))
            {
                reader.ReadObject();
            }
        }

        private void DoIncludeAndAppend(string filePath)
        {
            var appendListener = _listener.GetAppendListener();

            using (var stream = OpenFileForInclude(filePath))
            using (var reader = new Kv1TextReader(new StreamReader(stream), appendListener, _options))
            {
                reader.ReadObject();
            }
        }

        private Stream OpenFileForInclude(string filePath)
        {
            if (_options.FileLoader == null)
            {
                throw new KeyValueException("Inclusions requirer a FileLoader to be provided in KVSerializerOptions.");
            }

            var stream = _options.FileLoader.OpenFile(filePath);
            if (stream == null)
            {
                throw new KeyValueException("IIncludedFileLoader returned null for included file path.");
            }

            return stream;
        }

        private static KvValue ParseValue(string text)
        {
            // "0x" + 2 digits per byte. Long is 8 bytes, so s + 16 = 18.
            // Expressed this way for readability, rather than using a magic value.
            const int hexStringLengthForUnsignedLong = 2 + (sizeof(long) * 2);

            if (text.Length == hexStringLengthForUnsignedLong && text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                var hexadecimalString = text.Substring(2);
                var data = ParseHexStringAsByteArray(hexadecimalString);

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(data);
                }

                var value = BitConverter.ToUInt64(data, 0);
                return new KvObjectValue<ulong>(value, KvValueType.UInt64);
            }

            if (int.TryParse(text, out var intValue))
            {
                return new KvObjectValue<int>(intValue, KvValueType.Int32);
            }

            const NumberStyles floatingPointNumberStyles =
                NumberStyles.AllowDecimalPoint |
                NumberStyles.AllowExponent |
                NumberStyles.AllowLeadingSign;

            if (float.TryParse(text, floatingPointNumberStyles, CultureInfo.InvariantCulture, out var floatValue))
                return new KvObjectValue<float>(floatValue, KvValueType.FloatingPoint);

            return new KvObjectValue<string>(text, KvValueType.String);
        }

        private static byte[] ParseHexStringAsByteArray(string hexadecimalRepresentation)
        {
            Require.NotNull(hexadecimalRepresentation, nameof(hexadecimalRepresentation));

            var data = new byte[hexadecimalRepresentation.Length / 2];
            for (var i = 0; i < data.Length; i++)
            {
                var currentByteText = hexadecimalRepresentation.Substring(i * 2, 2);
                data[i] = byte.Parse(currentByteText, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return data;
        }
    }
}
