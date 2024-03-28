using System;
using System.Collections.Generic;

namespace Preparation.Interface;

public interface IObjPool<T, TType>
    where T : class
    where TType : notnull
{
    public int Size { get; }
    public bool IsEmpty { get; }
    public int IdleNum { get; }

    public void Initiate(TType tp, int num, Func<T> func);

    public T? GetObj(TType tp);
    public void ReturnObj(T obj);

    public void Append(T obj);
    public void Append(IEnumerable<T> objs);
    public void AppendNoCheck(TType tp, IEnumerable<T> objs);

    public int GetNum(TType tp);
    public bool CheckEmpty(TType tp);
    public int GetIdleNum(TType tp);

    public List<TResult> Travel<TResult>(Func<T, TResult> func);
    public void Travel(Action<T> func);
    public List<TResult>? Travel<TResult>(TType tp, Func<T, TResult> func);
    public void Travel(TType tp, Action<T> func);

    public T? Find(Func<T, bool> cond);
    public T? Find(TType tp, Func<T, bool> cond);

    public (List<TResult> results, T? target) Until<TResult>(Func<T, TResult> func, Func<T, bool> cond);
    public T? Until(Action<T> func, Func<T, bool> cond);
    public (List<TResult>? results, T? target) Until<TResult>(TType tp, Func<T, TResult> func, Func<T, bool> cond);
    public T? Until(TType tp, Action<T> func, Func<T, bool> cond);

    public void Clear();
    public void Clear(TType tp);
}
