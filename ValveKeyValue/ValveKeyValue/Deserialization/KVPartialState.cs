using System.Collections.Generic;

namespace ValveKeyValue.Deserialization
{
    internal class KvPartialState
    {
        public string Key { get; set; }
        public KvValue Value { get; set; }
        public IList<KvObject> Items { get; } = new List<KvObject>();
        public bool Discard { get; set; }
    }
}
