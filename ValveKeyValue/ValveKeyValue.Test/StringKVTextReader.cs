using ValveKeyValue.Test.Helpers;
using ValveKeyValue.Test.Test_Data;

namespace ValveKeyValue.Test
{
    internal sealed class StringKvTextReader : IKvTextReader
    {
        public StringKvTextReader()
        {
            _serializer = KvSerializer.Create(KvSerializationFormat.KeyValues1Text);
        }

        private readonly KvSerializer _serializer;

        KvObject IKvTextReader.Read(string resourceName, KvSerializerOptions options)
        {
            var text = TestDataHelper.ReadTextResource(resourceName);
            return _serializer.Deserialize(text, options);
        }

        T IKvTextReader.Read<T>(string resourceName, KvSerializerOptions options)
        {
            var text = TestDataHelper.ReadTextResource(resourceName);
            return _serializer.Deserialize<T>(text, options);
        }
    }
}
