namespace ValveKeyValue.Test
{
    interface IKVTextReader
    {
        KvObject Read(string resourceName, KvSerializerOptions options = null);

        T Read<T>(string resourceName, KvSerializerOptions options = null);
    }
}
