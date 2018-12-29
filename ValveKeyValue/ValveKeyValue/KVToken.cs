namespace ValveKeyValue
{
    internal class KvToken
    {
        public KvToken(KvTokenType type, string value = null)
        {
            TokenType = type;
            Value = value;
        }

        public KvTokenType TokenType { get; }
        public string Value { get; }
    }
}
