using System;
using System.IO;
using System.Text;
using Force.Crc32;
using ValveKeyValue.Abstraction;

namespace ValveKeyValue.Deserialization
{
    internal class Kv1BinaryReader : IVisitingReader
    {
        public Kv1BinaryReader(Stream stream, IVisitationListener listener, KvSerializerOptions options)
        {
            Require.NotNull(stream, nameof(stream));
            Require.NotNull(listener, nameof(listener));

            _isVbkv = options.HasVbkvHeader;
            if (_isVbkv)
                _terminator = Kv1BinaryNodeType.EndVbkv;

            if (!stream.CanSeek)
                throw new ArgumentException("Stream must be seekable", nameof(stream));

            _stream = stream;
            _listener = listener;
            _reader = new BinaryReader(stream, Encoding.UTF8, true);
        }

        private readonly Stream _stream;
        private readonly BinaryReader _reader;
        private readonly IVisitationListener _listener;
        private bool _disposed;

        private readonly bool _isVbkv;
        private readonly Kv1BinaryNodeType _terminator = Kv1BinaryNodeType.End;

        public void ReadObject()
        {
            Require.NotDisposed(nameof(Kv1TextReader), _disposed);
            
            try
            {
                // If we have a VBKV, read the header and validate the checksum.
                if (_isVbkv)
                    ReadVbkvHeader();

                ReadObjectCore();
            }
            catch (IOException ex)
            {
                throw new KeyValueException("Error while reading binary KeyValues.", ex);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new KeyValueException("Error while parsing binary KeyValues.", ex);
            }
            catch (NotSupportedException ex)
            {
                throw new KeyValueException("Error while parsing binary KeyValues.", ex);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _reader.Dispose();
            _disposed = true;
        }

        private void ReadVbkvHeader()
        {
            var header = new string(_reader.ReadChars(4));
            if (header != "VBKV")
                throw new KeyValueException("VBKV format was specified but got an invalid header.");
            var checksum = _reader.ReadUInt32();

            // Verify the CRC32 checksum
            using (var ms = new MemoryStream())
            {
                var pos = _stream.Position;
                _stream.CopyTo(ms);
                var array = ms.ToArray();

                // compute the crc
                var crc = Crc32Algorithm.Compute(array);
                if (checksum != crc)
                    throw new KeyValueException("Failed to validate the CRC32 checksum.");
                _stream.Seek(pos, SeekOrigin.Begin);
            }
        }

        private void ReadObjectCore()
        {
            Kv1BinaryNodeType type;

            // Keep reading values, until we reach the terminator
            while ((type = ReadNextNodeType()) != _terminator)
                ReadValue(type);
        }

        private void ReadValue(Kv1BinaryNodeType type)
        {
            var name = Encoding.UTF8.GetString(ReadNullTerminatedBytes());
            KvValue value;

            switch (type)
            {
                case Kv1BinaryNodeType.ChildObject:
                    _listener.OnObjectStart(name);
                    ReadObjectCore();
                    _listener.OnObjectEnd();
                    return;

                case Kv1BinaryNodeType.String:
                    // UTF8 encoding is used for string values
                    value = new KvObjectValue<string>(Encoding.UTF8.GetString(ReadNullTerminatedBytes()), KvValueType.String);
                    break;

                case Kv1BinaryNodeType.WideString:
                    throw new NotSupportedException("Wide String is not supported.");

                case Kv1BinaryNodeType.Int32:
                case Kv1BinaryNodeType.Color:
                case Kv1BinaryNodeType.Pointer:
                    value = new KvObjectValue<int>(_reader.ReadInt32(), KvValueType.Int32);
                    break;

                case Kv1BinaryNodeType.UInt64:
                    value = new KvObjectValue<ulong>(_reader.ReadUInt64(), KvValueType.UInt64);
                    break;

                case Kv1BinaryNodeType.Float32:
                    var floatValue = BitConverter.ToSingle(_reader.ReadBytes(4), 0);
                    value = new KvObjectValue<float>(floatValue, KvValueType.FloatingPoint);
                    break;

                case Kv1BinaryNodeType.Int64:
                    value = new KvObjectValue<long>(_reader.ReadInt64(), KvValueType.Int64);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }

            _listener.OnKeyValuePair(name, value);
        }

        private byte[] ReadNullTerminatedBytes()
        {
            using (var mem = new MemoryStream())
            {
                byte nextByte;
                while ((nextByte = _reader.ReadByte()) != 0)
                    mem.WriteByte(nextByte);
                return mem.ToArray();
            }
        }

        private Kv1BinaryNodeType ReadNextNodeType()
            => (Kv1BinaryNodeType)_reader.ReadByte();
    }
}
