using Preparation.Utility;
using System;
using System.Numerics;
using System.Threading;

namespace Preparation.Utility
{
    //其对应属性不应当有set访问器，避免不安全的=赋值

    /// <summary>
    /// 一个保证在[0,maxValue]的可变值，支持可变的maxValue（请确保大于0）
    /// </summary>
    public class InVariableRange<T> : LockedValue, IIntAddable, IAddable<T>
        where T : IConvertible, IComparable<T>, INumber<T>
    {
        protected T v;
        protected T maxV;
        #region 构造与读取
        public InVariableRange(T value, T maxValue) : base()
        {
            if (value < T.Zero)
            {
                Debugger.Output("Warning:Try to set IntInTheVariableRange to " + value.ToString() + ".");
                value = T.Zero;
            }
            if (maxValue < T.Zero)
            {
                Debugger.Output("Warning:Try to set IntInTheVariableRange.maxValue to " + maxValue.ToString() + ".");
                maxValue = T.Zero;
            }
            v = value.CompareTo(maxValue) < 0 ? value : maxValue;
            this.maxV = maxValue;
        }
        /// <summary>
        /// 默认使Value=maxValue
        /// </summary>
        public InVariableRange(T maxValue) : base()
        {
            if (maxValue < T.Zero)
            {
                Debugger.Output("Warning:Try to set IntInTheVariableRange.maxValue to " + maxValue.ToString() + ".");
                maxValue = T.Zero;
            }
            v = this.maxV = maxValue;
        }

        public override string ToString()
        {
            lock (vLock)
            {
                return "value:" + v.ToString() + " , maxValue:" + maxV.ToString();
            }
        }
        public T GetValue() { lock (vLock) return v; }
        public static implicit operator T(InVariableRange<T> aint) => aint.GetValue();
        public T GetMaxV() { lock (vLock) return maxV; }
        public (T, T) GetValueAndMaxV() { lock (vLock) return (v, maxV); }
        public bool IsMaxV() { lock (vLock) return v == maxV; }
        #endregion

        #region 内嵌读取（在锁的情况下读取内容同时读取其他更基本的外部数据）
        public (T, long) GetValue(StartTime startTime)
        {
            lock (vLock)
            {
                return (v, startTime.Get());
            }
        }
        public (T, T, long) GetValueAndMaxValue(StartTime startTime)
        {
            lock (vLock)
            {
                return (v, maxV, startTime.Get());
            }
        }
        #endregion

        #region 普通设置MaxV与Value的值的方法
        /// <summary>
        /// 若maxValue<=0则maxValue设为0并返回False
        /// </summary>
        public virtual bool SetMaxV(T maxValue)
        {
            if (maxValue.CompareTo(0) <= 0)
            {
                lock (vLock)
                {
                    v = maxV = T.Zero;
                    return false;
                }
            }
            lock (vLock)
            {
                maxV = maxValue;
                if (v > maxValue) v = maxValue;
            }
            return true;
        }
        /// <summary>
        /// 应当保证该maxValue>=0
        /// </summary>
        public virtual void SetPositiveMaxV(T maxValue)
        {
            lock (vLock)
            {
                maxV = maxValue;
                if (v > maxValue) v = maxValue;
            }
        }

        public virtual T SetRNow(T value)
        {
            if (value.CompareTo(0) <= 0)
            {
                lock (vLock)
                {
                    return v = T.Zero;
                }
            }
            lock (vLock)
            {
                return v = (value > maxV) ? maxV : value;
            }
        }
        /// <summary>
        /// 应当保证该value>=0
        /// </summary>
        public virtual T SetPositiveVRNow(T value)
        {
            lock (vLock)
            {
                return v = (value > maxV) ? maxV : value;
            }
        }
        #endregion

        #region 特殊条件的设置MaxV与Value的值的方法
        /// <summary>
        /// 如果当前值大于maxValue,则更新maxValue失败
        /// </summary>
        public virtual bool TrySetMaxV(T maxValue)
        {
            lock (vLock)
            {
                if (v > maxValue) return false;
                maxV = maxValue;
                return true;
            }
        }
        public virtual void SetVToMaxV()
        {
            lock (vLock)
            {
                v = maxV;
            }
        }
        public virtual void SetVToMaxV(double ratio)
        {
            lock (vLock)
            {
                v = T.CreateChecked(maxV.ToDouble(null) * ratio);
            }
        }
        public virtual bool Set0IfNotMaxor0()
        {
            lock (vLock)
            {
                if (v < maxV && v.CompareTo(0) > 0)
                {
                    v = T.Zero;
                    return true;
                }
            }
            return false;
        }
        public virtual bool Set0IfMax()
        {
            lock (vLock)
            {
                if (v == maxV)
                {
                    v = T.Zero;
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 普通运算
        public virtual void Add(T addV)
        {
            lock (vLock)
            {
                v += addV;
                if (v < T.Zero) v = T.Zero;
                if (v > maxV) v = maxV;
            }
        }
        public virtual void Add(int addV)
        {
            lock (vLock)
            {
                v += T.CreateChecked(addV);
                if (v < T.Zero) v = T.Zero;
                if (v > maxV) v = maxV;
            }
        }

        public virtual T AddRNow(T addV)
        {
            lock (vLock)
            {
                v += addV;
                if (v < T.Zero) v = T.Zero;
                if (v > maxV) v = maxV;
                return v;
            }
        }

        /// <returns>返回实际改变量</returns>
        public virtual T AddRChange(T addV)
        {
            lock (vLock)
            {
                T previousV = v;
                v += addV;
                if (v < T.Zero) v = T.Zero;
                if (v > maxV) v = maxV;
                return v - previousV;
            }
        }
        /// <summary>
        /// 应当保证增加值大于0
        /// </summary>
        /// <returns>返回实际改变量</returns>
        public virtual T AddPositiveVRChange(T addPositiveV)
        {
            lock (vLock)
            {
                addPositiveV = (addPositiveV < maxV - v) ? addPositiveV : maxV - v;
                v += addPositiveV;
            }
            return addPositiveV;
        }
        /// <summary>
        /// 应当保证增加值大于0
        /// </summary>
        public virtual void AddPositiveV(T addPositiveV)
        {
            lock (vLock)
            {
                v += addPositiveV;
                if (v > maxV) v = maxV;
            }
        }

        public virtual void Mul(T mulV)
        {
            if (mulV <= T.Zero)
            {
                lock (vLock) v = T.Zero;
                return;
            }
            lock (vLock)
            {
                if (v > maxV / mulV) v = maxV; //避免溢出
                else v *= mulV;
            }
        }
        public virtual void Mul<TA>(TA mulV) where TA : IConvertible, INumber<TA>
        {
            if (mulV < TA.Zero)
            {
                lock (vLock) v = T.Zero;
                return;
            }
            lock (vLock)
            {
                if (v > T.CreateChecked(maxV.ToDouble(null) / mulV.ToDouble(null))) v = maxV; //避免溢出
                else
                    v = T.CreateChecked(v.ToDouble(null) * mulV.ToDouble(null));
            }
        }
        /// <summary>
        /// 应当保证乘数大于0
        /// </summary>
        public virtual void MulPositiveV(T mulPositiveV)
        {
            lock (vLock)
            {
                if (v > maxV / mulPositiveV) v = maxV; //避免溢出
                else v *= mulPositiveV;
            }
        }
        /// <summary>
        /// 应当保证乘数大于0
        /// </summary>
        public virtual void MulPositiveV<TA>(TA mulV) where TA : IConvertible, INumber<TA>
        {
            lock (vLock)
            {
                if (v > T.CreateChecked(maxV.ToDouble(null) / mulV.ToDouble(null))) v = maxV; //避免溢出
                else
                    v = T.CreateChecked(v.ToDouble(null) * mulV.ToDouble(null));
            }
        }
        /// <returns>返回实际改变量</returns>
        public virtual T SubRChange(T subV)
        {
            lock (vLock)
            {
                T previousV = v;
                v -= subV;
                if (v < T.Zero) v = T.Zero;
                if (v > maxV) v = maxV;
                return v - previousV;
            }
        }

        public virtual T SubRNow(T subV)
        {
            lock (vLock)
            {
                v -= subV;
                if (v < T.Zero) v = T.Zero;
                if (v > maxV) v = maxV;
                return v;
            }
        }

        /// <summary>
        /// 应当保证该减少值大于0
        /// </summary>
        /// <returns>返回实际改变量</returns>
        public virtual T SubPositiveVRChange(T subPositiveV)
        {
            lock (vLock)
            {
                subPositiveV = (subPositiveV < v) ? subPositiveV : v;
                v -= subPositiveV;
            }
            return subPositiveV;
        }
        /// <summary>
        /// 应当保证该减少值大于0
        /// </summary>
        public virtual void SubPositiveV(T subPositiveV)
        {
            lock (vLock)
            {
                v = (subPositiveV < v) ? v - subPositiveV : T.Zero;
            }
        }
        #endregion

        #region 特殊条件的运算
        /// <summary>
        /// 试图加到满，如果无法加到maxValue则不加并返回-1
        /// </summary>
        /// <returns>返回实际改变量</returns>
        public virtual T TryAddToMaxVRChange(T addV)
        {
            lock (vLock)
            {
                if (maxV - v <= addV)
                {
                    addV = maxV - v;
                    v = maxV;
                    return addV;
                }
                return -T.One;
            }
        }

        /// <summary>
        /// ratio可以为负
        /// </summary>
        /// <returns>返回实际改变量</returns>
        public virtual T VAddPartMaxVRChange(double ratio)
        {
            lock (vLock)
            {
                T preV = v;
                v += T.CreateChecked(ratio * maxV.ToDouble(null));
                if (v < T.Zero) v = T.Zero;
                if (v > maxV) v = maxV;
                return v - preV;
            }
        }
        #endregion

        #region 与InVariableRange类的运算，运算会影响该对象的值
        public virtual T AddRChange<TA>(InVariableRange<TA> a, double speed = 1.0) where TA : IConvertible, IComparable<TA>, INumber<TA>
        {
            return EnterOtherLock<T>(a, () =>
            {
                T previousV = v;
                v += T.CreateChecked(a.GetValue().ToDouble(null) * speed);
                if (v > maxV) v = maxV;
                a.SubPositiveVRChange(TA.CreateChecked(v - previousV));
                return v - previousV;
            })!;
        }
        public virtual T AddVUseOtherRChange<TA>(T value, InVariableRange<TA> other, double speed = 1.0) where TA : IConvertible, IComparable<TA>, INumber<TA>
        {
            return EnterOtherLock<T>(other, () =>
            {
                T previousV = v;
                T otherValue = T.CreateChecked(other.GetValue().ToDouble(null) * speed);
                value = value > otherValue ? otherValue : value;
                v += value;
                if (v > maxV) v = maxV;
                other.SubPositiveVRChange(TA.CreateChecked((v - previousV).ToDouble(null) / speed));
                return v - previousV;
            })!;
        }
        public virtual T SubRChange<TA>(InVariableRange<TA> a) where TA : IConvertible, IComparable<TA>, IComparable<int>, INumber<TA>
        {
            return EnterOtherLock<T>(a, () =>
            {
                T previousV = v;
                v -= T.CreateChecked(a.GetValue());
                if (v < T.Zero) v = T.Zero;
                a.SubPositiveVRChange(TA.CreateChecked(previousV - v));
                return v - previousV;
            })!;
        }
        #endregion

        #region 与StartTime类的特殊条件的运算，运算会影响StartTime类的值
        /// <summary>
        /// 试图加到满，如果加上时间差*速度可以达到MaxV，则加上并使startTime变为long.MaxValue
        /// 如果无法加到maxValue则不加
        /// </summary>
        /// <returns>返回试图加到的值与最大值</returns>
        public virtual (T, T, long) TryAddToMaxV(StartTime startTime, double speed = 1.0)
        {
            lock (vLock)
            {
                long addV = (long)(startTime.StopIfPassing((maxV - v).ToInt64(null)) * speed);
                if (addV < 0) return (v, maxV, startTime.Get());
                if (maxV - v < T.CreateChecked(addV)) return (v = maxV, maxV, startTime.Get());
                return (v, maxV, startTime.Get());
            }
        }
        /// <summary>
        /// 增加量为时间差*速度，并将startTime变为long.MaxValue
        /// </summary>
        /// <returns>返回实际改变量</returns>
        public virtual T AddRChange(StartTime startTime, double speed = 1.0)
        {
            lock (vLock)
            {
                T previousV = v;
                T addV = T.CreateChecked((Environment.TickCount64 - startTime.Stop()) * speed);
                if (addV <= T.Zero) return T.Zero;
                else v += addV;
                if (v > maxV) v = maxV;
                return v - previousV;
            }
        }

        /// <summary>
        /// 试图加到满，如果加上时间差*速度可以达到MaxV，则加上并使startTime变为long.MaxValue
        /// 如果无法加到maxValue则清零
        /// </summary>
        /// <returns>返回是否清零</returns>
        public virtual bool Set0IfNotAddToMaxV(StartTime startTime, double speed = 1.0)
        {
            lock (vLock)
            {
                if (v == maxV) return false;
                T addV = T.CreateChecked(startTime.StopIfPassing((maxV - v).ToInt64(null)) * speed);
                if (addV < T.Zero)
                {
                    v = T.Zero;
                    return true;
                }
                if (maxV - v < addV)
                {
                    v = maxV;
                    return false;
                }
                v = T.Zero;
                return true;
            }
        }
        #endregion
    }

    /// <summary>
    /// 可以设定IIntAddable类的Score，默认初始为0的AtomicInt
    /// 在发生正向的变化时，自动给Score加上正向变化的差乘以speed（取整）。
    /// </summary>
    public class InVariableRangeOnlyAddScore<T>(T value, T maxValue, double speed = 1.0) : InVariableRange<T>(value, maxValue), IIntAddable, IAddable<T>
        where T : IConvertible, IComparable<T>, INumber<T>
    {
        private IIntAddable score = new AtomicInt(0);
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

        public TA ScoreAdd<TA>(Func<TA> x)
        {
            lock (vLock)
            {
                T previousV = v;
                TA ans = x();
                if (v > previousV)
                    Score.Add((int)((v - previousV).ToDouble(null) * speed));
                return ans;
            }
        }
        public void ScoreAdd(Action x)
        {
            lock (vLock)
            {
                T previousV = v;
                x();
                if (v > previousV)
                    Score.Add((int)((v - previousV).ToDouble(null) * speed));
            }
        }

        public override bool SetMaxV(T maxValue)
        {
            return ScoreAdd<bool>(() => base.SetMaxV(maxValue));
        }

        public override void SetPositiveMaxV(T maxValue)
        {
            ScoreAdd(() => base.SetPositiveMaxV(maxValue));
        }

        public override T SetRNow(T value)
        {
            return ScoreAdd<T>(() => base.SetRNow(value));
        }

        public override T SetPositiveVRNow(T value)
        {
            return ScoreAdd<T>(() => base.SetPositiveVRNow(value));
        }

        public override bool TrySetMaxV(T maxValue)
        {
            return ScoreAdd<bool>(() => base.TrySetMaxV(maxValue));
        }

        public override void SetVToMaxV()
        {
            ScoreAdd(() => base.SetVToMaxV());
        }

        public override void SetVToMaxV(double ratio)
        {
            ScoreAdd(() => base.SetVToMaxV(ratio));
        }

        public override bool Set0IfNotMaxor0()
        {
            return ScoreAdd<bool>(() => base.Set0IfNotMaxor0());
        }

        public override bool Set0IfMax()
        {
            return ScoreAdd<bool>(() => base.Set0IfMax());
        }

        public override void Add(T addV)
        {
            ScoreAdd(() => base.Add(addV));
        }

        public override void Add(int addV)
        {
            ScoreAdd(() => base.Add(addV));
        }

        public override T AddRNow(T addV)
        {
            return ScoreAdd(() => base.AddRNow(addV));
        }

        public override T AddRChange(T addV)
        {
            return ScoreAdd<T>(() => base.AddRChange(addV));
        }

        public override T AddPositiveVRChange(T addPositiveV)
        {
            return ScoreAdd<T>(() => base.AddPositiveVRChange(addPositiveV));
        }

        public override void AddPositiveV(T addPositiveV)
        {
            ScoreAdd(() => base.AddPositiveV(addPositiveV));
        }

        public override void Mul(T mulV)
        {
            ScoreAdd(() => base.Mul(mulV));
        }

        public override void Mul<TA>(TA mulV)
        {
            ScoreAdd(() => base.Mul(mulV));
        }

        public override void MulPositiveV(T mulPositiveV)
        {
            ScoreAdd(() => base.MulPositiveV(mulPositiveV));
        }

        public override void MulPositiveV<TA>(TA mulV)
        {
            ScoreAdd(() => base.MulPositiveV(mulV));
        }

        public override T SubRChange(T subV)
        {
            return ScoreAdd<T>(() => base.SubRChange(subV));
        }

        public override T SubRNow(T subV)
        {
            return ScoreAdd<T>(() => base.SubRNow(subV));
        }

        public override T SubPositiveVRChange(T subPositiveV)
        {
            return ScoreAdd<T>(() => base.SubPositiveVRChange(subPositiveV));
        }

        public override void SubPositiveV(T subPositiveV)
        {
            ScoreAdd(() => base.SubPositiveV(subPositiveV));
        }

        public override T TryAddToMaxVRChange(T addV)
        {
            return ScoreAdd<T>(() => base.TryAddToMaxVRChange(addV));
        }

        public override T VAddPartMaxVRChange(double ratio)
        {
            return ScoreAdd<T>(() => base.VAddPartMaxVRChange(ratio));
        }

        public override T AddRChange<TA>(InVariableRange<TA> a, double speed = 1.0)
        {
            return ScoreAdd<T>(() => base.AddRChange(a, speed));
        }

        public override T AddVUseOtherRChange<TA>(T value, InVariableRange<TA> other, double speed = 1.0)
        {
            return ScoreAdd<T>(() => base.AddVUseOtherRChange(value, other, speed));
        }

        public override T SubRChange<TA>(InVariableRange<TA> a)
        {
            return ScoreAdd<T>(() => base.SubRChange(a));
        }

        public override (T, T, long) TryAddToMaxV(StartTime startTime, double speed = 1.0)
        {
            return ScoreAdd<(T, T, long)>(() => base.TryAddToMaxV(startTime, speed));
        }

        public override T AddRChange(StartTime startTime, double speed = 1.0)
        {
            return ScoreAdd<T>(() => base.AddRChange(startTime, speed));
        }

        public override bool Set0IfNotAddToMaxV(StartTime startTime, double speed = 1.0)
        {
            return ScoreAdd<bool>(() => base.Set0IfNotAddToMaxV(startTime, speed));
        }

    }
}
