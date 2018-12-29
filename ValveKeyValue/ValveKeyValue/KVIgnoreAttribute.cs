using System;

namespace ValveKeyValue
{
    /// <inheritdoc />
    /// <summary>
    /// This attribute is used to tell the deserializer to ignore a given property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class KvIgnoreAttribute : Attribute
    {
    }
}
