using Preparation.Utility;

namespace Preparation.Interface
{
    public interface IMoneyPool
    {
        public AtomicLong Money { get; }
        public AtomicLong Score { get; }
        public long AddMoney(long add);
        public void SubMoney(long sub);
    }
}
