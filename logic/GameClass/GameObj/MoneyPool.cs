using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj;

public class MoneyPool : IMoneyPool
{
    public AtomicLongOnlyAddScore Money { get; } = new(0);
    public AtomicLong Score { get; } = new AtomicLong(0);
    public MoneyPool()
    {
        Money.SetScore(Score);
    }
    public long AddMoney(long add)
    {
        return Money.Add(add);
    }
    public long SubMoney(long sub)
    {
        return Money.Sub(sub);
    }
}