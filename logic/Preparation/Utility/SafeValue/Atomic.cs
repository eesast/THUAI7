using System;
using System.Threading;

namespace Preparation.Utility
{
    //其对应属性不应当有set访问器，避免不安全的=赋值
    public abstract class Atomic
    {
    }

    public class AtomicInt : Atomic
    {
        protected int v;
        public AtomicInt(int x)
        {
            v = x;
        }
        public override string ToString() => Interlocked.CompareExchange(ref v, -1, -1).ToString();
        public int Get() => Interlocked.CompareExchange(ref v, -1, -1);
        public static implicit operator int(AtomicInt aint) => Interlocked.CompareExchange(ref aint.v, -1, -1);
        /// <returns>返回操作前的值</returns>
        public virtual int SetReturnOri(int value) => Interlocked.Exchange(ref v, value);
        public virtual int Add(int x) => Interlocked.Add(ref v, x);
        /// <summary>
        /// 注意：确保参数为非负数
        /// </summary>
        public virtual int AddPositive(int x) => Interlocked.Add(ref v, x);
        public virtual int Sub(int x) => Interlocked.Add(ref v, -x);
        /// <summary>
        /// 注意：确保参数为非负数
        /// </summary>
        public virtual int SubPositive(int x) => Interlocked.Add(ref v, -x);
        public virtual int Inc() => Interlocked.Increment(ref v);
        public virtual int Dec() => Interlocked.Decrement(ref v);
        /// <returns>返回操作前的值</returns>
        public virtual int CompareExReturnOri(int newV, int compareTo) => Interlocked.CompareExchange(ref v, newV, compareTo);
    }

    /// <summary>
    /// 参数要求倍率speed（默认1）
    /// 可（在构造时）使用SetScore以设定AtomicInt类的Score
    /// 在发生正向的变化时，自动给Score加上正向变化的差乘以speed（取整）。
    /// 注意：AtomicIntOnlyAddScore本身为AtomicInt，提供的Score可能构成环而死锁。
    /// </summary>
    public class AtomicIntOnlyAddScore : AtomicInt
    {
        public AtomicInt Score { get; set; }
        public AtomicDouble speed;
        public AtomicIntOnlyAddScore(int x, double speed = 1.0) : base(x)
        {
            this.speed = new(speed);
            this.Score = new(0);
        }
        public void SetScore(AtomicInt score) => Score = score;
        /// <returns>返回操作前的值</returns>
        public override int SetReturnOri(int value)
        {
            int previousV = Interlocked.Exchange(ref v, value);
            if (value - previousV > 0)
                Score.AddPositive(Convert.ToInt32((value - previousV) * speed));
            return previousV;
        }
        /// <returns>返回操作前的值</returns>
        public int SetReturnOriNotAddScore(int value)
        {
            return Interlocked.Exchange(ref v, value);
        }
        public override int Add(int x)
        {
            if (x > 0) Score.AddPositive(Convert.ToInt32(x * speed));
            return Interlocked.Add(ref v, x);
        }
        public int AddNotAddScore(int x)
        {
            return Interlocked.Add(ref v, x);
        }
        /// <summary>
        /// 注意：确保参数为非负数
        /// </summary>
        public override int AddPositive(int x)
        {
            Score.AddPositive(Convert.ToInt32(x * speed));
            return Interlocked.Add(ref v, x);
        }
        public override int Sub(int x)
        {
            if (x < 0) Score.AddPositive(Convert.ToInt32((-x) * speed));
            return Interlocked.Add(ref v, -x);
        }
        public int SubNotAddScore(int x)
        {
            return Interlocked.Add(ref v, -x);
        }
        /// <summary>
        /// 注意：确保参数为非负数
        /// </summary>
        public override int SubPositive(int x)
        {
            return Interlocked.Add(ref v, -x);
        }
        public override int Inc()
        {
            Score.AddPositive(Convert.ToInt32(speed));
            return Interlocked.Increment(ref v);
        }
        public int IncNotAddScore()
        {
            return Interlocked.Increment(ref v);
        }
        /// <returns>返回操作前的值</returns>
        public override int CompareExReturnOri(int newV, int compareTo)
        {
            int previousV = Interlocked.CompareExchange(ref v, newV, compareTo);
            if (newV - previousV > 0)
                Score.AddPositive(Convert.ToInt32((newV - previousV) * speed));
            return previousV;
        }
        /// <returns>返回操作前的值</returns>
        public int CompareExReturnOriNotAddScore(int newV, int compareTo)
        {
            return Interlocked.CompareExchange(ref v, newV, compareTo);
        }
    }

    /// <summary>
    /// 参数要求倍率speed（默认1）
    /// 可（在构造时）使用SetScore以设定AtomicLong类的Score（取整）
    /// 在发生正向的变化时，自动给Score加上正向变化的差乘以speed。
    /// </summary>
    public class AtomicIntOnlyAddLongScore : AtomicInt
    {
        public AtomicLong Score { get; set; }
        public AtomicDouble speed;
        public AtomicIntOnlyAddLongScore(int x, double speed = 1.0) : base(x)
        {
            this.Score = new(0);
            this.speed = new(speed);
        }
        public void SetScore(AtomicLong score) => Score = score;
        /// <returns>返回操作前的值</returns>
        public override int SetReturnOri(int value)
        {
            int previousV = Interlocked.Exchange(ref v, value);
            if (value - previousV > 0)
                Score.AddPositive((Convert.ToInt32((value - previousV) * speed)));
            return previousV;
        }
        /// <returns>返回操作前的值</returns>
        public int SetReturnOriNotAddScore(int value)
        {
            return Interlocked.Exchange(ref v, value);
        }
        public override int Add(int x)
        {
            if (x > 0) Score.AddPositive(Convert.ToInt32(x * speed));
            return Interlocked.Add(ref v, x);
        }
        public int AddNotAddScore(int x)
        {
            return Interlocked.Add(ref v, x);
        }
        /// <summary>
        /// 注意：确保参数为非负数
        /// </summary>
        public override int AddPositive(int x)
        {
            Score.AddPositive(Convert.ToInt32((x) * speed));
            return Interlocked.Add(ref v, x);
        }
        public override int Sub(int x)
        {
            if (x < 0) Score.AddPositive(Convert.ToInt32((-x) * speed));
            return Interlocked.Add(ref v, -x);
        }
        public int SubNotAddScore(int x)
        {
            return Interlocked.Add(ref v, -x);
        }
        public override int Inc()
        {
            Score.AddPositive(Convert.ToInt32(speed));
            return Interlocked.Increment(ref v);
        }
        public int IncNotAddScore()
        {
            return Interlocked.Increment(ref v);
        }
        /// <returns>返回操作前的值</returns>
        public override int CompareExReturnOri(int newV, int compareTo)
        {
            int previousV = Interlocked.CompareExchange(ref v, newV, compareTo);
            if (newV - previousV > 0)
                Score.AddPositive(Convert.ToInt32((newV - previousV) * speed));
            return previousV;
        }
        /// <returns>返回操作前的值</returns>
        public int CompareExReturnOriNotAddScore(int newV, int compareTo)
        {
            return Interlocked.CompareExchange(ref v, newV, compareTo);
        }
    }

    /// <summary>
    /// 参数要求倍率speed（默认1）
    /// 可（在构造时）使用SetScore以设定AtomicInt类的Score
    /// 在发生变化时，自动给Score加上变化的差乘以speed取整。
    /// 注意：AtomicIntChangeAffectScore本身为AtomicInt，提供的Score可能构成环而死锁。
    /// </summary>
    public class AtomicIntChangeAffectScore : AtomicInt
    {
        public AtomicInt Score { get; set; }
        public AtomicDouble speed;
        public AtomicIntChangeAffectScore(int x, double speed = 1.0) : base(x)
        {
            this.Score = new(0);
            this.speed = new(speed);
        }
        public void SetScore(AtomicInt score) => Score = score;
        /// <returns>返回操作前的值</returns>
        public override int SetReturnOri(int value)
        {
            int previousV = Interlocked.Exchange(ref v, value);
            Score.Add(Convert.ToInt32((value - previousV) * speed));
            return previousV;
        }
        /// <returns>返回操作前的值</returns>
        public int SetReturnOriNotAddScore(int value)
        {
            return Interlocked.Exchange(ref v, value);
        }
        public override int Add(int x)
        {
            Score.Add(Convert.ToInt32((x) * speed));
            return Interlocked.Add(ref v, x);
        }
        public int AddNotAddScore(int x)
        {
            return Interlocked.Add(ref v, x);
        }
        /// <summary>
        /// 注意：确保参数为非负数
        /// </summary>
        public override int AddPositive(int x)
        {
            Score.AddPositive(Convert.ToInt32((x) * speed));
            return Interlocked.Add(ref v, x);
        }
        public override int Sub(int x)
        {
            Score.Sub(Convert.ToInt32((x) * speed));
            return Interlocked.Add(ref v, -x);
        }
        public int SubNotAddScore(int x)
        {
            return Interlocked.Add(ref v, -x);
        }
        /// <summary>
        /// 注意：确保参数为非负数
        /// </summary>
        public override int SubPositive(int x)
        {
            Score.SubPositive(Convert.ToInt32((x) * speed));
            return Interlocked.Add(ref v, -x);
        }
        public override int Inc()
        {
            Score.AddPositive(Convert.ToInt32(speed));
            return Interlocked.Increment(ref v);
        }
        public int IncNotAddScore()
        {
            return Interlocked.Increment(ref v);
        }
        public override int Dec()
        {
            Score.SubPositive(Convert.ToInt32(speed));
            return Interlocked.Decrement(ref v);
        }
        public int DecNotAddScore()
        {
            return Interlocked.Decrement(ref v);
        }
        /// <returns>返回操作前的值</returns>
        public override int CompareExReturnOri(int newV, int compareTo)
        {
            int previousV = Interlocked.CompareExchange(ref v, newV, compareTo);
            Score.Add(Convert.ToInt32((newV - previousV) * speed));
            return previousV;
        }
        /// <returns>返回操作前的值</returns>
        public int CompareExReturnOriNotAddScore(int newV, int compareTo)
        {
            return Interlocked.CompareExchange(ref v, newV, compareTo);
        }
    }

    public class AtomicLong : Atomic
    {
        protected long v;
        public AtomicLong(long x)
        {
            v = x;
        }
        public override string ToString() => Interlocked.CompareExchange(ref v, -1, -1).ToString();
        public long Get() => Interlocked.CompareExchange(ref v, -1, -1);
        public static implicit operator long(AtomicLong along) => Interlocked.CompareExchange(ref along.v, -1, -1);
        /// <returns>返回操作前的值</returns>
        public virtual long SetReturnOri(long value) => Interlocked.Exchange(ref v, value);
        public virtual long Add(long x) => Interlocked.Add(ref v, x);
        /// <summary>
        /// 注意：确保参数为非负数
        /// </summary>
        public virtual long AddPositive(long x) => Interlocked.Add(ref v, x);
        public virtual long Sub(long x) => Interlocked.Add(ref v, -x);
        /// <summary>
        /// 注意：确保参数为非负数
        /// </summary>
        public virtual long SubPositive(long x) => Interlocked.Add(ref v, -x);
        public virtual long Inc() => Interlocked.Increment(ref v);
        public virtual long Dec() => Interlocked.Decrement(ref v);
        /// <returns>返回操作前的值</returns>
        public virtual long CompareExReturnOri(long newV, long compareTo) => Interlocked.CompareExchange(ref v, newV, compareTo);
    }

    /// <summary>
    /// 参数要求倍率speed（默认1）
    /// 可（在构造时）使用SetScore以设定AtomicLong类的Score
    /// 在发生正向的变化时，自动给Score加上正向变化的差乘以speed取整。
    /// 注意：AtomicLongOnlyAddScore本身为AtomicLong，提供的Score可能构成环而死锁。
    /// </summary>
    public class AtomicLongOnlyAddScore : AtomicLong
    {
        public AtomicLong Score { get; set; }
        public AtomicDouble speed;
        public AtomicLongOnlyAddScore(long x, double speed = 1.0) : base(x)
        {
            this.Score = new(0);
            this.speed = new(speed);
        }
        public void SetScore(AtomicLong score) => Score = score;

        /// <returns>返回操作前的值</returns>
        public override long SetReturnOri(long value)
        {
            long previousV = Interlocked.Exchange(ref v, value);
            if (value - previousV > 0)
                Score.AddPositive(Convert.ToInt32((value - previousV) * speed));
            return previousV;
        }
        /// <returns>返回操作前的值</returns>
        public long SetReturnOriNotAddScore(long value)
        {
            return Interlocked.Exchange(ref v, value);
        }
        public override long Add(long x)
        {
            if (x > 0) Score.AddPositive(Convert.ToInt32((x) * speed));
            return Interlocked.Add(ref v, x);
        }
        public long AddNotAddScore(long x)
        {
            return Interlocked.Add(ref v, x);
        }
        /// <summary>
        /// 注意：确保参数为非负数
        /// </summary>
        public override long AddPositive(long x)
        {
            Score.AddPositive(Convert.ToInt32((x) * speed));
            return Interlocked.Add(ref v, x);
        }
        public override long Sub(long x)
        {
            if (x < 0) Score.AddPositive(Convert.ToInt32((-x) * speed));
            return Interlocked.Add(ref v, -x);
        }
        public long SubNotAddScore(long x)
        {
            return Interlocked.Add(ref v, -x);
        }
        public override long Inc()
        {
            Score.AddPositive(Convert.ToInt32(speed));
            return Interlocked.Increment(ref v);
        }
        public long IncNotAddScore()
        {
            return Interlocked.Increment(ref v);
        }
        /// <returns>返回操作前的值</returns>
        public override long CompareExReturnOri(long newV, long compareTo)
        {
            long previousV = Interlocked.CompareExchange(ref v, newV, compareTo);
            if (newV - previousV > 0)
                Score.AddPositive(Convert.ToInt32((newV - previousV) * speed));
            return previousV;
        }
        /// <returns>返回操作前的值</returns>
        public long CompareExReturnOriNotAddScore(long newV, long compareTo)
        {
            return Interlocked.CompareExchange(ref v, newV, compareTo);
        }
    }

    public class AtomicDouble : Atomic
    {
        private double v;
        public AtomicDouble(double x)
        {
            v = x;
        }
        public override string ToString() => Interlocked.CompareExchange(ref v, -2.0, -2.0).ToString();
        public double Get() => Interlocked.CompareExchange(ref v, -2.0, -2.0);
        public static implicit operator double(AtomicDouble adouble) => Interlocked.CompareExchange(ref adouble.v, -2.0, -2.0);
        /// <returns>返回操作前的值</returns>
        public double SetReturnOri(double value) => Interlocked.Exchange(ref v, value);
        /// <returns>返回操作前的值</returns>
        public double CompareExReturnOri(double newV, double compareTo) => Interlocked.CompareExchange(ref v, newV, compareTo);
    }

    public class AtomicBool : Atomic
    {
        private int v;//v&1==0为false,v&1==1为true
        public AtomicBool(bool x)
        {
            v = x ? 1 : 0;
        }
        public override string ToString() => ((Interlocked.CompareExchange(ref v, -2, -2) & 1) == 0) ? "false" : "true";
        public bool Get() => ((Interlocked.CompareExchange(ref v, -2, -2) & 1) == 1);
        public static implicit operator bool(AtomicBool abool) => abool.Get();
        /// <returns>返回操作前的值</returns>
        public bool SetReturnOri(bool value) => ((Interlocked.Exchange(ref v, value ? 1 : 0) & 1) == 1);
        /// <returns>赋值前的值是否与将赋予的值不相同</returns>
        public bool TrySet(bool value)
        {
            return ((Interlocked.Exchange(ref v, value ? 1 : 0) & 1) != (value ? 1 : 0));
        }
        public bool And(bool x) => (Interlocked.And(ref v, x ? 1 : 0) & 1) == 1;
        public bool Or(bool x) => (Interlocked.Or(ref v, x ? 1 : 0) & 1) == 1;
        /// <returns>返回操作后的值</returns>
        public bool Reverse() => (Interlocked.Increment(ref v) & 1) == 1;
        public bool Xor(bool x) => (Interlocked.Add(ref v, x ? 1 : 0) & 1) == 1;
    }
}