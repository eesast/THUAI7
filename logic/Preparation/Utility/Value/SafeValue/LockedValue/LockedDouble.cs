using System;
using System.Threading;

namespace Preparation.Utility
{
    public class LockedDouble(double x) : LockedValue, IDouble
    {
        private double v = x;

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
