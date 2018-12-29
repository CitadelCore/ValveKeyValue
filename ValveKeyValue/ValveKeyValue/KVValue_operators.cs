using System;

namespace ValveKeyValue
{
    /// <inheritdoc />
    /// <summary>
    /// Container type for value of a KeyValues object.
    /// </summary>
    public abstract partial class KvValue
    {
        /// <summary>
        /// Implicit cast operator for <see cref="string"/> to KVValue.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to cast.</param>
        public static implicit operator KvValue(string value)
        {
            Require.NotNull(value, nameof(value));
            return new KvObjectValue<string>(value, KvValueType.String);
        }

        /// <summary>
        /// Implicit cast operator for <see cref="int"/> to KVValue.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to cast.</param>
        public static implicit operator KvValue(int value)
        {
            return new KvObjectValue<int>(value, KvValueType.Int32);
        }

        /// <summary>
        /// Implicit cast operator for <see cref="IntPtr"/> to KVValue.
        /// </summary>
        /// <param name="value">The <see cref="IntPtr"/> to cast.</param>
        public static implicit operator KvValue(IntPtr value)
        {
            return new KvObjectValue<int>(value.ToInt32(), KvValueType.Pointer);
        }

        /// <summary>
        /// Implicit cast operator for <see cref="ulong"/> to KVValue.
        /// </summary>
        /// <param name="value">The <see cref="ulong"/> to cast.</param>
        public static implicit operator KvValue(ulong value)
        {
            return new KvObjectValue<ulong>(value, KvValueType.UInt64);
        }

        /// <summary>
        /// Implicit cast operator for <see cref="float"/> to KVValue.
        /// </summary>
        /// <param name="value">The <see cref="float"/> to cast.</param>
        public static implicit operator KvValue(float value)
        {
            return new KvObjectValue<float>(value, KvValueType.FloatingPoint);
        }

        /// <summary>
        /// Implicit cast operator for <see cref="long"/> to KVValue.
        /// </summary>
        /// <param name="value">The <see cref="long"/> to cast.</param>
        public static implicit operator KvValue(long value)
        {
            return new KvObjectValue<long>(value, KvValueType.Int64);
        }

        /// <summary>
        /// Converts a <see cref="KvValue"/> to a <see cref="string"/>.
        /// </summary>
        /// <param name="value">The <see cref="KvValue"/> to convert.</param>
        public static explicit operator string(KvValue value)
        {
            return value?.ToString(null);
        }

        /// <summary>
        /// Converts a <see cref="KvValue"/> to a <see cref="bool"/>.
        /// </summary>
        /// <param name="value">The <see cref="KvValue"/> to convert.</param>
        public static explicit operator bool(KvValue value)
        {
            return value.ToBoolean(null);
        }

        /// <summary>
        /// Converts a <see cref="KvValue"/> to a <see cref="byte"/>.
        /// </summary>
        /// <param name="value">The <see cref="KvValue"/> to convert.</param>
        public static explicit operator byte(KvValue value)
        {
            return value.ToByte(null);
        }

        /// <summary>
        /// Converts a <see cref="KvValue"/> to a <see cref="char"/>.
        /// </summary>
        /// <param name="value">The <see cref="KvValue"/> to convert.</param>
        public static explicit operator char(KvValue value)
        {
            return value.ToChar(null);
        }

        /// <summary>
        /// Converts a <see cref="KvValue"/> to a <see cref="decimal"/>.
        /// </summary>
        /// <param name="value">The <see cref="KvValue"/> to convert.</param>
        public static explicit operator decimal(KvValue value)
        {
            return value.ToDecimal(null);
        }

        /// <summary>
        /// Converts a <see cref="KvValue"/> to a <see cref="double"/>.
        /// </summary>
        /// <param name="value">The <see cref="KvValue"/> to convert.</param>
        public static explicit operator double(KvValue value)
        {
            return value.ToDouble(null);
        }

        /// <summary>
        /// Converts a <see cref="KvValue"/> to a <see cref="float"/>.
        /// </summary>
        /// <param name="value">The <see cref="KvValue"/> to convert.</param>
        public static explicit operator float(KvValue value)
        {
            return value.ToSingle(null);
        }

        /// <summary>
        /// Converts a <see cref="KvValue"/> to a <see cref="bool"/>.
        /// </summary>
        /// <param name="value">The <see cref="KvValue"/> to convert.</param>
        public static explicit operator int(KvValue value)
        {
            return value.ToInt32(null);
        }

        /// <summary>
        /// Converts a <see cref="KvValue"/> to a <see cref="long"/>.
        /// </summary>
        /// <param name="value">The <see cref="KvValue"/> to convert.</param>
        public static explicit operator long(KvValue value)
        {
            return value.ToInt64(null);
        }

        /// <summary>
        /// Converts a <see cref="KvValue"/> to a <see cref="sbyte"/>.
        /// </summary>
        /// <param name="value">The <see cref="KvValue"/> to convert.</param>
        public static explicit operator sbyte(KvValue value)
        {
            return value.ToSByte(null);
        }

        /// <summary>
        /// Converts a <see cref="KvValue"/> to a <see cref="short"/>.
        /// </summary>
        /// <param name="value">The <see cref="KvValue"/> to convert.</param>
        public static explicit operator short(KvValue value)
        {
            return value.ToInt16(null);
        }

        /// <summary>
        /// Converts a <see cref="KvValue"/> to a <see cref="uint"/>.
        /// </summary>
        /// <param name="value">The <see cref="KvValue"/> to convert.</param>
        public static explicit operator uint(KvValue value)
        {
            return value.ToUInt32(null);
        }

        /// <summary>
        /// Converts a <see cref="KvValue"/> to a <see cref="ulong"/>.
        /// </summary>
        /// <param name="value">The <see cref="KvValue"/> to convert.</param>
        public static explicit operator ulong(KvValue value)
        {
            return value.ToUInt64(null);
        }

        /// <summary>
        /// Converts a <see cref="KvValue"/> to a <see cref="ushort"/>.
        /// </summary>
        /// <param name="value">The <see cref="KvValue"/> to convert.</param>
        public static explicit operator ushort(KvValue value)
        {
            return value.ToUInt16(null);
        }
    }
}
