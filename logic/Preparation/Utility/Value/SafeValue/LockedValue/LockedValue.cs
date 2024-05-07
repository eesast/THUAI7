using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace Preparation.Utility.Value.SafeValue.LockedValue
{
    public abstract class LockedValue
    {
        protected readonly object vLock = new();
        public object VLock => vLock;

        #region NeedToDo
        public virtual TResult ReadNeed<TResult>(Func<TResult> func)
        {
            lock (vLock)
            {
                return func();
            }
        }
        public virtual void ReadNeed(Action func)
        {
            lock (vLock)
            {
                func();
            }
        }

        public virtual TResult WriteNeed<TResult>(Func<TResult> func)
        {
            lock (vLock)
            {
                return func();
            }
        }
        public virtual void WriteNeed(Action func)
        {
            lock (vLock)
            {
                func();
            }
        }
        #endregion

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
            if (idInClass == a.idInClass) return default;
            bool thisLock = false;
            bool thatLock = false;
            try
            {
                if (idInClass < a.idInClass)
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
            if (idInClass == a.idInClass) return;
            bool thisLock = false;
            bool thatLock = false;
            try
            {
                if (idInClass < a.idInClass)
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

        public static TResult? EnterLocks<TResult>(List<LockedValue> a, Func<TResult?> func)
        {
            bool[] locks = Enumerable.Repeat(false, a.Count).ToArray();
            try
            {
                a.Sort(delegate (LockedValue x, LockedValue y)
                {
                    if (x.IdInClass == y.IdInClass) return 0;
                    else return x.IdInClass < y.IdInClass ? -1 : 1;
                });
                for (int i = 0; i <= a.Count; ++i)
                    Monitor.Enter(a[i].VLock, ref locks[i]);
                return func();
            }
            finally
            {
                for (int i = 0; i <= a.Count; ++i)
                    if (locks[i]) Monitor.Exit(a[i].VLock);
            }
        }
    }
}