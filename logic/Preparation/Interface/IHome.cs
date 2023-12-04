using Preparation.Utility;

namespace Preparation.Interface
{
    public interface IHome
    {
        public AtomicLong TeamID { get; }
        public LongInTheVariableRange HP { get; }
    }
}
