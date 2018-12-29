namespace ValveKeyValue.Test
{
    sealed class StreamKVTextReader : IKVTextReader
    {
        KvObject IKVTextReader.Read(string resourceName, KvSerializerOptions options)
        {
            using (var stream = TestDataHelper.OpenResource(resourceName))
            {
                return KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize(stream, options);
            }
        }

        T IKVTextReader.Read<T>(string resourceName, KvSerializerOptions options)
        {
            using (var stream = TestDataHelper.OpenResource(resourceName))
            {
                return KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize<T>(stream, options);
            }
        }
    }
}
