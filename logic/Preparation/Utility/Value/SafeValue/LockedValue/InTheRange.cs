using Preparation.Interface;
using System;
using System.Numerics;
using System.Threading;
using Preparation.Utility.Value.SafeValue.Atomic;
using Preparation.Utility.Value.SafeValue.TimeBased;

namespace Preparation.Utility.Value.SafeValue.LockedValue
{
    //其对应属性不应当有set访问器，避免不安全的=赋值

    /// <summary>
    /// 一个保证在[0,maxValue]的可变值，支持可变的maxValue（请确保大于0）
    /// </summary>
    public class InVariableRange<T> : LockedValue, IIntAddable, IAddable<T>, IDouble, IDoubleAddable, IConvertible
        where T : IConvertible, IComparable<T>, INumber<T>
    {
        protected T v;
        protected T maxV;

        #region 实现IConvertible接口
        public TypeCode GetTypeCode()
        {
            return ReadNeed(() => v.GetTypeCode());
        }

        public bool ToBoolean(IFormatProvider? provider)
        {
            return ReadNeed(() => v.ToBoolean(provider));
        }
        public byte ToByte(IFormatProvider? provider)
        {
            return ReadNeed(() => v.ToByte(provider));
        }

        public char ToChar(IFormatProvider? provider)
        {
            return ReadNeed(() => v.ToChar(provider));
        }

        public DateTime ToDateTime(IFormatProvider? provider)
        {
            return ReadNeed(() => v.ToDateTime(provider));
        }

        public decimal ToDecimal(IFormatProvider? provider)
        {
            return ReadNeed(() => v.ToDecimal(provider));
        }

        public double ToDouble(IFormatProvider? provider)
        {
            return ReadNeed(() => v.ToDouble(provider));
        }

        public short ToInt16(IFormatProvider? provider)
        {
            return ReadNeed(() => v.ToInt16(provider));
        }

        public int ToInt32(IFormatProvider? provider)
        {
            return ReadNeed(() => v.ToInt32(provider));
        }

        public long ToInt64(IFormatProvider? provider)
        {
            return ReadNeed(() => v.ToInt64(provider));
        }

        public sbyte ToSByte(IFormatProvider? provider)
        {
            return ReadNeed(() => v.ToSByte(provider));
        }

        public float ToSingle(IFormatProvider? provider)
        {
            return ReadNeed(() => v.ToSingle(provider));
        }

        public string ToString(IFormatProvider? provider)
        {
            return ReadNeed(() => v.ToString(provider));
        }

        public object ToType(Type conversionType, IFormatProvider? provider)
        {
            return ReadNeed(() => v.ToType(conversionType, provider));
        }

        public ushort ToUInt16(IFormatProvider? provider)
        {
            return ReadNeed(() => v.ToUInt16(provider));
        }

        public uint ToUInt32(IFormatProvider? provider)
        {
            return ReadNeed(() => v.ToUInt32(provider));
        }

        public ulong ToUInt64(IFormatProvider? provider)
        {
            return ReadNeed(() => v.ToUInt64(provider));
        }
        #endregion

        #region 构造与读取
        public InVariableRange(T value, T maxValue) : base()
        {
            if (value < T.Zero)
            {
                LockedValueLogging.logger.ConsoleLogDebug(
                    $"Warning: Try to set IntInTheVariableRange to {value}");
                value = T.Zero;
            }
            if (maxValue < T.Zero)
            {
                LockedValueLogging.logger.ConsoleLogDebug(
                    $"Warning: Try to set IntInTheVariableRange.maxValue to {maxValue}");
                maxValue = T.Zero;
            }
            v = value.CompareTo(maxValue) < 0 ? value : maxValue;
            maxV = maxValue;
        }
        /// <summary>
        /// 默认使Value=maxValue
        /// </summary>
        public InVariableRange(T maxValue) : base()
        {
            if (maxValue < T.Zero)
            {
                LockedValueLogging.logger.ConsoleLogDebug(
                    $"Warning: Try to set IntInTheVariableRange.maxValue to {maxValue}");
                maxValue = T.Zero;
            }
            v = maxV = maxValue;
        }

        public override string ToString()
        {
            return ReadNeed(() => $"value: {v} , maxValue: {maxV}");
        }
        public T GetValue() { return ReadNeed(() => v); }
        public double ToDouble() => GetValue().ToDouble(null);
        public static implicit operator T(InVariableRange<T> aint) => aint.GetValue();
        public override bool Equals(object? obj)
        {
            return obj != null && (obj is IConvertible k) && ToDouble(null) == k.ToDouble(null);
        }
        public override int GetHashCode()
        {
            return ReadNeed(() => v.GetHashCode() ^ maxV.GetHashCode());
        }

        public T GetMaxV()
        {
            return ReadNeed(() => maxV);
        }
        public (T, T) GetValueAndMaxV()
        {
            return ReadNeed(() => (v, maxV));
        }
        public T GetDifference() => ReadNeed(() => maxV - v);
        public double GetDivideValueByMaxV()
        {
            return ReadNeed(() => v.ToDouble(null) / maxV.ToDouble(null));
        }
        #endregion

        #region 特别的读取
        public bool IsMaxV()
        {
            return ReadNeed(() => v == maxV);
        }
        public bool IsBelowMaxTimes(double a) => ReadNeed(() => v.ToDouble(null) < maxV.ToDouble(null) * a);
        #endregion

        #region 内嵌读取（在锁的情况下读取内容同时读取其他更基本的外部数据）
        public (T, long) GetValue(StartTime startTime)
        {
            return ReadNeed(() => (v, startTime.Get()));
        }

        public (T, T, long) GetValueAndMaxValue(StartTime startTime)
        {
            return ReadNeed(() => (v, maxV, startTime.Get()));
        }
        #endregion

        #region 普通设置MaxV与Value的值的方法
        /// <summary>
        /// 若maxValue<=0则maxValue设为0并返回False
        /// </summary>
        public bool SetMaxV(T maxValue)
        {
            if (maxValue <= T.Zero)
            {
                return WriteNeed(() =>
                {
                    v = maxV = T.Zero;
                    return false;
                });
            }
            else
            {
                return WriteNeed(() =>
                {
                    maxV = maxValue;
                    if (v > maxValue) v = maxValue;
                    return true;
                });
            }
        }

        /// <summary>
        /// 应当保证该maxValue>=0
        /// </summary>
        public void SetPositiveMaxV(T maxValue)
        {
            WriteNeed(() =>
            {
                maxV = maxValue;
                if (v > maxV) v = maxV;
            });
        }

        public T SetRNow(T value)
        {
            if (value < T.Zero)
            {
                return WriteNeed(() => v = T.Zero);
            }
            else
            {
                return WriteNeed(() => v = value > maxV ? maxV : value);
            }
        }

        public void Set(double value)
        {
            if (value < 0)
            {
                WriteNeed(() => v = T.Zero);
            }
            T va = T.CreateChecked(value);
            WriteNeed(() => v = va > maxV ? maxV : va);
        }

        /// <summary>
        /// 应当保证该value>=0
        /// </summary>
        public T SetPositiveVRNow(T value)
        {
            return WriteNeed(() => v = value > maxV ? maxV : value);
        }
        #endregion

        #region 特殊条件的设置MaxV与Value的值的方法
        /// <summary>
        /// 如果当前值大于试图更新的maxValue,则更新maxValue失败
        /// </summary>
        public bool TrySetMaxV(T maxValue)
        {
            return WriteNeed(() =>
            {
                if (v > maxValue) return false;
                maxV = maxValue;
                return true;
            });
        }

        public void SetVToMaxV()
        {
            WriteNeed(() =>
            {
                v = maxV;
            });
        }

        public void SetVToMaxV(double ratio)
        {
            WriteNeed(() =>
                v = T.CreateChecked(maxV.ToDouble(null) * ratio)
            );
        }

        public bool Set0IfNotMaxor0()
        {
            return WriteNeed(() =>
            {
                if (v < maxV && v > T.Zero)
                {
                    v = T.Zero;
                    return true;
                }
                return false;
            });
        }
        public bool Set0IfMax()
        {
            return WriteNeed(() =>
            {
                if (v == maxV)
                {
                    v = T.Zero;
                    return true;
                }
                return false;
            });
        }
        #endregion

        #region 普通运算
        public void Add(T addV)
        {
            WriteNeed(() =>
            {
                v += addV;
                if (v < T.Zero) v = T.Zero;
                if (v > maxV) v = maxV;
            });
        }

        public void Add(int addV)
        {
            WriteNeed(() =>
            {
                v += T.CreateChecked(addV);
                if (v < T.Zero) v = T.Zero;
                if (v > maxV) v = maxV;
            });
        }

        public void Add(double addV)
        {
            WriteNeed(() =>
            {
                v += T.CreateChecked(addV);
                if (v < T.Zero) v = T.Zero;
                if (v > maxV) v = maxV;
            });
        }

        public T AddRNow(T addV)
        {
            return WriteNeed(() =>
            {
                v += addV;
                if (v < T.Zero) v = T.Zero;
                if (v > maxV) v = maxV;
                return v;
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
                if (v > maxV) v = maxV;
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
                addPositiveV = addPositiveV < maxV - v ? addPositiveV : maxV - v;
                v += addPositiveV;
            });
            return addPositiveV;
        }
        /// <summary>
        /// 应当保证增加值大于0
        /// </summary>
        public void AddPositiveV(T addPositiveV)
        {
            WriteNeed(() =>
            {
                v += addPositiveV;
                if (v > maxV) v = maxV;
            });
        }

        public void Mul(T mulV)
        {
            if (mulV <= T.Zero)
            {
                WriteNeed(() => v = T.Zero);
                return;
            }
            WriteNeed(() =>
            {
                if (v > maxV / mulV) v = maxV; //避免溢出
                else v *= mulV;
            });
        }
        public void Mul<TA>(TA mulV) where TA : IConvertible, INumber<TA>
        {
            if (mulV < TA.Zero)
            {
                WriteNeed(() => v = T.Zero);
                return;
            }
            WriteNeed(() =>
            {
                if (v > T.CreateChecked(maxV.ToDouble(null) / mulV.ToDouble(null))) v = maxV; //避免溢出
                else
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
                if (v > maxV / mulPositiveV) v = maxV; //避免溢出
                else v *= mulPositiveV;
            });
        }
        /// <summary>
        /// 应当保证乘数大于0
        /// </summary>
        public void MulPositiveV<TA>(TA mulV) where TA : IConvertible, INumber<TA>
        {
            WriteNeed(() =>
            {
                if (v > T.CreateChecked(maxV.ToDouble(null) / mulV.ToDouble(null))) v = maxV; // Avoid overflow
                else
                    v = T.CreateChecked(v.ToDouble(null) * mulV.ToDouble(null));
            });
        }

        /// <returns>返回实际改变量</returns>
        public T SubRChange(T subV)
        {
            return WriteNeed(() =>
            {
                T previousV = v;
                v -= subV;
                if (v < T.Zero) v = T.Zero;
                if (v > maxV) v = maxV;
                return v - previousV;
            });
        }

        public T SubRNow(T subV)
        {
            return WriteNeed(() =>
            {
                v -= subV;
                if (v < T.Zero) v = T.Zero;
                if (v > maxV) v = maxV;
                return v;
            });
        }

        /// <summary>
        /// 应当保证该减少值大于0
        /// </summary>
        /// <returns>返回实际改变量</returns>
        public T SubPositiveVRChange(T subPositiveV)
        {
            WriteNeed(() =>
            {
                subPositiveV = subPositiveV < v ? subPositiveV : v;
                v -= subPositiveV;
            });
            return subPositiveV;
        }
        /// <summary>
        /// 应当保证该减少值大于0
        /// </summary>
        public void SubPositiveV(T subPositiveV)
        {
            WriteNeed(() =>
            {
                v = subPositiveV < v ? v - subPositiveV : T.Zero;
            });
        }
        #endregion

        #region 特殊条件的运算
        /// <summary>
        /// 试图加到满，如果无法加到maxValue则不加并返回-1
        /// </summary>
        /// <returns>返回实际改变量</returns>
        public T TryAddToMaxVRChange(T addV)
        {
            return WriteNeed(() =>
            {
                if (maxV - v <= addV)
                {
                    addV = maxV - v;
                    v = maxV;
                    return addV;
                }
                return -T.One;
            });
        }

        /// <summary>
        /// ratio可以为负
        /// </summary>
        /// <returns>返回实际改变量</returns>
        public T VAddPartMaxVRChange(double ratio)
        {
            return WriteNeed(() =>
            {
                T preV = v;
                v += T.CreateChecked(ratio * maxV.ToDouble(null));
                if (v < T.Zero) v = T.Zero;
                if (v > maxV) v = maxV;
                return v - preV;
            });
        }
        #endregion

        #region 与InVariableRange类的运算，运算会影响该对象的值
        public T AddRChange<TA>(InVariableRange<TA> a, double speed = 1.0)
            where TA : IConvertible, IComparable<TA>, INumber<TA>
        {
            return EnterOtherLock<T>(a, () => WriteNeed(() =>
            {
                T previousV = v;
                v += T.CreateChecked(a.GetValue().ToDouble(null) * speed);
                if (v > maxV) v = maxV;
                a.SubPositiveVRChange(TA.CreateChecked(v - previousV));
                return v - previousV;
            }))!;
        }
        public T AddVUseOtherRChange<TA>(T value, InVariableRange<TA> other, double speed = 1.0)
            where TA : IConvertible, IComparable<TA>, INumber<TA>
        {
            return EnterOtherLock<T>(other, () => WriteNeed(() =>
            {
                T previousV = v;
                T otherValue = T.CreateChecked(other.GetValue().ToDouble(null) * speed);
                value = value > otherValue ? otherValue : value;
                v += value;
                if (v > maxV) v = maxV;
                other.SubPositiveVRChange(TA.CreateChecked((v - previousV).ToDouble(null) / speed));
                return v - previousV;
            }))!;
        }
        public T SubVLimitedByAddingOtherRChange<TA>(T value, InVariableRange<TA> other, double speed = 1.0)
            where TA : IConvertible, IComparable<TA>, INumber<TA>
        {
            return EnterOtherLock<T>(other, () => WriteNeed(() =>
            {
                T previousV = v;
                T otherValue = T.CreateChecked(other.GetDifference().ToDouble(null) * speed);
                value = value > otherValue ? otherValue : value;
                if (v < value)
                    return -T.One;
                v -= value;
                other.AddPositiveVRChange(TA.CreateChecked(value.ToDouble(null) / speed));
                return value;
            }))!;
        }
        public T SubRChange<TA>(InVariableRange<TA> a)
            where TA : IConvertible, IComparable<TA>, IComparable<int>, INumber<TA>
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
        /// 试图加到满，如果加上时间差*速度可以达到MaxV，则加上并使startTime变为long.MaxValue
        /// 如果无法加到maxValue则不加
        /// </summary>
        /// <returns>返回试图加到的值与最大值</returns>
        public (T, T, long) TryAddToMaxV(StartTime startTime, double speed = 1.0)
        {
            return WriteNeed(() =>
            {
                long addV = (long)(startTime.StopIfPassing((maxV - v).ToInt64(null)) * speed);
                if (addV < 0)
                    return (v, maxV, startTime.Get());
                if (maxV - v < T.CreateChecked(addV))
                    return (v = maxV, maxV, startTime.Get());
                return (v, maxV, startTime.Get());
            });
        }
        /// <summary>
        /// 增加量为时间差*速度，并将startTime变为long.MaxValue
        /// </summary>
        /// <returns>返回实际改变量</returns>
        public T AddRChange(StartTime startTime, double speed = 1.0)
        {
            return WriteNeed(() =>
            {
                T previousV = v;
                T addV = T.CreateChecked((Environment.TickCount64 - startTime.Stop()) * speed);
                if (addV <= T.Zero) return T.Zero;
                else v += addV;
                if (v > maxV) v = maxV;
                return v - previousV;
            });
        }

        /// <summary>
        /// 试图加到满，如果加上时间差*速度可以达到MaxV，则加上并使startTime变为long.MaxValue
        /// 如果无法加到maxValue则清零
        /// </summary>
        /// <returns>返回是否清零</returns>
        public bool Set0IfNotAddToMaxV(StartTime startTime, double speed = 1.0)
        {
            return WriteNeed(() =>
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
            });
        }
        #endregion
    }

    /// <summary>
    /// 可以设定IIntAddable类的Score，默认初始为0的AtomicInt
    /// 在发生正向的变化时，自动给Score加上正向变化的差乘以speed（取整）。
    /// </summary>
    public class InVariableRangeOnlyAddScore<T>(T value, T maxValue, double speed = 1.0) : InVariableRange<T>(value, maxValue)
        where T : IConvertible, IComparable<T>, INumber<T>
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


        public override TResult WriteNeed<TResult>(Func<TResult> func)
        {
            lock (vLock)
            {
                T previousV = v;
                TResult ans = func();
                if (v > previousV)
                    Score.Add((int)((v - previousV).ToDouble(null) * speed.ToDouble()));
                return ans;
            }
        }
        public override void WriteNeed(Action func)
        {
            lock (vLock)
            {
                T previousV = v;
                func();
                if (v > previousV)
                    Score.Add((int)((v - previousV).ToDouble(null) * speed.ToDouble()));
            }
        }
    }
}
