namespace ValveKeyValue.Deserialization
{
    enum Kv1TextReaderState
    {
       InObjectBeforeKey,
       InObjectBetweenKeyAndValue,
       InObjectAfterValue
    }
}
