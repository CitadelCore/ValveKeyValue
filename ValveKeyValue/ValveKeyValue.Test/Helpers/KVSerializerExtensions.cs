using System.IO;

namespace ValveKeyValue.Test
{
    static class KVSerializerExtensions
    {
        public static KvObject Deserialize(this KvSerializer serializer, byte[] data, KvSerializerOptions options = null)
        {
            using (var ms = new MemoryStream(data))
            {
                return serializer.Deserialize(ms, options);
            }
        }

        public static TObject Deserialize<TObject>(this KvSerializer serializer, byte[] data, KvSerializerOptions options = null)
        {
            using (var ms = new MemoryStream(data))
            {
                return serializer.Deserialize<TObject>(ms, options);
            }
        }

        public static KvObject Deserialize(this KvSerializer serializer, string text, KvSerializerOptions options = null)
        {
            using (var ms = new MemoryStream())
            using (var writer = new StreamWriter(ms))
            {
                writer.Write(text);
                writer.Flush();

                ms.Seek(0, SeekOrigin.Begin);

                return serializer.Deserialize(ms, options);
            }
        }

        public static TObject Deserialize<TObject>(this KvSerializer serializer, string text, KvSerializerOptions options = null)
        {
            using (var ms = new MemoryStream())
            using (var writer = new StreamWriter(ms))
            {
                writer.Write(text);
                writer.Flush();

                ms.Seek(0, SeekOrigin.Begin);

                return serializer.Deserialize<TObject>(ms, options);
            }
        }
    }
}
