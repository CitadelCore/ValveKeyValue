using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ValveKeyValue
{
    internal class KvCollectionValue : KvValue, IEnumerable<KvObject>
    {
        public KvCollectionValue()
        {
            _children = new List<KvObject>();
        }

        private readonly List<KvObject> _children;

        public override KvValueType ValueType => KvValueType.Collection;
        public override KvValue this[string key]
        {
            get
            {
                Require.NotNull(key, nameof(key));
                return Get(key)?.Value;
            }
        }

        public void Add(KvObject value)
        {
            Require.NotNull(value, nameof(value));
            _children.Add(value);
        }

        public void AddRange(IEnumerable<KvObject> values)
        {
            var enumerable = values.ToList();
            Require.NotNull(enumerable, nameof(values));
            _children.AddRange(enumerable);
        }

        public KvObject Get(string name)
        {
            Require.NotNull(name, nameof(name));
            return _children.FirstOrDefault(c => c.Name == name);
        }

        public void Set(string name, KvValue value)
        {
            Require.NotNull(name, nameof(name));
            Require.NotNull(value, nameof(value));

            _children.RemoveAll(kv => kv.Name == name);
            _children.Add(new KvObject(name, value));
        }

        #region IEnumerable<KVObject>

        public IEnumerator<KvObject> GetEnumerator() => _children.GetEnumerator();

        #endregion

        #region IConvertible

        public override TypeCode GetTypeCode()
        {
            throw new NotSupportedException();
        }

        public override bool ToBoolean(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        public override byte ToByte(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        public override char ToChar(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        public override DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        public override decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        public override double ToDouble(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        public override short ToInt16(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        public override int ToInt32(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        public override long ToInt64(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        public override sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        public override float ToSingle(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        public override string ToString(IFormatProvider provider)
             => ToString();

        public override object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        public override ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        public override uint ToUInt32(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        public override ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() => _children.GetEnumerator();

        #endregion

        public override string ToString() => "[Collection]";
    }
}
