namespace ValveKeyValue.Deserialization
{
    internal enum Kv1TextReaderState
    {
       InObjectBeforeKey,
       InObjectBetweenKeyAndValue,
       InObjectAfterValue
    }
}
