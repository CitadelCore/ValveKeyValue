namespace ValveKeyValue.Test
{
    internal interface IKvTextReader
    {
        KvObject Read(string resourceName, KvSerializerOptions options = null);
        T Read<T>(string resourceName, KvSerializerOptions options = null);
    }
}
