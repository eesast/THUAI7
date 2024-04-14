using System;
using System.Numerics;
using System.Threading;

namespace Preparation.Utility
{
    /// <summary>
    /// 一个保证大于0的可变值
    /// 建议使用类似[0,int.MaxValue]的InVariableRange
    /// 其对应属性不应当有set访问器，避免不安全的=赋值
    /// </summa>
    public class PositiveValue<T> : LockedValue, IIntAddable, IAddable<T>
        where T : IConvertible, IComparable<T>, INumber<T>
    {
        protected T v;
        #region 构造与读取
        public PositiveValue(T value) : base()
        {
            if (value < T.Zero)
            {
                Debugger.Output("Warning:Try to set PositiveValue to " + value.ToString() + ".");
                value = T.Zero;
            }
            v = value;
        }
        public PositiveValue() : base()
        {
            v = T.Zero;
        }

        public override string ToString()
        {
            return ReadNeed(() => "value:" + v.ToString());
        }
        public T Get() { return ReadNeed(() => v); }
        public static implicit operator T(PositiveValue<T> aint) => aint.Get();
        public bool IsZero() { return ReadNeed(() => v == T.Zero); }
        #endregion

        #region 内嵌读取（在锁的情况下读取内容同时读取其他更基本的外部数据）
        public (T, long) GetValue(StartTime startTime)
        {
            return ReadNeed(() => (v, startTime.Get()));
        }
        #endregion

        #region 普通设置与计算
        public T SetRNow(T value)
        {
            if (value < T.Zero)
            {
                return WriteNeed(() => v = T.Zero);
            }
            return WriteNeed(() => v = value);
        }
        /// <summary>
        /// 应当保证该value>=0
        /// </summary>
        public T SetPositiveVRNow(T value)
        {
            return WriteNeed(() => v = value);
        }

        public void Add(T addV)
        {
            WriteNeed(() =>
            {
                v += addV;
                if (v < T.Zero) v = T.Zero;
            });
        }

        public void Add(int addV)
        {
            WriteNeed(() =>
            {
                v += T.CreateChecked(addV);
                if (v < T.Zero) v = T.Zero;
            });
        }

        public void Add<TA>(TA addV) where TA : IConvertible, INumber<TA>
        {
            WriteNeed(() =>
            {
                v += T.CreateChecked(addV);
                if (v < T.Zero) v = T.Zero;
            });
        }

        /// <returns>返回实际改变量</returns>
        public T AddRChange(T addV)
        {
            return WriteNeed(() =>
            {
                T previousV = v;
                v += addV;
                if (v < T.Zero) v = T.Zero;
                return v - previousV;
            });
        }
        /// <summary>
        /// 应当保证增加值大于0
        /// </summary>
        /// <returns>返回实际改变量</returns>
        public T AddPositiveVRChange(T addPositiveV)
        {
            WriteNeed(() =>
            {
                v += addPositiveV;
            });
            return addPositiveV;
        }
        public void Mul(T mulV)
        {
            if (mulV.CompareTo(0) <= 0)
            {
                WriteNeed(() => v = T.Zero);
                return;
            }
            WriteNeed(() =>
            {
                v *= mulV;
            });
        }
        public void Mul<TA>(TA mulV) where TA : IConvertible, INumber<TA>
        {
            if (mulV < TA.Zero)
            {
                WriteNeed(() => v = T.Zero); ;
                return;
            }
            WriteNeed(() =>
            {
                v = T.CreateChecked(v.ToDouble(null) * mulV.ToDouble(null));
            });
        }
        /// <summary>
        /// 应当保证乘数大于0
        /// </summary>
        public void MulPositiveV(T mulPositiveV)
        {
            WriteNeed(() =>
            {
                v *= mulPositiveV;
            });
        }

        /// <returns>返回实际改变量</returns>
        public T SubRChange(T subV)
        {
            WriteNeed(() =>
            {
                subV = (subV.CompareTo(v) > 0) ? v : subV;
                v -= subV;
            });
            return subV;
        }
        #endregion

        #region 特殊条件的设置和运算
        public bool Set0IfNot0()
        {
            return WriteNeed(() =>
            {
                if (v.CompareTo(0) > 0)
                {
                    v = T.Zero;
                    return true;
                }
                return false;
            });
        }
        #endregion

        #region 与LockedValue类的运算，运算会影响该对象的值
        public T AddRChange<TA>(InVariableRange<TA> a) where TA : IConvertible, IComparable<TA>, INumber<TA>
        {
            return EnterOtherLock<T>(a, () => WriteNeed(() =>
            {
                T previousV = v;
                v += T.CreateChecked(a.GetValue());
                a.SubPositiveVRChange(TA.CreateChecked(v - previousV));
                return v - previousV;
            }))!;
        }
        public T SubRChange<TA>(InVariableRange<TA> a) where TA : IConvertible, IComparable<TA>, INumber<TA>
        {
            return EnterOtherLock<T>(a, () => WriteNeed(() =>
            {
                T previousV = v;
                v -= T.CreateChecked(a.GetValue());
                if (v < T.Zero) v = T.Zero;
                a.SubPositiveVRChange(TA.CreateChecked(previousV - v));
                return v - previousV;
            }))!;
        }
        #endregion

        #region 与StartTime类的特殊条件的运算，运算会影响StartTime类的值
        /// <summary>
        /// 增加量为时间差*速度，并将startTime变为long.MaxValue
        /// </summary>
        /// <returns>返回实际改变量</returns>
        public T AddV(StartTime startTime, double speed = 1.0)
        {
            return WriteNeed(() =>
            {
                T previousV = v;
                T addV = T.CreateChecked((Environment.TickCount64 - startTime.Stop()) * speed);
                if (addV.CompareTo(T.Zero) <= 0) return T.Zero;
                else v += addV;
                return v - previousV;
            });
        }
        #endregion
    }
}