using Preparation.Utility;

namespace Preparation.Interface
{
    public interface IPlayer
    {
        public AtomicLong TeamID { get; }
        public AtomicLong PlayerID { get; }
    }
}
