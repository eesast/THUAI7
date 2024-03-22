using Preparation.Interface;
using System;
using System.Collections.Generic;

namespace Preparation.Utility;

public class ObjPool<T>(Func<T, bool> funcCheckIdle, Action<T> funcSwitchIdle) : IObjPool<T>
    where T : class
{
    private readonly object dictLock = new();
    private readonly Dictionary<Type, LockedClassList<T>> objs = [];
    private readonly Func<T, bool> checkIdle = funcCheckIdle;
    private readonly Action<T> switchIdle = funcSwitchIdle;

    public int Count
    {
        get
        {
            lock (dictLock)
            {
                int ret = 0;
                foreach (var ls in objs.Values)
                {
                    ret += ls.Count;
                }
                return ret;
            }
        }
    }
    public bool IsEmpty
    {
        get
        {
            lock (dictLock)
            {
                foreach (var ls in objs.Values)
                {
                    if (ls.Count != 0) return false;
                }
                return true;
            }
        }
    }
    public int CountIdle => Travel(checkIdle, true).Count;

    #region Initiate
    public void Initiate<TDerive>(int num)
        where TDerive : class, T, new()
        => Initiate(num, () => new TDerive());
    public void Initiate<TDerive>(int num, Func<TDerive> func)
        where TDerive : class, T
    {
        Type tp = typeof(TDerive);
        if (objs.ContainsKey(tp)) Clear<TDerive>();
        lock (dictLock)
        {
            objs[tp] = new(num);
            for (int i = 0; i < num; i++)
            {
                objs[tp].Add(func());
            }
        }
    }
    #endregion

    public T? GetObj<TDerive>()
        where TDerive : class, T
    {
        if (CheckEmpty<TDerive>() || GetIdleNum<TDerive>() == 0) return null;
        var ret = Find((TDerive o) => checkIdle(o));
        if (ret == null) return null;
        switchIdle(ret);
        return ret;
    }
    public void ReturnObj(T obj) => switchIdle(obj);

    #region Append
    public void Append<TDerive>(TDerive obj)
        where TDerive : class, T
    {
        Type tp = typeof(TDerive);
        if (!objs.TryGetValue(tp, out var ls))
        {
            LockedClassList<T> temp = new();
            temp.Add(obj);
            lock (dictLock) objs[tp] = temp;
        }
        else
        {
            lock (dictLock) ls.Add(obj);
        }
    }
    public void Append<TDerive>(IEnumerable<TDerive> objs)
        where TDerive : class, T
    {
        Type tp = typeof(TDerive);
        if (!this.objs.TryGetValue(tp, out var ls))
        {
            LockedClassList<T> temp = new();
            temp.AddRange(objs);
            lock (dictLock) this.objs[tp] = temp;
        }
        else
        {
            lock (dictLock) ls.AddRange(objs);
        }
    }
    #endregion

    public int GetNum<TDerive>()
        where TDerive : class, T
        => objs[typeof(TDerive)].Count;
    public bool CheckEmpty<TDerive>()
        where TDerive : class, T
        => GetNum<TDerive>() == 0;
    public int GetIdleNum<TDerive>()
        where TDerive : class, T
    {
        var ret = Travel((TDerive o) => checkIdle(o));
        return ret is not null ? ret.Count : 0;
    }

    #region Travel
    public List<TResult> Travel<TResult>(Func<T, TResult> func, bool _)
    {
        List<TResult> ret = [];
        lock (dictLock)
        {
            foreach (var ls in objs.Values)
            {
                var len = ls.Count;
                for (int i = 0; i < len; i++)
                {
                    ret.Add(func(ls[i]));
                }
            }
        }
        return ret;
    }
    public List<TResult>? Travel<TDerive, TResult>(Func<TDerive, TResult> func)
        where TDerive : class, T
    {
        Type tp = typeof(TDerive);
        if (!objs.TryGetValue(tp, out var ls)) return null;
        var len = ls.Count;
        List<TResult> ret = new(len);
        lock (dictLock)
        {
            for (int i = 0; i < len; i++)
            {
                ret.Add(func((TDerive)ls[i]));
            }
        }
        return ret;
    }
    #endregion

    #region Find
    public T? Find(Func<T, bool> cond, bool _)
    {
        lock (dictLock)
        {
            foreach (var ls in objs.Values)
            {
                var len = ls.Count;
                for (int i = 0; i < len; i++)
                {
                    if (cond(ls[i])) return ls[i];
                }
            }
            return null;
        }
    }
    public TDerive? Find<TDerive>(Func<TDerive, bool> cond)
        where TDerive : class, T
    {
        Type tp = typeof(TDerive);
        if (!objs.TryGetValue(tp, out var ls)) return null;
        var len = ls.Count;
        lock (dictLock)
        {
            for (int i = 0; i < len; i++)
            {
                if (cond((TDerive)ls[i])) return (TDerive)ls[i];
            }
        }
        return null;
    }
    #endregion

    #region Until
    public (List<TResult> results, T? target) Until<TResult>(Func<T, TResult> func, Func<T, bool> cond, bool _)
    {
        List<TResult> ret = [];
        T? retObj = null;
        lock (dictLock)
        {
            bool flag = false;
            foreach (var ls in objs.Values)
            {
                var len = ls.Count;
                for (int i = 0; i < len; i++)
                {
                    ret.Add(func(ls[i]));
                    if (cond(ls[i]))
                    {
                        retObj = ls[i];
                        flag = true;
                        break;
                    }
                }
                if (flag) break;
            }
        }
        return (ret, retObj);
    }
    public (List<TResult>? results, TDerive? target) Until<TDerive, TResult>(Func<TDerive, TResult> func, Func<TDerive, bool> cond)
        where TDerive : class, T
    {
        Type tp = typeof(TDerive);
        if (!objs.TryGetValue(tp, out var ls)) return (null, null);
        var len = ls.Count;
        List<TResult> ret = new(len);
        TDerive? retObj = null;
        lock (dictLock)
        {
            for (int i = 0; i < len; i++)
            {
                ret.Add(func((TDerive)ls[i]));
                if (cond((TDerive)ls[i]))
                {
                    retObj = (TDerive)ls[i];
                    break;
                }
            }
        }
        return (ret, retObj);
    }
    #endregion


    #region Clear
    public void Clear()
    {
        lock (dictLock)
        {
            foreach (var k in objs.Keys)
            {
                objs[k].Clear();
                objs.Remove(k);
            }
        }
    }
    public void Clear<TDerive>()
        where TDerive : class, T
    {
        Type tp = typeof(TDerive);
        if (!objs.TryGetValue(tp, out var ls)) return;
        lock (dictLock)
        {
            ls.Clear();
            objs.Remove(tp);
        }
    }
    #endregion
}

