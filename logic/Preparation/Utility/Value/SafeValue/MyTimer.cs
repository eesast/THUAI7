using System;
using System.Threading;

namespace Preparation.Utility
{
    public class MyTimer : IMyTimer
    {
        private readonly AtomicLong startTime = new(long.MaxValue);
        public int NowTime() => (int)(Environment.TickCount64 - startTime);
        public bool IsGaming => startTime != long.MaxValue;

        public bool Start(Action start, Action end, int timeInMilliseconds)
        {
            start();
            if (startTime.CompareExROri(Environment.TickCount64, long.MaxValue) != long.MaxValue)
                return false;
            new Thread
              (
                () =>
                {
                    Thread.Sleep(timeInMilliseconds);
                    startTime.SetROri(long.MaxValue);
                    end();
                }
            )
            { IsBackground = true }.Start();
            return true;
        }
    }
}
