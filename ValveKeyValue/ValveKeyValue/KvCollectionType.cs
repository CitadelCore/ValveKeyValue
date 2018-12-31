using System;
using System.Collections.Generic;
using System.Text;

namespace ValveKeyValue
{
    public enum KvCollectionType
    {
        /// <summary>
        /// Comma separated values.
        /// </summary>
        ValueList,
        CharSeparated,
        Default = ValueList
    }
}
