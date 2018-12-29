using System.Collections.Generic;

namespace ValveKeyValue.Deserialization
{
    internal class KvPartialState<TState> : KvPartialState
    {
        public Stack<TState> States { get; } = new Stack<TState>();
    }
}
