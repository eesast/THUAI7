using Preparation.Interface;
using System;

namespace Preparation.Utility.Value.SafeValue.LockedValue
{
    public class LockedDouble(double x) : LockedValue, IDouble, IConvertible
    {
        private double v = x;

        #region 实现IConvertible接口
        public TypeCode GetTypeCode()
        {
            return ReadNeed(() => v.GetTypeCode());
        }

        public bool ToBoolean(IFormatProvider? provider)
        {
            return ReadNeed(() => Convert.ToBoolean(v, provider));
        }

        public char ToChar(IFormatProvider? provider)
        {
            return ReadNeed(() => Convert.ToChar(v, provider));
        }

        public sbyte ToSByte(IFormatProvider? provider)
        {
            return ReadNeed(() => Convert.ToSByte(v, provider));
        }

        public byte ToByte(IFormatProvider? provider)
        {
            return ReadNeed(() => Convert.ToByte(v, provider));
        }

        public short ToInt16(IFormatProvider? provider)
        {
            return ReadNeed(() => Convert.ToInt16(v, provider));
        }

        public ushort ToUInt16(IFormatProvider? provider)
        {
            return ReadNeed(() => Convert.ToUInt16(v, provider));
        }

        public int ToInt32(IFormatProvider? provider)
        {
            return ReadNeed(() => Convert.ToInt32(v, provider));
        }

        public uint ToUInt32(IFormatProvider? provider)
        {
            return ReadNeed(() => Convert.ToUInt32(v, provider));
        }

        public long ToInt64(IFormatProvider? provider)
        {
            return ReadNeed(() => Convert.ToInt64(v, provider));
        }

        public ulong ToUInt64(IFormatProvider? provider)
        {
            return ReadNeed(() => Convert.ToUInt64(v, provider));
        }

        public float ToSingle(IFormatProvider? provider)
        {
            return ReadNeed(() => Convert.ToSingle(v, provider));
        }

        public double ToDouble(IFormatProvider? provider)
        {
            return ReadNeed(() => v);
        }

        public decimal ToDecimal(IFormatProvider? provider)
        {
            return ReadNeed(() => Convert.ToDecimal(v, provider));
        }

        public DateTime ToDateTime(IFormatProvider? provider)
        {
            return ReadNeed(() => Convert.ToDateTime(v, provider));
        }

        public string ToString(IFormatProvider? provider)
        {
            return ReadNeed(() => v.ToString(provider));
        }

        public object ToType(Type conversionType, IFormatProvider? provider)
        {
            return ReadNeed(() => Convert.ChangeType(v, conversionType, provider));
        }
        #endregion

        public override string ToString()
        {
            return ReadNeed(() => v.ToString());
        }
        public double Get()
        {
            return ReadNeed(() => v);
        }
        public double ToDouble() => Get();
        public static implicit operator double(LockedDouble adouble) => adouble.Get();
        public override bool Equals(object? obj)
        {
            return obj != null && (obj is IConvertible k) && ToDouble(null) == k.ToDouble(null);
        }
        public override int GetHashCode()
        {
            return ReadNeed(() => v.GetHashCode());
        }

        public void Set(double value)
        {
            WriteNeed(() => v = value);
        }
        public double AddRNow(double value)
        {
            return WriteNeed(() => v += value);
        }
        public double MulRNow(double value)
        {
            return WriteNeed(() => v *= value);
        }
    }
}
