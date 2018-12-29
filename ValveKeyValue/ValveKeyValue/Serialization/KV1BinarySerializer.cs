using System;
using System.IO;
using System.Text;
using Force.Crc32;
using ValveKeyValue.Abstraction;

namespace ValveKeyValue.Serialization
{
    sealed class KV1BinarySerializer : IVisitationListener, IDisposable
    {
        public KV1BinarySerializer(Stream stream, KVSerializerOptions options)
        {
            Require.NotNull(stream, nameof(stream));

            _isVbkv = options.HasVbkvHeader;
            if (_isVbkv)
                _terminator = KV1BinaryNodeType.EndVbkv;

            writer = new BinaryWriter(new MemoryStream(), Encoding.UTF8);
            _realWriter = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
        }

        private readonly bool _isVbkv;
        private readonly KV1BinaryNodeType _terminator = KV1BinaryNodeType.End;
        private readonly BinaryWriter _realWriter;

        readonly BinaryWriter writer;
        int objectDepth;

        public void Dispose()
        {
            writer.Dispose();
            _realWriter.Dispose();
        }

        private void WriteVbkvHeader()
        {
            // Writing the VBKV header is done in a slightly different way.
            // We use a secondary "wrapper" BinaryWriter that writes to a MemoryStream.
            // This is so we can calculate the CRC hash and write it before the KeyValues1 data.
            var mem = (MemoryStream) writer.BaseStream;
            mem.Seek(0, SeekOrigin.Begin);
            var array = mem.ToArray();

            // Compute CRC32 of the array
            var crc = Crc32Algorithm.Compute(array);

            // Write to the real stream
            _realWriter.Write("VBKV".ToCharArray());
            _realWriter.Write(crc);
            _realWriter.Write(array);
        }

        public void OnObjectStart(string name)
        {
            objectDepth++;
            Write(KV1BinaryNodeType.ChildObject);
            WriteNullTerminatedBytes(Encoding.UTF8.GetBytes(name));
        }

        public void OnObjectEnd()
        {
            Write(_terminator);

            objectDepth--;
            if (objectDepth != 0) return;

            Write(_terminator);
            if (!_isVbkv)
            {
                // No VBKV data, so just copy the stream
                writer.BaseStream.Seek(0, SeekOrigin.Begin);
                writer.BaseStream.CopyTo(_realWriter.BaseStream);
            }
            else
            {
                // VBKV, do more stuff
                WriteVbkvHeader();
            }
        }

        public void OnKeyValuePair(string name, KVValue value)
        {
            Write(GetNodeType(value.ValueType));
            WriteNullTerminatedBytes(Encoding.UTF8.GetBytes(name));

            switch (value.ValueType)
            {
                case KVValueType.FloatingPoint:
                    writer.Write((float)value);
                    break;

                case KVValueType.Int32:
                case KVValueType.Pointer:
                    writer.Write((int)value);
                    break;

                case KVValueType.String:
                    WriteNullTerminatedBytes(Encoding.UTF8.GetBytes((string)value));
                    break;

                case KVValueType.UInt64:
                    writer.Write((ulong)value);
                    break;

                case KVValueType.Int64:
                    writer.Write((long)value);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(value.ValueType), value.ValueType, "Value was of an unsupported type.");
            }
        }

        void Write(KV1BinaryNodeType nodeType)
        {
            writer.Write((byte)nodeType);
        }

        void WriteNullTerminatedBytes(byte[] value)
        {
            writer.Write(value);
            writer.Write((byte)0);
        }

        static KV1BinaryNodeType GetNodeType(KVValueType type)
        {
            switch (type)
            {
                case KVValueType.FloatingPoint:
                    return KV1BinaryNodeType.Float32;

                case KVValueType.Int32:
                    return KV1BinaryNodeType.Int32;

                case KVValueType.Pointer:
                    return KV1BinaryNodeType.Pointer;

                case KVValueType.String:
                    return KV1BinaryNodeType.String;

                case KVValueType.UInt64:
                    return KV1BinaryNodeType.UInt64;

                case KVValueType.Int64:
                    return KV1BinaryNodeType.Int64;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported value type.");
            }
        }
    }
}
