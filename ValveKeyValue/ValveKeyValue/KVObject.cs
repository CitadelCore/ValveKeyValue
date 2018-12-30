﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ValveKeyValue
{
    /// <inheritdoc />
    /// <summary>
    /// Represents a dynamic KeyValue object.
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDescription) + "}")]
    public partial class KvObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KvObject"/> class.
        /// </summary>
        /// <param name="name">Name of this object.</param>
        /// <param name="value">Value of this object.</param>
        public KvObject(string name, KvValue value)
        {
            Require.NotNull(name, nameof(name));
            Require.NotNull(value, nameof(value));

            Name = name;
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KvObject"/> class.
        /// </summary>
        /// <param name="name">Name of this object.</param>
        /// <param name="items">Child items of this object.</param>
        public KvObject(string name, IEnumerable<KvObject> items)
        {
            Require.NotNull(name, nameof(name));
            Require.NotNull(items, nameof(items));

            Name = name;
            var value = new KvCollectionValue();
            value.AddRange(items);

            Value = value;
        }

        /// <summary>
        /// Gets the name of this object.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the value of this object.
        /// </summary>
        public KvValue Value { get; }

        /// <summary>
        /// Indexer to find a child item by name.
        /// </summary>
        /// <param name="key">Key of the child object to find</param>
        /// <returns>A <see cref="KvObject"/> if the child item exists, otherwise <c>null</c>.</returns>
        public KvValue this[string key]
        {
            get
            {
                Require.NotNull(key, nameof(key));

                var children = GetCollectionValue();
                return children[key];
            }

            set
            {
                Require.NotNull(key, nameof(key));

                var children = GetCollectionValue();
                children.Set(key, value);
            }
        }

        /// <summary>
        /// Adds a <see cref="KvObject" /> as a child of the current object.
        /// </summary>
        /// <param name="value">The child to add.</param>
        public void Add(KvObject value)
        {
            Require.NotNull(value, nameof(value));
            GetCollectionValue().Add(value);
        }

        /// <summary>
        /// Gets the children of this <see cref="KvObject"/>.
        /// </summary>
        public IEnumerable<KvObject> Children => (Value as KvCollectionValue) ?? Enumerable.Empty<KvObject>();

        private KvCollectionValue GetCollectionValue()
        {
            if (!(Value is KvCollectionValue collection))
                throw new InvalidOperationException($"This operation on a {nameof(KvObject)} can only be used when the value has children.");

            return collection;
        }

        private string DebuggerDescription
        {
            get
            {
                var description = new StringBuilder();
                description.Append(Name);
                description.Append(": ");
                description.Append(Value.ToString(CultureInfo.InvariantCulture));

                return description.ToString();
            }
        }
    }
}
