using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj;

public class MoneyPool : IMoneyPool
{
    public AtomicLong Money { get; } = new AtomicLong(0);
    public AtomicLong Score { get; } = new AtomicLong(0);
    public long AddMoney(long add)
    {
        Money.Add(add);
        Score.Add(add);
        return add;
    }
    public void SubMoney(long sub)
    {
        Money.Sub(sub);
    }
}