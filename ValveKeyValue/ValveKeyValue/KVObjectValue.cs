using System;
using System.Diagnostics;
using System.Globalization;

namespace ValveKeyValue
{
    [DebuggerDisplay("{" + nameof(_value) + "}")]
    internal class KvObjectValue<TObject> : KvValue
        where TObject : IConvertible
    {
        public KvObjectValue(TObject value, KvValueType valueType)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            _value = value;
            ValueType = valueType;
        }

        private readonly TObject _value;

        public override KvValueType ValueType { get; }

        public override TypeCode GetTypeCode()
        {
            switch (ValueType)
            {
                case KvValueType.Collection:
                    return TypeCode.Object;
                case KvValueType.FloatingPoint:
                    return TypeCode.Single;
                case KvValueType.Int32:
                case KvValueType.Pointer:
                    return TypeCode.Int32;
                case KvValueType.String:
                    return TypeCode.String;
                case KvValueType.UInt64:
                    return TypeCode.UInt64;
                default:
                    throw new NotImplementedException(string.Format(CultureInfo.InvariantCulture, "No known TypeCode for '{0}'", ValueType));
            }
        }

        public override bool ToBoolean(IFormatProvider provider) => ToInt32(provider) == 1;

        public override byte ToByte(IFormatProvider provider) => (byte)Convert.ChangeType(_value, typeof(byte), provider);

        public override char ToChar(IFormatProvider provider) => (char)Convert.ChangeType(_value, typeof(char), provider);

        public override DateTime ToDateTime(IFormatProvider provider) => throw new InvalidCastException();

        public override decimal ToDecimal(IFormatProvider provider) => (decimal)Convert.ChangeType(_value, typeof(decimal), provider);

        public override double ToDouble(IFormatProvider provider) => (double)Convert.ChangeType(_value, typeof(double), provider);

        public override short ToInt16(IFormatProvider provider) => (short)Convert.ChangeType(_value, typeof(short), provider);

        public override int ToInt32(IFormatProvider provider) => (int)Convert.ChangeType(_value, typeof(int), provider);

        public override long ToInt64(IFormatProvider provider) => (long)Convert.ChangeType(_value, typeof(long), provider);

        public override sbyte ToSByte(IFormatProvider provider) => (sbyte)Convert.ChangeType(_value, typeof(sbyte), provider);

        public override float ToSingle(IFormatProvider provider) => (float)Convert.ChangeType(_value, typeof(float), provider);

        public override string ToString(IFormatProvider provider) => (string)Convert.ChangeType(_value, typeof(string), provider);

        public override object ToType(Type conversionType, IFormatProvider provider) => Convert.ChangeType(_value, conversionType, provider);

        public override ushort ToUInt16(IFormatProvider provider) => (ushort)Convert.ChangeType(_value, typeof(ushort), provider);

        public override uint ToUInt32(IFormatProvider provider) => (uint)Convert.ChangeType(_value, typeof(uint), provider);

        public override ulong ToUInt64(IFormatProvider provider) => (ulong)Convert.ChangeType(_value, typeof(ulong), provider);

        public override string ToString() => ToString(null);
    }
}
