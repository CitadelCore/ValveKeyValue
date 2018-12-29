using System.Collections.Generic;

namespace ValveKeyValue.Deserialization
{
    internal class Kv1TextReaderStateMachine
    {
        public Kv1TextReaderStateMachine()
        {
            _states = new Stack<KvPartialState<Kv1TextReaderState>>();
            _includedPathsToMerge = new List<string>();
            _includedPathsToAppend = new List<string>();

            PushObject();
            Push(Kv1TextReaderState.InObjectBeforeKey);
        }

        private readonly Stack<KvPartialState<Kv1TextReaderState>> _states;
        private readonly IList<string> _includedPathsToMerge;
        private readonly IList<string> _includedPathsToAppend;

        public Kv1TextReaderState Current => CurrentObject.States.Peek();
        public bool IsInObject => _states.Count > 0;
        public bool IsAtStart => _states.Count == 1 && CurrentObject.States.Count == 1 && Current == Kv1TextReaderState.InObjectBeforeKey;
        public void PushObject() => _states.Push(new KvPartialState<Kv1TextReaderState>());
        public void Push(Kv1TextReaderState state) => CurrentObject.States.Push(state);

        public void PopObject(out bool discard)
        {
            var state = _states.Pop();
            discard = state.Discard;
        }

        public string CurrentName => CurrentObject.Key;
        public void Pop() => CurrentObject.States.Pop();
        public void SetName(string name) => CurrentObject.Key = name;
        public void SetValue(KvValue value) => CurrentObject.Value = value;
        public void AddItem(KvObject item) => CurrentObject.Items.Add(item);
        public void SetDiscardCurrent() => CurrentObject.Discard = true;
        public IEnumerable<string> ItemsForMerging => _includedPathsToMerge;
        public void AddItemForMerging(string item) => _includedPathsToMerge.Add(item);
        public IEnumerable<string> ItemsForAppending => _includedPathsToAppend;
        public void AddItemForAppending(string item) => _includedPathsToAppend.Add(item);
        private KvPartialState<Kv1TextReaderState> CurrentObject => _states.Peek();
    }
}
