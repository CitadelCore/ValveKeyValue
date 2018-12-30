using ValveKeyValue.Test.Test_Data;

namespace ValveKeyValue.Test
{
    internal sealed class StreamKvTextReader : IKvTextReader
    {
        KvObject IKvTextReader.Read(string resourceName, KvSerializerOptions options)
        {
            using (var stream = TestDataHelper.OpenResource(resourceName))
                return KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize(stream, options);
        }

        T IKvTextReader.Read<T>(string resourceName, KvSerializerOptions options)
        {
            using (var stream = TestDataHelper.OpenResource(resourceName))
                return KvSerializer.Create(KvSerializationFormat.KeyValues1Text).Deserialize<T>(stream, options);
        }
    }
}
