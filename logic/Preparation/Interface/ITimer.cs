using Preparation.Utility;

namespace Preparation.Interface
{
    public interface ITimer
    {
        AtomicBool IsGaming { get; }
        public int nowTime();
        public bool StartGame(int timeInMilliseconds);
    }
}
