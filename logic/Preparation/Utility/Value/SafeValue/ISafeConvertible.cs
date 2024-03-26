using System;
using System.Threading;

namespace Preparation.Utility
{
    public interface ISafeConvertible<T> where T : struct
    {
        public void Set(T value);
        public void Add(T value);
        public void Sub(T value);
    }
    /*
    Set (Return void)
    SetRNow
    SetRChange
    SetROri
     */
}