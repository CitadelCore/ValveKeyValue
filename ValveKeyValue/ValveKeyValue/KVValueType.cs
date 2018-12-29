namespace ValveKeyValue
{
    /// <summary>
    /// Represents the type of a given <see cref="KvValue"/>
    /// </summary>
    public enum KvValueType
    {
        /// <summary>
        /// This <see cref="KvValue"/> represents a collection of child <see cref="KvObject"/>s.
        /// </summary>
        Collection,

        /// <summary>
        /// This <see cref="KvValue"/> is represented by a <see cref="string"/>.
        /// </summary>
        String,

        /// <summary>
        /// This <see cref="KvValue"/> is represented by a <see cref="int"/>
        /// </summary>
        Int32,

        /// <summary>
        /// This <see cref="KvValue"/> is represented by a <see cref="ulong"/>
        /// </summary>
        UInt64,

        /// <summary>
        /// This <see cref="KvValue"/> is represented by a <see cref="float"/>
        /// </summary>
        FloatingPoint,

        /// <summary>
        /// This <see cref="KvValue"/> is represented by a <see cref="int"/>, but represents a pointer.
        /// </summary>
        Pointer,

        /// <summary>
        /// This <see cref="KvValue"/> is represented by a <see cref="long"/>.
        /// </summary>
        Int64
    }
}
