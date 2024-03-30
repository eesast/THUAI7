using Preparation.Utility;

namespace Preparation.Interface
{
    public interface ITimer
    {
        AtomicBool IsGaming { get; }
        public int NowTime();
        public bool StartGame(int timeInMilliseconds);
    }
}
