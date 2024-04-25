using System;
using System.Threading;
using Preparation.Interface;
using Preparation.Utility.Value.SafeValue.Atomic;
using Preparation.Utility.Value.SafeValue.TimeBased;

namespace Preparation.Utility.Value.SafeValue.LockedValue
{
    //其对应属性不应当有set访问器，避免不安全的=赋值

    public class LongInVariableRangeWithStartTime : InVariableRange<long>
    {
        public StartTime startTime = new();
        public LongInVariableRangeWithStartTime(long value, long maxValue) : base(value, maxValue) { }
        /// <summary>
        /// 默认使Value=maxValue
        /// </summary>
        public LongInVariableRangeWithStartTime(long maxValue) : base(maxValue) { }

        #region 读取
        public (long, long) GetValueWithStartTime()
        {
            return ReadNeed(() => (v, startTime.Get()));
        }
        public (long, long, long) GetValueAndMaxVWithStartTime()
        {
            return ReadNeed(() => (v, maxV, startTime.Get()));
        }
        #endregion

        /// <summary>
        /// 试图加到满，如果加上时间差*速度可以达到MaxV，则加上并使startTime变为long.MaxValue
        /// 如果无法加到maxValue则不加
        /// </summary>
        /// <returns>返回试图加到的值与最大值</returns>
        public (long, long, long) AddStartTimeToMaxV(double speed = 1.0)
        {
            return WriteNeed(() =>
            {
                long addV = (long)(startTime.StopIfPassing(maxV - v) * speed);
                if (addV < 0)
                    return (v, maxV, startTime.Get());
                if (maxV - v < addV)
                    return (v = maxV, maxV, startTime.Get());
                return (v + addV, maxV, startTime.Get());
            });
        }

        /// <summary>
        /// 增加量为时间差*速度，并将startTime变为long.MaxValue
        /// </summary>
        /// <returns>返回实际改变量</returns>
        public long AddStartTime(double speed = 1.0)
        {
            return WriteNeed(() =>
            {
                long previousV = v;
                long addV = Environment.TickCount64 - startTime.Stop();
                if (addV > 0) v += (long)(addV * speed);
                else return 0;
                if (v > maxV) v = maxV;
                return v - previousV;
            });
        }

        /// <summary>
        /// 试图加到满，如果加上时间差*速度可以达到MaxV，则加上
        /// 如果无法加到maxValue则清零
        /// 无论如何startTime变为long.MaxValue
        /// </summary>
        /// <returns>返回是否清零</returns>
        public bool Set0IfNotAddStartTimeToMaxV(double speed = 1.0)
        {
            return WriteNeed(() =>
            {
                if (v == maxV) return false;
                long addV = (long)(startTime.Stop() * speed);
                if (addV < 0)
                {
                    v = 0;
                    return true;
                }
                if (maxV - v < addV)
                {
                    v = maxV;
                    return false;
                }
                v = 0;
                return false;
            });
        }

        public void SetAndStop(long value = 0)
        {
            WriteNeed(() =>
            {
                v = value;
                startTime.Stop();
            });
        }
    }

    public class TimeBasedProgressAtVariableSpeed
    {
        private readonly LongInVariableRangeWithStartTime progress;
        private IDouble speed;
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

        #region 构造 
        public TimeBasedProgressAtVariableSpeed(long needProgress, double speed = 1.0)
        {
            progress = new LongInVariableRangeWithStartTime(0, needProgress);
            if (needProgress <= 0)
                LockedValueLogging.logger.ConsoleLogDebug(
                    $"Bug: TimeBasedProgressAtVariableSpeed.needProgress({needProgress}) is less than 0");
            this.speed = new AtomicDouble(speed);
        }
        public TimeBasedProgressAtVariableSpeed()
        {
            progress = new LongInVariableRangeWithStartTime(0, 0);
            speed = new AtomicDouble(1.0);
        }
        #endregion

        #region 读取
        public override string ToString()
        {
            long progressStored, lastStartTime;
            (progressStored, lastStartTime) = progress.GetValueWithStartTime();
            return $"ProgressStored: {progressStored}; "
                   + $"LastStartTime: {lastStartTime} ms; "
                   + $"Speed: {speed}";
        }
        public long GetProgressNow()
            => progress.AddStartTimeToMaxV(speed.ToDouble()).Item1;
        public (long, long, long) GetProgressNowAndNeedTimeAndLastStartTime()
            => progress.AddStartTimeToMaxV(speed.ToDouble());
        public long GetProgressStored()
            => progress.GetValue();
        public (long, long) GetProgressStoredAndNeedTime()
            => progress.GetValueAndMaxV();
        public (long, long, long) GetProgressStoredAndNeedTimeAndLastStartTime()
            => progress.GetValueAndMaxVWithStartTime();

        public bool IsFinished()
        {
            long progressNow, needTime;
            (progressNow, needTime, _) = progress.AddStartTimeToMaxV(speed.ToDouble());
            return progressNow == needTime;
        }
        public bool IsProgressing()
        {
            long progressNow, needTime, startT;
            (progressNow, needTime, startT) = progress.AddStartTimeToMaxV(speed.ToDouble());
            return startT != long.MaxValue && progressNow != needTime;
        }
        #endregion

        public bool Start(long needTime)
        {
            if (needTime <= 2)
            {
                LockedValueLogging.logger.ConsoleLogDebug(
                    $"Warning: Start TimeBasedProgressAtVariableSpeed with the needProgress({needTime}) which is less than 0");
                return false;
            }
            if (progress.startTime.Start() != long.MaxValue)
                return false;
            progress.SetMaxV(needTime);
            return true;
        }
        public bool Start()
        {
            return progress.startTime.Start() == long.MaxValue;
        }
        /// <summary>
        /// 使进度条强制终止清零
        /// </summary>
        public void Set0()
        {
            progress.SetAndStop();
        }
        /// <summary>
        /// 如果进度条加上时间差不能为满，使进度条强制终止清零
        /// </summary>
        public void TryStop()
        {
            progress.Set0IfNotAddStartTimeToMaxV(speed.ToDouble());
        }
        /// <summary>
        /// 使进度条暂停
        /// </summary>
        public bool Pause()
        {
            return progress.AddStartTime(speed.ToDouble()) != 0;
        }
        /// <summary>
        /// 使进度条进度为满
        /// </summary>
        public void Finish()
        {
            progress.SetVToMaxV();
            progress.startTime.Stop();
        }
    }
}
