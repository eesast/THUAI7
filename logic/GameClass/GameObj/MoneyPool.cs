using Preparation.Interface;
using Preparation.Utility.Value.SafeValue.Atomic;
using Preparation.Utility.Value.SafeValue.LockedValue;

namespace GameClass.GameObj;

public class MoneyPool : IMoneyPool
{
    public InVariableRangeOnlyAddScore<long> Money { get; } = new(0, long.MaxValue);
    public AtomicLong Score { get; } = new AtomicLong(0);
    public MoneyPool()
    {
        Money.Score = Score;
    }
    public long AddMoney(long add)
    {
        return Money.AddRNow(add);
    }
    public long SubMoney(long sub)
    {
        return Money.SubRNow(sub);
    }
}