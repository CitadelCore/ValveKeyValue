using System;
using System.IO;
using System.Text;
using Force.Crc32;
using ValveKeyValue.Abstraction;

namespace ValveKeyValue.Deserialization
{
    class KV1BinaryReader : IVisitingReader
    {
        public KV1BinaryReader(Stream stream, IVisitationListener listener, KVSerializerOptions options)
        {
            Require.NotNull(stream, nameof(stream));
            Require.NotNull(listener, nameof(listener));

            _isVbkv = options.HasVbkvHeader;
            if (_isVbkv)
                _terminator = KV1BinaryNodeType.EndVbkv;

            if (!stream.CanSeek)
            {
                throw new ArgumentException("Stream must be seekable", nameof(stream));
            }

            this.stream = stream;
            this.listener = listener;
            reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
        }

        readonly Stream stream;
        readonly BinaryReader reader;
        readonly IVisitationListener listener;
        bool disposed;

        private readonly bool _isVbkv;
        private readonly KV1BinaryNodeType _terminator = KV1BinaryNodeType.End;

        public void ReadObject()
        {
            Require.NotDisposed(nameof(KV1TextReader), disposed);
            
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
            if (!disposed)
            {
                reader.Dispose();
                disposed = true;
            }
        }

        void ReadVbkvHeader()
        {
            var header = new string(reader.ReadChars(4));
            if (header != "VBKV")
                throw new KeyValueException("VBKV format was specified but got an invalid header.");
            var checksum = reader.ReadUInt32();

            // Verify the CRC32 checksum
            using (var ms = new MemoryStream())
            {
                var pos = stream.Position;
                stream.CopyTo(ms);
                var array = ms.ToArray();

                // compute the crc
                var crc = Crc32Algorithm.Compute(array);
                if (checksum != crc)
                    throw new KeyValueException("Failed to validate the CRC32 checksum.");
                stream.Seek(pos, SeekOrigin.Begin);
            }
        }

        void ReadObjectCore()
        {
            var header = new string(reader.ReadChars(4));
            if (header != "VBKV")
                throw new KeyValueException("VBKV format was specified but got an invalid header.");
            var checksum = reader.ReadUInt32();

            // Verify the CRC32 checksum
            using (var ms = new MemoryStream())
            {
                var pos = stream.Position;
                stream.CopyTo(ms);
                var array = ms.ToArray();

                // compute the crc
                var crc = Crc32Algorithm.Compute(array);
                if (checksum != crc)
                    throw new KeyValueException("Failed to validate the CRC32 checksum.");
                stream.Seek(pos, SeekOrigin.Begin);
            }
        }

        void ReadObjectCore()
        {
            KV1BinaryNodeType type;

            // Keep reading values, until we reach the terminator
            while ((type = ReadNextNodeType()) != KV1BinaryNodeType.End)
            {
                ReadValue(type);
            }
        }
        
        void ReadValue(KV1BinaryNodeType type)
        {
            var name = Encoding.UTF8.GetString(ReadNullTerminatedBytes());
            KVValue value;

            switch (type)
            {
                case KV1BinaryNodeType.ChildObject:
                    listener.OnObjectStart(name);
                    ReadObjectCore();
                    listener.OnObjectEnd();
                    return;

                case KV1BinaryNodeType.String:
                    // UTF8 encoding is used for string values
                    value = new KVObjectValue<string>(Encoding.UTF8.GetString(ReadNullTerminatedBytes()), KVValueType.String);
                    break;

                case KV1BinaryNodeType.WideString:
                    throw new NotSupportedException("Wide String is not supported.");

                case KV1BinaryNodeType.Int32:
                case KV1BinaryNodeType.Color:
                case KV1BinaryNodeType.Pointer:
                    value = new KVObjectValue<int>(reader.ReadInt32(), KVValueType.Int32);
                    break;

                case KV1BinaryNodeType.UInt64:
                    value = new KVObjectValue<ulong>(reader.ReadUInt64(), KVValueType.UInt64);
                    break;

                case KV1BinaryNodeType.Float32:
                    var floatValue = BitConverter.ToSingle(reader.ReadBytes(4), 0);
                    value = new KVObjectValue<float>(floatValue, KVValueType.FloatingPoint);
                    break;

                case KV1BinaryNodeType.Int64:
                    value = new KVObjectValue<long>(reader.ReadInt64(), KVValueType.Int64);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }

            listener.OnKeyValuePair(name, value);
        }

        byte[] ReadNullTerminatedBytes()
        {
            using (var mem = new MemoryStream())
            {
                byte nextByte;
                while ((nextByte = reader.ReadByte()) != 0)
                {
                    mem.WriteByte(nextByte);
                }

                return mem.ToArray();
            }
        }

        KV1BinaryNodeType ReadNextNodeType()
            => (KV1BinaryNodeType)reader.ReadByte();
    }
}
