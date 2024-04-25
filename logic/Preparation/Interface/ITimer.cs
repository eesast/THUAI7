using Preparation.Utility.Value.SafeValue.Atomic;

namespace Preparation.Interface
{
    public interface ITimer
    {
        AtomicBool IsGaming { get; }
        public int nowTime();
        public bool StartGame(int timeInMilliseconds);
    }
}
