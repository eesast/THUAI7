using System;
using System.Numerics;

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
        public bool SetMaxV(T maxValue)
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
        public void SetPositiveMaxV(T maxValue)
        {
            lock (vLock)
            {
                maxV = maxValue;
                if (v > maxValue) v = maxValue;
            }
        }

        public T SetRNow(T value)
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
        public T SetPositiveVRNow(T value)
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
        public bool TrySetMaxV(T maxValue)
        {
            lock (vLock)
            {
                if (v > maxValue) return false;
                maxV = maxValue;
                return true;
            }
        }
        public void SetVToMaxV()
        {
            lock (vLock)
            {
                v = maxV;
            }
        }
        public void SetVToMaxV(double ratio)
        {
            lock (vLock)
            {
                v = T.CreateChecked(maxV.ToDouble(null) * ratio);
            }
        }
        public bool Set0IfNotMaxor0()
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
        public bool Set0IfMax()
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
        public void Add(T addV)
        {
            lock (vLock)
            {
                v += addV;
                if (v < T.Zero) v = T.Zero;
                if (v > maxV) v = maxV;
            }
        }
        public void Add(int addV)
        {
            lock (vLock)
            {
                v += T.CreateChecked(addV);
                if (v < T.Zero) v = T.Zero;
                if (v > maxV) v = maxV;
            }
        }

        /// <returns>返回实际改变量</returns>
        public T AddRChange(T addV)
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
        public T AddPositiveVRChange(T addPositiveV)
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
        public void AddPositiveV(T addPositiveV)
        {
            lock (vLock)
            {
                v += addPositiveV;
                if (v > maxV) v = maxV;
            }
        }

        public void Mul(T mulV)
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
        public void Mul<TA>(TA mulV) where TA : IConvertible, INumber<TA>
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
        public void MulPositiveV(T mulPositiveV)
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
        public void MulPositiveV<TA>(TA mulV) where TA : IConvertible, INumber<TA>
        {
            lock (vLock)
            {
                if (v > T.CreateChecked(maxV.ToDouble(null) / mulV.ToDouble(null))) v = maxV; //避免溢出
                else
                    v = T.CreateChecked(v.ToDouble(null) * mulV.ToDouble(null));
            }
        }
        /// <returns>返回实际改变量</returns>
        public T SubRChange(T subV)
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
        /// <summary>
        /// 应当保证该减少值大于0
        /// </summary>
        /// <returns>返回实际改变量</returns>
        public T SubPositiveVRChange(T subPositiveV)
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
        public void SubPositiveV(T subPositiveV)
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
        public T TryAddToMaxVRChange(T addV)
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
        public T VAddPartMaxVRChange(double ratio)
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
        public T AddRChange<TA>(InVariableRange<TA> a) where TA : IConvertible, IComparable<TA>, INumber<TA>
        {
            return EnterOtherLock<T>(a, () =>
            {
                T previousV = v;
                v += T.CreateChecked(a.GetValue());
                if (v > maxV) v = maxV;
                a.SubPositiveVRChange(TA.CreateChecked(v - previousV));
                return v - previousV;
            })!;
        }
        public T SubRChange<TA>(InVariableRange<TA> a) where TA : IConvertible, IComparable<TA>, IComparable<int>, INumber<TA>
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
        public (T, T, long) TryAddToMaxV(StartTime startTime, double speed = 1.0)
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
        public T AddRChange(StartTime startTime, double speed = 1.0)
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
        public bool Set0IfNotAddToMaxV(StartTime startTime, double speed = 1.0)
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

}