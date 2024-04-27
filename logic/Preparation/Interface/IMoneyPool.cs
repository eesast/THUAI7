using Preparation.Utility.Value.SafeValue.Atomic;
using Preparation.Utility.Value.SafeValue.LockedValue;

namespace Preparation.Interface
{
    public interface IMoneyPool
    {
        public InVariableRangeOnlyAddScore<long> Money { get; }
        public AtomicLong Score { get; }
        public long AddMoney(long add);
        public long SubMoney(long sub);
    }
}
