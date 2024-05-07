using Preparation.Utility.Value.SafeValue.Atomic;

namespace Preparation.Interface
{
    public interface IPlayer
    {
        public AtomicLong TeamID { get; }
        public AtomicLong PlayerID { get; }
    }
}
