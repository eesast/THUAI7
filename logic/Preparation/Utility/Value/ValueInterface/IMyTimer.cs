using System;

namespace Preparation.Utility
{
    public interface IMyTimer
    {
        bool IsGaming { get; }
        public int NowTime();
        public bool Start(Action start, Action end, int timeInMilliseconds);
    }
}
