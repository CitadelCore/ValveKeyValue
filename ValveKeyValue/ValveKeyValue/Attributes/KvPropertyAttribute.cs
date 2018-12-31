using System;

namespace ValveKeyValue.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// This attribute is used to tell the deserializer to map a given property to a particular
    /// node in the KeyValue object tree, by name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class KvPropertyAttribute : Attribute
    {
        public KvPropertyAttribute() { }
        public KvPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        /// <summary>
        /// Gets the name of the property as it appears in KeyValues serialized data.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Gets or sets the array deserialisation type of this property.
        /// </summary>
        public KvCollectionType CollectionType { get; set; }

        /// <summary>
        /// Gets or sets the separator to use when the collection type is <see cref="KvCollectionType.CharSeparated"/>.
        /// </summary>
        public char CollectionTypeSeparator { get; set; } = ',';
    }
}
