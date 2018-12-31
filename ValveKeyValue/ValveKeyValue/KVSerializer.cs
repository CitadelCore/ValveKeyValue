using System;
using System.IO;
using ValveKeyValue.Abstraction;
using ValveKeyValue.Deserialization;
using ValveKeyValue.Serialization;

namespace ValveKeyValue
{
    /// <summary>
    /// Helper class to easily deserialize KeyValue objects.
    /// </summary>
    public class KvSerializer
    {
        private KvSerializer(KvSerializationFormat format)
        {
            _format = format;
        }

        private readonly KvSerializationFormat _format;

        /// <summary>
        /// Creates a new <see cref="KvSerializer"/> for the given format.
        /// </summary>
        /// <param name="format">The <see cref="KvSerializationFormat"/> to use when (de)serializing. </param>
        /// <returns>A new <see cref="KvSerializer"/> that (de)serializes with the given format.</returns>
        public static KvSerializer Create(KvSerializationFormat format)
            => new KvSerializer(format);

        /// <summary>
        /// Deserializes a KeyValue object from a stream.
        /// </summary>
        /// <param name="stream">The stream to deserialize from.</param>
        /// <param name="options">Options to use that can influence the deserialization process.</param>
        /// <returns>A <see cref="KvObject"/> representing the KeyValues structure encoded in the stream.</returns>
        public KvObject Deserialize(Stream stream, KvSerializerOptions options = null)
        {
            Require.NotNull(stream, nameof(stream));
            var builder = new KvObjectBuilder();

            using (var reader = MakeReader(stream, builder, options ?? KvSerializerOptions.DefaultOptions))
                reader.ReadObject();

            return builder.GetObject();
        }

        /// <summary>
        /// Deserializes an object from a KeyValues representation in a stream.
        /// </summary>
        /// <param name="stream">The stream to deserialize from.</param>
        /// <param name="options">Options to use that can influence the deserialization process.</param>
        /// <returns>A <typeparamref name="TObject" /> instance representing the KeyValues structure in the stream.</returns>
        /// <typeparam name="TObject">The type of object to deserialize.</typeparam>;
        public TObject Deserialize<TObject>(Stream stream, KvSerializerOptions options = null)
        {
            Require.NotNull(stream, nameof(stream));

            var @object = Deserialize(stream, options ?? KvSerializerOptions.DefaultOptions);
            var typedObject = ObjectCopier.MakeObject<TObject>(@object);
            return typedObject;
        }

        /// <summary>
        /// Serializes a KeyValue object into stream.
        /// </summary>
        /// <param name="stream">The stream to serialize into.</param>
        /// <param name="data">The data to serialize.</param>
        /// <param name="options">Options to use that can influence the serialization process.</param>
        public void Serialize(Stream stream, KvObject data, KvSerializerOptions options = null)
        {
            using (var serializer = MakeSerializer(stream, options ?? KvSerializerOptions.DefaultOptions))
            {
                var visitor = new KvObjectVisitor(serializer);
                visitor.Visit(data);
            }
        }

        /// <summary>
        /// Serializes a KeyValue object into stream in plain text..
        /// </summary>
        /// <param name="stream">The stream to serialize into.</param>
        /// <param name="data">The data to serialize.</param>
        /// <param name="name">The top-level object name</param>
        /// <param name="options">Options to use that can influence the serialization process.</param>
        /// <typeparam name="TData">The type of object to serialize.</typeparam>
        public void Serialize<TData>(Stream stream, TData data, string name, KvSerializerOptions options = null)
        {
            Require.NotNull(stream, nameof(stream));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            Require.NotNull(name, nameof(name));

            var kvObjectTree = ObjectCopier.FromObject(data, name);
            Serialize(stream, kvObjectTree, options);
        }

        private IVisitingReader MakeReader(Stream stream, IParsingVisitationListener listener, KvSerializerOptions options)
        {
            Require.NotNull(stream, nameof(stream));
            Require.NotNull(listener, nameof(listener));
            Require.NotNull(options, nameof(options));

            switch (_format)
            {
                case KvSerializationFormat.KeyValues1Text:
                    return new Kv1TextReader(new StreamReader(stream), listener, options);
                case KvSerializationFormat.KeyValues1Binary:
                    return new Kv1BinaryReader(stream, listener, options);
                default:
                    throw new ArgumentOutOfRangeException(nameof(_format), _format, "Invalid serialization format.");
            }
        }

        private IVisitationListener MakeSerializer(Stream stream, KvSerializerOptions options)
        {
            Require.NotNull(stream, nameof(stream));
            Require.NotNull(options, nameof(options));

            switch (_format)
            {
                case KvSerializationFormat.KeyValues1Text:
                    return new Kv1TextSerializer(stream, options);
                case KvSerializationFormat.KeyValues1Binary:
                    return new Kv1BinarySerializer(stream, options);
                default:
                    throw new ArgumentOutOfRangeException(nameof(_format), _format, "Invalid serialization format.");
            }
        }
    }
}
