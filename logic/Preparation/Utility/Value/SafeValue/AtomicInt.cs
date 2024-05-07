using Preparation.Interface;
using System;
using System.Threading;

namespace Preparation.Utility.Value.SafeValue.Atomic
{
    //其对应属性不应当有set访问器，避免不安全的=赋值

    public class AtomicInt(int x) : Atomic, IIntAddable, IConvertible
    {
        protected int v = x;

        #region 实现IConvertible接口

        public TypeCode GetTypeCode()
        {
            return TypeCode.Int32;
        }

        public bool ToBoolean(IFormatProvider? provider)
        {
            return Convert.ToBoolean(Get(), provider);
        }

        public char ToChar(IFormatProvider? provider)
        {
            return Convert.ToChar(Get(), provider);
        }

        public sbyte ToSByte(IFormatProvider? provider)
        {
            return Convert.ToSByte(Get(), provider);
        }

        public byte ToByte(IFormatProvider? provider)
        {
            return Convert.ToByte(Get(), provider);
        }

        public short ToInt16(IFormatProvider? provider)
        {
            return Convert.ToInt16(Get(), provider);
        }

        public ushort ToUInt16(IFormatProvider? provider)
        {
            return Convert.ToUInt16(Get(), provider);
        }

        public int ToInt32(IFormatProvider? provider)
        {
            return Convert.ToInt32(Get(), provider);
        }

        public uint ToUInt32(IFormatProvider? provider)
        {
            return Convert.ToUInt32(Get(), provider);
        }

        public long ToInt64(IFormatProvider? provider)
        {
            return Convert.ToInt64(Get(), provider);
        }

        public ulong ToUInt64(IFormatProvider? provider)
        {
            return Convert.ToUInt64(Get(), provider);
        }

        public float ToSingle(IFormatProvider? provider)
        {
            return Convert.ToSingle(Get(), provider);
        }

        public double ToDouble(IFormatProvider? provider)
        {
            return Get();
        }

        public decimal ToDecimal(IFormatProvider? provider)
        {
            return Convert.ToDecimal(Get(), provider);
        }

        public DateTime ToDateTime(IFormatProvider? provider)
        {
            return Convert.ToDateTime(Get(), provider);
        }

        public string ToString(IFormatProvider? provider)
        {
            return Get().ToString(provider);
        }

        public object ToType(Type conversionType, IFormatProvider? provider)
        {
            return Convert.ChangeType(Get(), conversionType, provider);
        }
        #endregion

        public override string ToString() => Interlocked.CompareExchange(ref v, -1, -1).ToString();
        public int Get() => Interlocked.CompareExchange(ref v, -1, -1);
        public static implicit operator int(AtomicInt aint) => Interlocked.CompareExchange(ref aint.v, -1, -1);
        public override bool Equals(object? obj)
        {
            return obj != null && (obj is IConvertible k) && ToDouble(null) == k.ToDouble(null);
        }
        public override int GetHashCode()
        {
            return Get().GetHashCode();
        }

        public virtual void Set(int value) => Interlocked.Exchange(ref v, value);
        /// <returns>返回操作前的值</returns>
        public virtual int SetROri(int value) => Interlocked.Exchange(ref v, value);

        public virtual void Add(int x) => Interlocked.Add(ref v, x);
        public virtual int AddRNow(int x) => Interlocked.Add(ref v, x);
        /// <summary>
        /// 注意：确保参数为非负数
        /// </summary>
        public virtual void AddPositive(int x) => Interlocked.Add(ref v, x);
        /// <summary>
        /// 注意：确保参数为非负数
        /// </summary>
        public virtual int AddPositiveRNow(int x) => Interlocked.Add(ref v, x);

        public virtual void Sub(int x) => Interlocked.Add(ref v, -x);
        public virtual int SubRNow(int x) => Interlocked.Add(ref v, -x);
        /// <summary>
        /// 注意：确保参数为非负数
        /// </summary>
        public virtual void SubPositive(int x) => Interlocked.Add(ref v, -x);

        public virtual int Inc() => Interlocked.Increment(ref v);
        public virtual int Dec() => Interlocked.Decrement(ref v);
        /// <returns>返回操作前的值</returns>
        public virtual int CompareExROri(int newV, int compareTo) => Interlocked.CompareExchange(ref v, newV, compareTo);
    }

    /// <summary>
    /// 参数要求倍率speed（默认1）
    /// 可以设定IIntAddable类的Score，默认初始为0的AtomicInt
    /// 在发生正向的变化时，自动给Score加上正向变化的差乘以speed（取整）。
    /// 注意：AtomicIntOnlyAddScore本身为AtomicInt，提供的Score可能构成环而死锁。
    /// </summary>
    public class AtomicIntOnlyAddScore(int x, double speed = 1.0) : AtomicInt(x)
    {
        private IIntAddable score = new AtomicInt(0);
        /// <summary>
        /// 注意：Score的set操作（即=）的真正意义是改变其引用，单纯改变值不应当使用该操作
        /// </summary>
        public IIntAddable Score
        {
            get
            {
                return Interlocked.CompareExchange(ref score!, null, null);
            }
            set
            {
                Interlocked.Exchange(ref score, value);
            }
        }
        private IDouble speed = new AtomicDouble(speed);
        /// <summary>
        /// 注意：set操作（即=）的真正意义是改变其引用，单纯改变值不应当使用该操作
        /// </summary>
        public IDouble Speed
        {
            get
            {
                return Interlocked.CompareExchange(ref speed!, null, null);
            }
            set
            {
                Interlocked.Exchange(ref speed, value);
            }
        }

        /// <returns>返回操作前的值</returns>
        public override int SetROri(int value)
        {
            int previousV = Interlocked.Exchange(ref v, value);
            if (value - previousV > 0)
                Score.Add(Convert.ToInt32((value - previousV) * speed.ToDouble()));
            return previousV;
        }
        /// <returns>返回操作前的值</returns>
        public int SetROriNotAddScore(int value)
        {
            return Interlocked.Exchange(ref v, value);
        }
        public override void Add(int x)
        {
            if (x > 0) Score.Add(Convert.ToInt32(x * speed.ToDouble()));
            Interlocked.Add(ref v, x);
        }
        public override int AddRNow(int x)
        {
            if (x > 0) Score.Add(Convert.ToInt32(x * speed.ToDouble()));
            return Interlocked.Add(ref v, x);
        }
        public void AddNotAddScore(int x) => Interlocked.Add(ref v, x);
        /// <summary>
        /// 注意：确保参数为非负数
        /// </summary>
        public override void AddPositive(int x)
        {
            Score.Add(Convert.ToInt32(x * speed.ToDouble()));
            Interlocked.Add(ref v, x);
        }
        /// <summary>
        /// 注意：确保参数为非负数
        /// </summary>
        public override int AddPositiveRNow(int x)
        {
            Score.Add(Convert.ToInt32(x * speed.ToDouble()));
            return Interlocked.Add(ref v, x);
        }

        public override int SubRNow(int x)
        {
            if (x < 0) Score.Add(Convert.ToInt32(-x * speed.ToDouble()));
            return Interlocked.Add(ref v, -x);
        }
        public override void Sub(int x)
        {
            if (x < 0) Score.Add(Convert.ToInt32(-x * speed.ToDouble()));
            Interlocked.Add(ref v, -x);
        }
        public int SubRNowNotAddScore(int x)
        {
            return Interlocked.Add(ref v, -x);
        }
        /// <summary>
        /// 注意：确保参数为非负数
        /// </summary>
        public override void SubPositive(int x) => Interlocked.Add(ref v, -x);
        public override int Inc()
        {
            Score.Add(Convert.ToInt32(speed));
            return Interlocked.Increment(ref v);
        }
        public int IncNotAddScore()
        {
            return Interlocked.Increment(ref v);
        }
        /// <returns>返回操作前的值</returns>
        public override int CompareExROri(int newV, int compareTo)
        {
            int previousV = Interlocked.CompareExchange(ref v, newV, compareTo);
            if (newV - previousV > 0)
                Score.Add(Convert.ToInt32((newV - previousV) * speed.ToDouble()));
            return previousV;
        }
        /// <returns>返回操作前的值</returns>
        public int CompareExROriNotAddScore(int newV, int compareTo)
        {
            return Interlocked.CompareExchange(ref v, newV, compareTo);
        }
    }

    /// <summary>
    /// 参数要求倍率speed（默认1）
    /// 可（在构造时）以设定ISafeConvertible<int>类的Score，默认初始为0的AtomicInt
    /// 在发生变化时，自动给Score加上变化的差乘以speed取整。
    /// 注意：AtomicIntChangeAffectScore本身为AtomicInt，提供的Score可能构成环而死锁。
    /// </summary>
    public class AtomicIntChangeAffectScore(int x, double speed = 1.0) : AtomicInt(x)
    {
        public IIntAddable score = new AtomicInt(0);
        public IIntAddable Score
        {
            get
            {
                return Interlocked.CompareExchange(ref score!, null, null);
            }
            set
            {
                Interlocked.Exchange(ref score, value);
            }
        }
        public AtomicDouble speed = new(speed);

        /// <returns>返回操作前的值</returns>
        public override int SetROri(int value)
        {
            int previousV = Interlocked.Exchange(ref v, value);
            Score.Add(Convert.ToInt32((value - previousV) * speed));
            return previousV;
        }
        /// <returns>返回操作前的值</returns>
        public int SetROriNotAddScore(int value)
        {
            return Interlocked.Exchange(ref v, value);
        }

        public override void Add(int x)
        {
            Score.Add(Convert.ToInt32(x * speed));
            Interlocked.Add(ref v, x);
        }
        public override int AddRNow(int x)
        {
            Score.Add(Convert.ToInt32(x * speed));
            return Interlocked.Add(ref v, x);
        }
        public void AddNotAddScore(int x) => Interlocked.Add(ref v, x);
        /// <summary>
        /// 注意：确保参数为非负数
        /// </summary>
        public override void AddPositive(int x)
        {
            Score.Add(Convert.ToInt32(x * speed));
            Interlocked.Add(ref v, x);
        }
        /// <summary>
        /// 注意：确保参数为非负数
        /// </summary>
        public override int AddPositiveRNow(int x)
        {
            Score.Add(Convert.ToInt32(x * speed));
            return Interlocked.Add(ref v, x);
        }

        public override void Sub(int x)
        {
            Score.Add(Convert.ToInt32(-x * speed));
            Interlocked.Add(ref v, -x);
        }
        public override int SubRNow(int x)
        {
            Score.Add(Convert.ToInt32(-x * speed));
            return Interlocked.Add(ref v, -x);
        }
        public void SubNotAddScore(int x) => Interlocked.Add(ref v, -x);
        /// <summary>
        /// 注意：确保参数为非负数
        /// </summary>
        public override void SubPositive(int x)
        {
            Score.Add(Convert.ToInt32(-x * speed));
            Interlocked.Add(ref v, -x);
        }
        public override int Inc()
        {
            Score.Add(Convert.ToInt32(speed));
            return Interlocked.Increment(ref v);
        }
        public int IncNotAddScore()
        {
            return Interlocked.Increment(ref v);
        }
        public override int Dec()
        {
            Score.Add(Convert.ToInt32(-speed));
            return Interlocked.Decrement(ref v);
        }
        public int DecNotAddScore()
        {
            return Interlocked.Decrement(ref v);
        }
        /// <returns>返回操作前的值</returns>
        public override int CompareExROri(int newV, int compareTo)
        {
            int previousV = Interlocked.CompareExchange(ref v, newV, compareTo);
            Score.Add(Convert.ToInt32((newV - previousV) * speed));
            return previousV;
        }
        /// <returns>返回操作前的值</returns>
        public int CompareExROriNotAddScore(int newV, int compareTo)
        {
            return Interlocked.CompareExchange(ref v, newV, compareTo);
        }
    }

    public class AtomicLong(long x) : Atomic, IIntAddable, IAddable<long>, IConvertible
    {
        protected long v = x;

        #region 实现IConvertible接口

        public TypeCode GetTypeCode()
        {
            return TypeCode.Int64;
        }

        public bool ToBoolean(IFormatProvider? provider)
        {
            return Convert.ToBoolean(Get(), provider);
        }

        public char ToChar(IFormatProvider? provider)
        {
            return Convert.ToChar(Get(), provider);
        }

        public sbyte ToSByte(IFormatProvider? provider)
        {
            return Convert.ToSByte(Get(), provider);
        }

        public byte ToByte(IFormatProvider? provider)
        {
            return Convert.ToByte(Get(), provider);
        }

        public short ToInt16(IFormatProvider? provider)
        {
            return Convert.ToInt16(Get(), provider);
        }

        public ushort ToUInt16(IFormatProvider? provider)
        {
            return Convert.ToUInt16(Get(), provider);
        }

        public int ToInt32(IFormatProvider? provider)
        {
            return Convert.ToInt32(Get(), provider);
        }

        public uint ToUInt32(IFormatProvider? provider)
        {
            return Convert.ToUInt32(Get(), provider);
        }

        public long ToInt64(IFormatProvider? provider)
        {
            return Convert.ToInt64(Get(), provider);
        }

        public ulong ToUInt64(IFormatProvider? provider)
        {
            return Convert.ToUInt64(Get(), provider);
        }

        public float ToSingle(IFormatProvider? provider)
        {
            return Convert.ToSingle(Get(), provider);
        }

        public double ToDouble(IFormatProvider? provider)
        {
            return Get();
        }

        public decimal ToDecimal(IFormatProvider? provider)
        {
            return Convert.ToDecimal(Get(), provider);
        }

        public DateTime ToDateTime(IFormatProvider? provider)
        {
            return Convert.ToDateTime(Get(), provider);
        }

        public string ToString(IFormatProvider? provider)
        {
            return Get().ToString(provider);
        }

        public object ToType(Type conversionType, IFormatProvider? provider)
        {
            return Convert.ChangeType(Get(), conversionType, provider);
        }
        #endregion

        public override string ToString() => Interlocked.CompareExchange(ref v, -1, -1).ToString();
        public long Get() => Interlocked.CompareExchange(ref v, -1, -1);
        public static implicit operator long(AtomicLong along) => Interlocked.CompareExchange(ref along.v, -1, -1);
        public override bool Equals(object? obj)
        {
            return obj != null && (obj is IConvertible k) && ToDouble(null) == k.ToDouble(null);
        }
        public override int GetHashCode()
        {
            return Get().GetHashCode();
        }

        /// <returns>返回操作前的值</returns>
        public virtual long SetROri(long value) => Interlocked.Exchange(ref v, value);
        public virtual void Add(long x) => Interlocked.Add(ref v, x);
        public virtual void Add(int x) => Interlocked.Add(ref v, x);
        public virtual long AddRNow(long x) => Interlocked.Add(ref v, x);

        public virtual void Sub(long x) => Interlocked.Add(ref v, -x);
        public virtual long SubRNow(long x) => Interlocked.Add(ref v, -x);

        public virtual long Inc() => Interlocked.Increment(ref v);
        public virtual long Dec() => Interlocked.Decrement(ref v);
        /// <returns>返回操作前的值</returns>
        public virtual long CompareExROri(long newV, long compareTo) => Interlocked.CompareExchange(ref v, newV, compareTo);
    }

    /// <summary>
    /// 参数要求倍率speed（默认1）
    /// 可（在构造时）以设定ISafeConvertible<long>类的Score，默认初始为0的AtomicLong
    /// 在发生正向的变化时，自动给Score加上正向变化的差乘以speed取整。
    /// 注意：AtomicLongOnlyAddScore本身为AtomicLong，提供的Score可能构成环而死锁。
    /// </summary>
    public class AtomicLongOnlyAddScore(long x, double speed = 1.0) : AtomicLong(x)
    {
        public IAddable<long> score = new AtomicLong(0);
        public IAddable<long> Score
        {
            get
            {
                return Interlocked.CompareExchange(ref score!, null, null)!;
            }
            set
            {
                Interlocked.Exchange(ref score, value);
            }
        }
        public AtomicDouble speed = new(speed);

        /// <returns>返回操作前的值</returns>
        public override long SetROri(long value)
        {
            long previousV = Interlocked.Exchange(ref v, value);
            if (value - previousV > 0)
                Score.Add(Convert.ToInt32((value - previousV) * speed));
            return previousV;
        }
        /// <returns>返回操作前的值</returns>
        public long SetROriNotAddScore(long value)
        {
            return Interlocked.Exchange(ref v, value);
        }
        public override void Add(long x)
        {
            if (x > 0) Score.Add(Convert.ToInt32(x * speed));
            Interlocked.Add(ref v, x);
        }
        public override long AddRNow(long x)
        {
            if (x > 0) Score.Add(Convert.ToInt32(x * speed));
            return Interlocked.Add(ref v, x);
        }
        public void AddNotAddScore(long x)
        {
            Interlocked.Add(ref v, x);
        }

        public override void Sub(long x)
        {
            if (x < 0) Score.Add(Convert.ToInt32(-x * speed));
            Interlocked.Add(ref v, -x);
        }
        public override long SubRNow(long x)
        {
            if (x < 0) Score.Add(Convert.ToInt32(-x * speed));
            return Interlocked.Add(ref v, -x);
        }
        public void SubNotAddScore(long x)
        {
            Interlocked.Add(ref v, -x);
        }

        public override long Inc()
        {
            Score.Add(Convert.ToInt32(speed));
            return Interlocked.Increment(ref v);
        }
        public long IncNotAddScore()
        {
            return Interlocked.Increment(ref v);
        }
        /// <returns>返回操作前的值</returns>
        public override long CompareExROri(long newV, long compareTo)
        {
            long previousV = Interlocked.CompareExchange(ref v, newV, compareTo);
            if (newV - previousV > 0)
                Score.Add(Convert.ToInt32((newV - previousV) * speed));
            return previousV;
        }
        /// <returns>返回操作前的值</returns>
        public long CompareExReturnOriNotAddScore(long newV, long compareTo)
        {
            return Interlocked.CompareExchange(ref v, newV, compareTo);
        }
    }
}