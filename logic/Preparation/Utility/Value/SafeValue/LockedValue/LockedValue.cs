using System;
using System.Threading;

namespace Preparation.Utility
{
    public abstract class LockedValue
    {
        protected readonly object vLock = new();
        protected object VLock => vLock;
        private static int numOfClass = 0;
        public static int NumOfClass => numOfClass;
        private readonly int idInClass;
        public int IdInClass => idInClass;

        public LockedValue()
        {
            idInClass = Interlocked.Increment(ref numOfClass);
        }

        public TResult? EnterOtherLock<TResult>(LockedValue a, Func<TResult?> func)
        {
            if (this.idInClass == a.idInClass) return default(TResult?);
            bool thisLock = false;
            bool thatLock = false;
            try
            {
                if (this.idInClass < a.idInClass)
                {
                    Monitor.Enter(vLock, ref thisLock);
                    Monitor.Enter(a.VLock, ref thatLock);
                }
                else
                {
                    Monitor.Enter(a.VLock, ref thatLock);
                    Monitor.Enter(vLock, ref thisLock);
                }
                return func();
            }
            finally
            {
                if (thisLock) Monitor.Exit(vLock);
                if (thatLock) Monitor.Exit(a.VLock);
            }
        }
        public void EnterOtherLock(LockedValue a, Action func)
        {
            if (this.idInClass == a.idInClass) return;
            bool thisLock = false;
            bool thatLock = false;
            try
            {
                if (this.idInClass < a.idInClass)
                {
                    Monitor.Enter(vLock, ref thisLock);
                    Monitor.Enter(a.VLock, ref thatLock);
                }
                else
                {
                    Monitor.Enter(a.VLock, ref thatLock);
                    Monitor.Enter(vLock, ref thisLock);
                }
                func();
            }
            finally
            {
                if (thisLock) Monitor.Exit(vLock);
                if (thatLock) Monitor.Exit(a.VLock);
            }
        }
    }
}