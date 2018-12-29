using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ValveKeyValue
{
    /// <inheritdoc />
    /// <summary>
    /// Represents a dynamic KeyValue object.
    /// </summary>
    public partial class KvObject : IEnumerable<KvObject>
    {
        /// <inheritdoc/>
        public IEnumerator<KvObject> GetEnumerator()
            => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Children.GetEnumerator();
    }
}
