using System;
using System.Collections.Generic;

namespace Preparation.Interface;

public interface IObjPool<T>
    where T : class
{
    public int Count { get; }
    public bool IsEmpty { get; }
    public int CountIdle { get; }

    public void Initiate<TDerive>(int num)
        where TDerive : class, T, new();
    public void Initiate<TDerive>(int num, Func<TDerive> func)
        where TDerive : class, T;

    public T? GetObj<TDerive>()
        where TDerive : class, T;
    public void ReturnObj(T obj);

    public void Append<TDerive>(TDerive obj)
        where TDerive : class, T;
    public void Append<TDerive>(IEnumerable<TDerive> objs)
        where TDerive : class, T;

    public int GetNum<TDerive>()
        where TDerive : class, T;
    public bool CheckEmpty<TDerive>()
        where TDerive : class, T;
    public int GetIdleNum<TDerive>()
        where TDerive : class, T;

    public List<TResult> Travel<TResult>(Func<T, TResult> func, bool _);    // 加参数避免二义性
    public List<TResult>? Travel<TDerive, TResult>(Func<TDerive, TResult> func)
        where TDerive : class, T;

    public T? Find(Func<T, bool> cond, bool _);    // 加参数避免二义性
    public TDerive? Find<TDerive>(Func<TDerive, bool> cond)
        where TDerive : class, T;

    public (List<TResult> results, T? target) Until<TResult>(Func<T, TResult> func, Func<T, bool> cond, bool _);    // 加参数避免二义性
    public (List<TResult>? results, TDerive? target) Until<TDerive, TResult>(Func<TDerive, TResult> func, Func<TDerive, bool> cond)
        where TDerive : class, T;

    public void Clear();
    public void Clear<TDerive>()
        where TDerive : class, T;
}
