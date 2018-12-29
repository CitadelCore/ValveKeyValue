using System;
using System.Collections.Generic;
using ValveKeyValue.Abstraction;

namespace ValveKeyValue.Deserialization
{
    internal class KvObjectBuilder : IParsingVisitationListener
    {
        private readonly IList<KvObjectBuilder> _associatedBuilders = new List<KvObjectBuilder>();

        public KvObject GetObject()
        {
            if (StateStack.Count != 1)
            {
                throw new KeyValueException("Builder is not in a fully completed state.");
            }

            foreach (var associatedBuilder in _associatedBuilders)
            {
                associatedBuilder.FinalizeState();
            }

            var state = StateStack.Peek();
            return MakeObject(state);
        }

        public void OnKeyValuePair(string name, KvValue value)
        {
            if (StateStack.Count > 0)
            {
                var state = StateStack.Peek();
                state.Items.Add(new KvObject(name, value));
            }
            else
            {
                var state = new KvPartialState {Key = name, Value = value};
                StateStack.Push(state);
            }
        }

        public void OnObjectEnd()
        {
            if (StateStack.Count <= 1)
                return;

            var state = StateStack.Pop();
            var completedObject = MakeObject(state);

            var parentState = StateStack.Peek();
            parentState.Items.Add(completedObject);
        }

        public void DiscardCurrentObject()
        {
            var state = StateStack.Peek();
            if (state.Items?.Count > 0)
            {
                state.Items.RemoveAt(state.Items.Count - 1);
            }
            else
            {
                StateStack.Pop();
            }
        }

        public void OnObjectStart(string name)
        {
            var state = new KvPartialState {Key = name};
            StateStack.Push(state);
        }

        public IParsingVisitationListener GetMergeListener()
        {
            var builder = new KvMergingObjectBuilder(this);
            _associatedBuilders.Add(builder);
            return builder;
        }

        public IParsingVisitationListener GetAppendListener()
        {
            var builder = new KvAppendingObjectBuilder(this);
            _associatedBuilders.Add(builder);
            return builder;
        }

        public void Dispose() { }

        internal Stack<KvPartialState> StateStack { get; } = new Stack<KvPartialState>();

        protected virtual void FinalizeState() { }

        private static KvObject MakeObject(KvPartialState state)
        {
            if (state.Discard)
                return null;
            return state.Value != null ? new KvObject(state.Key, state.Value) : new KvObject(state.Key, state.Items);
        }
    }
}
