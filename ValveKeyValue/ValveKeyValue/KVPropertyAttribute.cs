using System;

namespace ValveKeyValue
{
    /// <inheritdoc />
    /// <summary>
    /// This attribute is used to tell the deserializer to map a given property to a particular
    /// node in the KeyValue object tree, by name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class KvPropertyAttribute : Attribute
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ValveKeyValue.KvPropertyAttribute" /> class.
        /// </summary>
        /// <param name="propertyName">The name of the property as it appears in KeyValues serialized data.</param>
        public KvPropertyAttribute(string propertyName)
        {
            Require.NotNull(propertyName, nameof(propertyName));
            PropertyName = propertyName;
        }

        /// <summary>
        /// Gets the name of the property as it appears in KeyValues serialized data.
        /// </summary>
        public string PropertyName { get; }
    }
}
