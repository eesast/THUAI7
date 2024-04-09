using System;
using System.Threading;

namespace Preparation.Utility
{
    public class LockedDouble(double x) : LockedValue, IDouble
    {
        private double v = x;

        public override string ToString()
        {
            lock (vLock)
                return v.ToString();
        }
        public double Get()
        {
            lock (vLock)
                return v;
        }
        public double ToDouble() => Get();
        public static implicit operator double(LockedDouble adouble) => adouble.Get();

        public void Set(double value)
        {
            lock (vLock)
                v = value;
        }
        public double AddRNow(double value)
        {
            lock (vLock)
                return v += value;
        }
        public double MulRNow(double value)
        {
            lock (vLock)
                return v *= value;
        }
    }
}
