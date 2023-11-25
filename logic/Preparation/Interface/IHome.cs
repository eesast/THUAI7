using Preparation.Utility;

namespace Preparation.Interface
{
    public interface IHome
    {
        public long TeamID { get; }
        public LongInTheVariableRange HP { get; }
        public AtomicLong Score { get; }
        public void AddScore(long add);
    }
}
