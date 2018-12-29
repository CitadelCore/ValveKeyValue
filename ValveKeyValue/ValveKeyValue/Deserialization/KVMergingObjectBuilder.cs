using System.Linq;

namespace ValveKeyValue.Deserialization
{
    internal sealed class KvMergingObjectBuilder : KvObjectBuilder
    {
        public KvMergingObjectBuilder(KvObjectBuilder originalBuilder)
        {
            Require.NotNull(originalBuilder, nameof(originalBuilder));
            _originalBuilder = originalBuilder;
        }

        private readonly KvObjectBuilder _originalBuilder;

        protected override void FinalizeState()
        {
            base.FinalizeState();

            var stateEntry = StateStack.Peek();
            var originalStateEntry = _originalBuilder.StateStack.Peek();

            Merge(stateEntry, originalStateEntry);
        }

        private static void Merge(KvPartialState from, KvPartialState into)
        {
            foreach (var item in from.Items)
            {
                var matchingItem = into.Items.FirstOrDefault(i => i.Name == item.Name);
                if (matchingItem == null)
                {
                    into.Items.Add(item);
                }
                else
                {
                    Merge(item, matchingItem);
                }
            }
        }

        private static void Merge(KvObject from, KvObject into)
        {
            foreach (var child in from)
            {
                var matchingChild = into.Children.FirstOrDefault(c => c.Name == child.Name);
                if (matchingChild == null && into.Value.ValueType == KvValueType.Collection)
                {
                    into.Add(child);
                }
                else
                {
                    Merge(child, matchingChild);
                }
            }
        }
    }
}
