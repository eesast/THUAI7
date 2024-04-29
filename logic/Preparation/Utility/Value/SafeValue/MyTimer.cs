using Preparation.Utility.Value.SafeValue.Atomic;
using System;
using System.Threading;

namespace Preparation.Utility.Value.SafeValue
{
    public class MyTimer : IMyTimer
    {
        private readonly AtomicLong startTime = new(long.MaxValue);
        public int NowTime() => (int)(Environment.TickCount64 - startTime);
        public bool IsGaming => startTime != long.MaxValue;

        public bool Start(Action start, Action endBefore, Action endAfter, int timeInMilliseconds)
        {
            start();
            if (startTime.CompareExROri(Environment.TickCount64, long.MaxValue) != long.MaxValue)
                return false;
            try
            {
                new Thread
                  (
                    () =>
                    {
                        Thread.Sleep(timeInMilliseconds);
                        endBefore();
                        startTime.SetROri(long.MaxValue);
                        endAfter();
                    }
                )
                { IsBackground = true }.Start();
            }
            catch (Exception ex)
            {
                startTime.SetROri(long.MaxValue);
                MyTimerLogging.logger.ConsoleLog(ex.Message);
            }
            return true;
        }
        public bool Start(Action start, Action end, int timeInMilliseconds)
        {
            start();
            if (startTime.CompareExROri(Environment.TickCount64, long.MaxValue) != long.MaxValue)
                return false;
            try
            {
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
            }
            catch (Exception ex)
            {
                startTime.SetROri(long.MaxValue);
                MyTimerLogging.logger.ConsoleLog(ex.Message);
            }
            return true;
        }
    }
}
