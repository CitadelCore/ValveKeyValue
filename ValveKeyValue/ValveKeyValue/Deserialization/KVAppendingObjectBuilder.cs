namespace ValveKeyValue.Deserialization
{
    internal sealed class KvAppendingObjectBuilder : KvObjectBuilder
    {
        public KvAppendingObjectBuilder(KvObjectBuilder originalBuilder)
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

            foreach (var item in stateEntry.Items)
            {
                originalStateEntry.Items.Add(item);
            }
        }
    }
}
