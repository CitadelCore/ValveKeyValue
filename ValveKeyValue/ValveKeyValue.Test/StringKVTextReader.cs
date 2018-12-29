namespace ValveKeyValue.Test
{
    sealed class StringKVTextReader : IKVTextReader
    {
        public StringKVTextReader()
        {
            serializer = KvSerializer.Create(KvSerializationFormat.KeyValues1Text);
        }

        readonly KvSerializer serializer;

        KvObject IKVTextReader.Read(string resourceName, KvSerializerOptions options)
        {
            var text = TestDataHelper.ReadTextResource(resourceName);
            return serializer.Deserialize(text, options);
        }

        T IKVTextReader.Read<T>(string resourceName, KvSerializerOptions options)
        {
            var text = TestDataHelper.ReadTextResource(resourceName);
            return serializer.Deserialize<T>(text, options);
        }
    }
}
