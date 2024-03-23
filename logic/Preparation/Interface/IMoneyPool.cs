using Preparation.Utility;

namespace Preparation.Interface
{
    public interface IMoneyPool
    {
        public AtomicLongOnlyAddScore Money { get; }
        public AtomicLong Score { get; }
        public long AddMoney(long add);
        public long SubMoney(long sub);
    }
}
