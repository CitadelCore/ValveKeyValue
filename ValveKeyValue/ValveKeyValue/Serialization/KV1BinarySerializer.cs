using System;
using System.IO;
using System.Text;
using Force.Crc32;
using ValveKeyValue.Abstraction;

namespace ValveKeyValue.Serialization
{
    internal sealed class Kv1BinarySerializer : IVisitationListener
    {
        public Kv1BinarySerializer(Stream stream, KvSerializerOptions options)
        {
            Require.NotNull(stream, nameof(stream));

            _isVbkv = options.HasVbkvHeader;
            if (_isVbkv)
                _terminator = Kv1BinaryNodeType.EndVbkv;

            _writer = new BinaryWriter(new MemoryStream(), Encoding.UTF8);
            _realWriter = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
        }

        private readonly bool _isVbkv;
        private readonly Kv1BinaryNodeType _terminator = Kv1BinaryNodeType.End;
        private readonly BinaryWriter _realWriter;

        private readonly BinaryWriter _writer;
        private int _objectDepth;

        public void Dispose()
        {
            _writer.Dispose();
            _realWriter.Dispose();
        }

        private void WriteVbkvHeader()
        {
            // Writing the VBKV header is done in a slightly different way.
            // We use a secondary "wrapper" BinaryWriter that writes to a MemoryStream.
            // This is so we can calculate the CRC hash and write it before the KeyValues1 data.
            var mem = (MemoryStream) _writer.BaseStream;
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
            _objectDepth++;
            Write(Kv1BinaryNodeType.ChildObject);
            WriteNullTerminatedBytes(Encoding.UTF8.GetBytes(name));
        }

        public void OnObjectEnd()
        {
            Write(_terminator);

            _objectDepth--;
            if (_objectDepth != 0) return;

            Write(_terminator);
            if (!_isVbkv)
            {
                // No VBKV data, so just copy the stream
                _writer.BaseStream.Seek(0, SeekOrigin.Begin);
                _writer.BaseStream.CopyTo(_realWriter.BaseStream);
            }
            else
            {
                // VBKV, do more stuff
                WriteVbkvHeader();
            }
        }

        public void OnKeyValuePair(string name, KvValue value)
        {
            Write(GetNodeType(value.ValueType));
            WriteNullTerminatedBytes(Encoding.UTF8.GetBytes(name));

            switch (value.ValueType)
            {
                case KvValueType.FloatingPoint:
                    _writer.Write((float)value);
                    break;

                case KvValueType.Int32:
                case KvValueType.Pointer:
                    _writer.Write((int)value);
                    break;

                case KvValueType.String:
                    WriteNullTerminatedBytes(Encoding.UTF8.GetBytes((string)value));
                    break;

                case KvValueType.UInt64:
                    _writer.Write((ulong)value);
                    break;

                case KvValueType.Int64:
                    _writer.Write((long)value);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(value.ValueType), value.ValueType, "Value was of an unsupported type.");
            }
        }

        private void Write(Kv1BinaryNodeType nodeType)
        {
            _writer.Write((byte)nodeType);
        }

        private void WriteNullTerminatedBytes(byte[] value)
        {
            _writer.Write(value);
            _writer.Write((byte)0);
        }

        private static Kv1BinaryNodeType GetNodeType(KvValueType type)
        {
            switch (type)
            {
                case KvValueType.FloatingPoint:
                    return Kv1BinaryNodeType.Float32;
                case KvValueType.Int32:
                    return Kv1BinaryNodeType.Int32;
                case KvValueType.Pointer:
                    return Kv1BinaryNodeType.Pointer;
                case KvValueType.String:
                    return Kv1BinaryNodeType.String;
                case KvValueType.UInt64:
                    return Kv1BinaryNodeType.UInt64;
                case KvValueType.Int64:
                    return Kv1BinaryNodeType.Int64;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported value type.");
            }
        }
    }
}
