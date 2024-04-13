using Preparation.Interface;
using Preparation.Utility;

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
        return Money.AddRChange(add);
    }
    public long SubMoney(long sub)
    {
        return Money.SubRChange(sub);
    }
}