using Preparation.Utility.Value.SafeValue.Atomic;
using System;
using System.Threading;

namespace Preparation.Utility.Value.SafeValue.TimeBased
{
    //其对应属性不应当有set访问器，避免不安全的=赋值

    /// <summary>
    /// 记录上次Start的时间，尚未Start则为long.MaxValue
    /// 当前不为long.MaxValue则不能Start
    /// </summary>
    public class StartTime : IConvertible
    {
        private long _time;
        public StartTime(long time)
        {
            _time = time;
        }
        public StartTime() { _time = long.MaxValue; }

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

        public long Get() => Interlocked.CompareExchange(ref _time, -2, -2);
        public override string ToString() => Get().ToString();
        public override bool Equals(object? obj)
        {
            return obj != null && (obj is IConvertible k) && ToDouble(null) == k.ToDouble(null);
        }
        public override int GetHashCode()
        {
            return Get().GetHashCode();
        }

        /// <returns>返回操作前的值</returns>
        public long Start() => Interlocked.CompareExchange(ref _time, Environment.TickCount64, long.MaxValue);
        /// <returns>返回操作前的值</returns>
        public long Stop() => Interlocked.Exchange(ref _time, long.MaxValue);
        /// <returns>返回时间差,<0意味着未开始</returns>
        public long StopIfPassing(long passedTime)
        {
            long ans = Environment.TickCount64 - Interlocked.CompareExchange(ref _time, -2, -2);
            if (ans > passedTime)
            {
                Interlocked.Exchange(ref _time, long.MaxValue);
            }
            return ans;
        }
    }

    /// <summary>
    /// 根据时间推算Start后完成多少进度的进度条（long）。
    /// 只允许Start（清零状态的进度条才可以Start）时修改needTime（请确保大于0）；
    /// 支持InterruptToSet0使未完成的进度条终止清零；支持Set0使进度条强制终止清零；
    /// 通过原子操作实现。
    /// </summary>
    public class TimeBasedProgressOptimizedForInterrupting
    {
        private long endT = long.MaxValue;
        private long needT;

        public TimeBasedProgressOptimizedForInterrupting(long needTime)
        {
            if (needTime <= 0)
                TimeBasedLogging.logger.ConsoleLogDebug(
                    $"Bug: TimeBasedProgressOptimizedForInterrupting.needProgress({needTime}) is less than 0");
            needT = needTime;
        }
        public TimeBasedProgressOptimizedForInterrupting()
        {
            needT = 0;
        }
        public long GetEndTime() => Interlocked.CompareExchange(ref endT, -2, -2);
        public long GetNeedTime() => Interlocked.CompareExchange(ref needT, -2, -2);
        public override string ToString()
            => $"EndTime: {Interlocked.CompareExchange(ref endT, -2, -2)} ms, NeedTime: {Interlocked.CompareExchange(ref needT, -2, -2)} ms";
        public bool IsFinished()
        {
            return Interlocked.CompareExchange(ref endT, -2, -2) <= Environment.TickCount64;
        }
        public bool IsStarted() => Interlocked.Read(ref endT) != long.MaxValue;
        /// <summary>
        /// GetProgress<0则表明未开始
        /// </summary>
        public long GetProgress()
        {
            long cutime = Interlocked.CompareExchange(ref endT, -2, -2) - Environment.TickCount64;
            if (cutime <= 0)
                return Interlocked.CompareExchange(ref needT, -2, -2);
            return Interlocked.CompareExchange(ref needT, -2, -2) - cutime;
        }
        public long GetNonNegativeProgress()
        {
            long cutime = Interlocked.CompareExchange(ref endT, -2, -2) - Environment.TickCount64;
            if (cutime <= 0)
                return Interlocked.CompareExchange(ref needT, -2, -2);
            long progress = Interlocked.CompareExchange(ref needT, -2, -2) - cutime;
            return progress < 0 ? 0 : progress;
        }
        /// <summary>
        /// GetProgress<0则表明未开始
        /// </summary>
        public long GetProgress(long time)
        {
            long cutime = Interlocked.CompareExchange(ref endT, -2, -2) - time;
            if (cutime <= 0)
                return Interlocked.CompareExchange(ref needT, -2, -2);
            return Interlocked.CompareExchange(ref needT, -2, -2) - cutime;
        }
        public long GetNonNegativeProgress(long time)
        {
            long cutime = Interlocked.Read(ref endT) - time;
            if (cutime <= 0)
                return Interlocked.CompareExchange(ref needT, -2, -2);
            long progress = Interlocked.CompareExchange(ref needT, -2, -2) - cutime;
            return progress < 0 ? 0 : progress;
        }
        /// <summary>
        /// 小于0则表明未开始
        /// </summary>
        public static implicit operator long(TimeBasedProgressOptimizedForInterrupting pLong)
            => pLong.GetProgress();

        /// <summary>
        /// GetProgressDouble < 0则表明未开始
        /// </summary>
        public double GetProgressDouble()
        {
            long cutime = Interlocked.CompareExchange(ref endT, -2, -2) - Environment.TickCount64;
            if (cutime <= 0)
                return 1;
            long needTime = Interlocked.CompareExchange(ref needT, -2, -2);
            if (needTime == 0)
                return 0;
            return 1.0 - (double)cutime / needTime;
        }
        public double GetNonNegativeProgressDouble(long time)
        {
            long cutime = Interlocked.Read(ref endT) - time;
            if (cutime <= 0)
                return 1;
            long needTime = Interlocked.CompareExchange(ref needT, -2, -2);
            if (needTime <= cutime)
                return 0;
            return 1.0 - (double)cutime / needTime;
        }

        public bool Start(long needTime)
        {
            if (needTime <= 0)
            {
                TimeBasedLogging.logger.ConsoleLogDebug(
                    $"Warning: Start TimeBasedProgressOptimizedForInterrupting with the needProgress({needTime}) which is less than 0");
                return false;
            }
            //规定只有Start可以修改needT，且需要先访问endTime，从而避免锁（某种程度上endTime可以认为是needTime的锁）
            if (Interlocked.CompareExchange(ref endT, Environment.TickCount64 + needTime, long.MaxValue) != long.MaxValue)
                return false;
            if (needTime <= 2)
                TimeBasedLogging.logger.ConsoleLogDebug(
                    $"Warning: The field of TimeBasedProgressOptimizedForInterrupting is {needTime},which is too small");
            Interlocked.Exchange(ref needT, needTime);
            return true;
        }
        public bool Start()
        {
            long needTime = Interlocked.CompareExchange(ref needT, -2, -2);
            if (Interlocked.CompareExchange(ref endT, Environment.TickCount64 + needTime, long.MaxValue) != long.MaxValue)
                return false;
            return true;
        }
        /// <summary>
        /// 使进度条强制终止清零
        /// </summary>
        public void Set0() => Interlocked.Exchange(ref endT, long.MaxValue);
        /// <summary>
        /// 使未完成的进度条终止清零
        /// </summary>
        public bool InterruptToSet0()
        {
            if (Environment.TickCount64 < Interlocked.CompareExchange(ref endT, -2, -2))
            {
                Interlocked.Exchange(ref endT, long.MaxValue);
                return true;
            }
            return false;
        }
        //增加其他新的写操作可能导致不安全
    }

    /// <summary>
    /// 冷却时间为可变的CDms的bool，不支持查看当前进度，初始为True
    /// </summary>
    public class BoolUpdateEachCD
    {
        private long cd;
        private long nextUpdateTime = 0;
        public BoolUpdateEachCD(int cd)
        {
            if (cd <= 1)
                TimeBasedLogging.logger.ConsoleLogDebug($"Bug: BoolUpdateEachCD.cd({cd}) is less than 1");
            this.cd = cd;
        }
        public BoolUpdateEachCD(long cd)
        {
            if (cd <= 1)
                TimeBasedLogging.logger.ConsoleLogDebug($"Bug: BoolUpdateEachCD.cd({cd}) is less than 1");
            this.cd = cd;
        }
        public BoolUpdateEachCD(long cd, long startTime)
        {
            if (cd <= 1)
                TimeBasedLogging.logger.ConsoleLogDebug($"Bug: BoolUpdateEachCD.cd({cd}) is less than 1");
            this.cd = cd;
            nextUpdateTime = startTime;
        }

        public long GetCD() => Interlocked.Read(ref cd);

        public bool TryUse()
        {
            long needTime = Interlocked.Exchange(ref nextUpdateTime, long.MaxValue);
            if (needTime <= Environment.TickCount64)
            {
                Interlocked.Exchange(ref nextUpdateTime, Environment.TickCount64 + Interlocked.Read(ref cd));
                return true;
            }
            Interlocked.Exchange(ref nextUpdateTime, needTime);
            return false;
        }
        public void SetCD(int cd)
        {
            if (cd <= 1)
                TimeBasedLogging.logger.ConsoleLogDebug($"Bug: BoolUpdateEachCD.cd to {cd}");
            Interlocked.Exchange(ref this.cd, cd);
        }
    }

    /// <summary>
    /// 冷却时间为可变的CDms的进度条，初始为满
    /// </summary>
    public class LongProgressUpdateEachCD
    {
        private int isusing = 0;
        private long cd;
        private long nextUpdateTime = 0;
        public LongProgressUpdateEachCD(int cd)
        {
            if (cd <= 1)
                TimeBasedLogging.logger.ConsoleLogDebug($"Bug: LongProgressUpdateEachCD.cd({cd}) is less than 1");
            this.cd = cd;
        }
        public LongProgressUpdateEachCD(long cd)
        {
            if (cd <= 1)
                TimeBasedLogging.logger.ConsoleLogDebug($"Bug: LongProgressUpdateEachCD.cd({cd}) is less than 1");
            this.cd = cd;
        }
        public LongProgressUpdateEachCD(long cd, long startTime)
        {
            if (cd <= 1)
                TimeBasedLogging.logger.ConsoleLogDebug($"Bug: LongProgressUpdateEachCD.cd({cd}) is less than 1");
            this.cd = cd;
            nextUpdateTime = startTime;
        }

        public long GetRemainingTime()
        {
            long v = Interlocked.Read(ref nextUpdateTime) - Environment.TickCount64;
            return v < 0 ? 0 : v;
        }
        public long GetCD() => Interlocked.Read(ref cd);

        public bool TryUse()
        {
            if (Interlocked.Exchange(ref isusing, 1) == 1) return false;
            long needTime = Interlocked.Read(ref nextUpdateTime);
            if (needTime <= Environment.TickCount64)
            {
                Interlocked.Exchange(ref nextUpdateTime, Environment.TickCount64 + Interlocked.Read(ref cd));
                Interlocked.Exchange(ref isusing, 0);
                return true;
            }
            Interlocked.Exchange(ref isusing, 0);
            return false;
        }
        public void SetCD(int cd)
        {
            if (cd <= 1)
                TimeBasedLogging.logger.ConsoleLogDebug($"Bug: Set LongProgressUpdateEachCD.cd to {cd}");
            Interlocked.Exchange(ref this.cd, cd);
        }
    }

    /// <summary>
    /// 一个保证在[0,maxNum],每CDms自动+1的int，支持可变的CD、maxNum(请确保大于0)
    /// </summary>
    public class IntNumUpdateEachCD
    {
        private int num;
        private int maxNum;
        public AtomicInt CD { get; } = new(int.MaxValue);
        private long updateTime = 0;
        private readonly object numLock = new();
        public IntNumUpdateEachCD(int num, int maxNum, int cd)
        {
            if (num < 0)
                TimeBasedLogging.logger.ConsoleLogDebug($"Bug: IntNumUpdateEachCD.num({num}) is less than 0");
            if (maxNum < 0)
                TimeBasedLogging.logger.ConsoleLogDebug($"Bug: IntNumUpdateEachCD.maxNum({maxNum}) is less than 0");
            if (cd <= 0)
                TimeBasedLogging.logger.ConsoleLogDebug($"Bug: IntNumUpdateEachCD.cd({cd}) is less than 0");
            this.num = num < maxNum ? num : maxNum;
            this.maxNum = maxNum;
            CD.Set(cd);
            updateTime = Environment.TickCount64;
        }
        /// <summary>
        /// 默认使num=maxNum
        /// </summary>
        public IntNumUpdateEachCD(int maxNum, int cd)
        {
            if (maxNum < 0)
                TimeBasedLogging.logger.ConsoleLogDebug($"Bug: IntNumUpdateEachCD.maxNum({maxNum}) is less than 0");
            if (cd <= 0)
                TimeBasedLogging.logger.ConsoleLogDebug($"Bug: IntNumUpdateEachCD.cd({cd}) is less than 0");
            num = this.maxNum = maxNum;
            CD.Set(cd);
        }
        public IntNumUpdateEachCD()
        {
            num = maxNum = 0;
        }

        public int GetMaxNum() { lock (numLock) return maxNum; }
        public int GetCD() => CD.Get();
        public int GetNum(long time)
        {
            lock (numLock)
            {
                if (num < maxNum && time - updateTime >= CD)
                {
                    int add = (int)Math.Min(maxNum - num, (time - updateTime) / CD);
                    updateTime += add * CD;
                    return num += add;
                }
                return num;
            }
        }
        public static implicit operator int(IntNumUpdateEachCD aint) => aint.GetNum(Environment.TickCount64);

        /// <summary>
        /// 应当保证该subV>=0
        /// </summary>
        public int TrySub(int subV)
        {
            if (subV < 0)
                TimeBasedLogging.logger.ConsoleLogDebug($"Bug: IntNumUpdateEachCD Try to sub {subV}, which is less than 0");
            long time = Environment.TickCount64;
            lock (numLock)
            {
                if (num < maxNum && time - updateTime >= CD)
                {
                    int add = (int)Math.Min(maxNum - num, (time - updateTime) / CD);
                    updateTime += add * CD;
                    num += add;
                }
                if (num == maxNum) updateTime = time;
                num -= subV = Math.Min(subV, num);
            }
            return subV;
        }
        /// <summary>
        /// 应当保证该addV>=0
        /// </summary>
        public void TryAdd(int addV)
        {
            if (addV < 0)
                TimeBasedLogging.logger.ConsoleLogDebug($"Bug: IntNumUpdateEachCD Try to add {addV}, which is less than 0");
            lock (numLock)
            {
                num += Math.Min(addV, maxNum - num);
            }
        }
        /// <summary>
        /// 若maxNum<=0则maxNum及Num设为0并返回False
        /// </summary>
        public bool SetMaxNumAndNum(int maxNum)
        {
            if (maxNum < 0) maxNum = 0;
            lock (numLock)
            {
                num = this.maxNum = maxNum;
            }
            return maxNum > 0;
        }
        /// <summary>
        /// 应当保证该maxnum>=0
        /// </summary>
        public void SetPositiveMaxNumAndNum(int maxNum)
        {
            lock (numLock)
            {
                num = this.maxNum = maxNum;
            }
        }
        /// <summary>
        /// 应当保证该maxnum>=0
        /// </summary>
        public void SetPositiveMaxNum(int maxNum)
        {
            lock (numLock)
            {
                if ((this.maxNum = maxNum) < num)
                    num = maxNum;
            }
        }
        /// <summary>
        /// 若maxNum<=0则maxNum及Num设为0并返回False
        /// </summary>
        public bool SetMaxNum(int maxNum)
        {
            if (maxNum < 0) maxNum = 0;
            lock (numLock)
            {
                if ((this.maxNum = maxNum) < num)
                    num = maxNum;
            }
            return maxNum > 0;
        }
        /// <summary>
        /// 若num<0则num设为0并返回False
        /// </summary>
        public bool SetNum(int num)
        {
            lock (numLock)
            {
                if (num < 0)
                {
                    this.num = 0;
                    updateTime = Environment.TickCount64;
                    return false;
                }
                if (num < maxNum)
                {
                    if (this.num == maxNum) updateTime = Environment.TickCount64;
                    this.num = num;
                }
                else this.num = maxNum;
                return true;
            }
        }
        /// <summary>
        /// 应当保证该num>=0
        /// </summary>
        public void SetPositiveNum(int num)
        {
            lock (numLock)
            {
                if (num < maxNum)
                {
                    if (this.num == maxNum) updateTime = Environment.TickCount64;
                    this.num = num;
                }
                else this.num = maxNum;
            }
        }
        public void SetCD(int cd)
        {
            if (cd <= 0)
                TimeBasedLogging.logger.ConsoleLogDebug($"Bug: Set IntNumUpdateEachCD.cd to {cd}");
            CD.Set(cd);
        }
    }
}