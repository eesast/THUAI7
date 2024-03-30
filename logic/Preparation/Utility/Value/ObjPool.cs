using Preparation.Interface;
using System;
using System.Collections.Generic;

namespace Preparation.Utility;

public class ObjPool<T, TType>(Func<T, TType> classfier,
                               Func<T, bool> idleChecker,
                               Action<T> activator,
                               Action<T> inactivator)
    : IObjPool<T, TType>
    where T : class
    where TType : notnull
{
    private readonly object dictLock = new();
    private readonly Dictionary<TType, LockedClassList<T>> objs = [];
    private readonly Func<T, TType> classfier = classfier;
    private readonly Func<T, bool> idleChecker = idleChecker;
    private readonly Action<T> activator = activator;
    private readonly Action<T> inactivator = inactivator;

    #region 属性

    public int Size
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
    public int IdleNum
    {
        get
        {
            int sum = 0;
            Travel((o) => { if (idleChecker(o)) sum++; });
            return sum;
        }
    }

    #endregion

    #region 主功能

    public void Initiate(TType tp, int num, Func<T> func)
    {
        if (objs.ContainsKey(tp)) Clear(tp);
        lock (dictLock)
        {
            objs[tp] = new(num);
            for (int i = 0; i < num; i++)
            {
                var obj = func();
                inactivator(obj);
                objs[tp].Add(obj);
            }
        }
    }
    public T? GetObj(TType tp)
    {
        lock (dictLock)
        {
            if (CheckEmpty(tp) || GetIdleNum(tp) == 0) return null;
            var ret = Find(idleChecker);
            if (ret is null) return null;
            activator(ret);
            return ret;
        }
    }
    public void ReturnObj(T obj)
    {
        lock (dictLock) inactivator(obj);
    }

    #endregion

    #region Append

    public void Append(T obj)
    {
        TType tp = classfier(obj);
        if (!objs.TryGetValue(tp, out var ls))
        {
            LockedClassList<T> temp = [];
            temp.Add(obj);
            lock (dictLock) objs[tp] = temp;
        }
        else
        {
            lock (dictLock) ls.Add(obj);
        }
    }
    public void Append(IEnumerable<T> objs)
    {
        foreach (var obj in objs)
        {
            Append(obj);
        }
    }
    public void AppendNoCheck(TType tp, IEnumerable<T> objs)
    {
        if (!this.objs.TryGetValue(tp, out var ls))
        {
            LockedClassList<T> temp = [];
            temp.AddRange(objs);
            lock (dictLock) this.objs[tp] = temp;
        }
        else
        {
            lock (dictLock) ls.AddRange(objs);
        }
    }

    #endregion

    #region 子属性

    public int GetNum(TType tp)
        => objs[tp].Count;
    public bool CheckEmpty(TType tp)
        => GetNum(tp) == 0;
    public int GetIdleNum(TType tp)
    {
        int sum = 0;
        Travel(tp, (o) => { if (idleChecker(o)) sum++; });
        return sum;
    }

    #endregion

    #region 遍历

    #region Travel

    public List<TResult> Travel<TResult>(Func<T, TResult> func)
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
    public void Travel(Action<T> func)
    {
        lock (dictLock)
        {
            foreach (var ls in objs.Values)
            {
                foreach (var obj in ls)
                {
                    func(obj);
                }
            }
        }
    }
    public List<TResult>? Travel<TResult>(TType tp, Func<T, TResult> func)
    {
        if (!objs.TryGetValue(tp, out var ls)) return null;
        var len = ls.Count;
        List<TResult> ret = new(len);
        lock (dictLock)
        {
            for (int i = 0; i < len; i++)
            {
                ret.Add(func(ls[i]));
            }
        }
        return ret;
    }
    public void Travel(TType tp, Action<T> func)
    {
        if (!objs.TryGetValue(tp, out var ls)) return;
        lock (dictLock)
        {
            foreach (var obj in ls)
            {
                func(obj);
            }
        }
    }

    #endregion

    #region Find

    public T? Find(Func<T, bool> cond)
    {
        lock (dictLock)
        {
            foreach (var ls in objs.Values)
            {
                foreach (var obj in ls)
                {
                    if (cond(obj)) return obj;
                }
            }
        }
        return null;
    }
    public T? Find(TType tp, Func<T, bool> cond)
    {
        if (!objs.TryGetValue(tp, out var ls)) return null;
        lock (dictLock)
        {
            foreach (var obj in ls)
            {
                if (cond(obj)) return obj;
            }
        }
        return null;
    }

    #endregion

    #region Until

    public (List<TResult> results, T? target) Until<TResult>(Func<T, TResult> func, Func<T, bool> cond)
    {
        List<TResult> ret = [];
        T? retObj = null;
        lock (dictLock)
        {
            bool flag = false;
            foreach (var ls in objs.Values)
            {
                foreach (var obj in ls)
                {
                    ret.Add(func(obj));
                    if (cond(obj))
                    {
                        retObj = obj;
                        flag = true;
                        break;
                    }
                }
                if (flag) break;
            }
        }
        return (ret, retObj);
    }
    public T? Until(Action<T> func, Func<T, bool> cond)
    {
        lock (dictLock)
        {
            foreach (var ls in objs.Values)
            {
                foreach (var obj in ls)
                {
                    func(obj);
                    if (cond(obj)) return obj;
                }
            }
        }
        return null;
    }
    public (List<TResult>? results, T? target) Until<TResult>(TType tp, Func<T, TResult> func, Func<T, bool> cond)
    {
        if (!objs.TryGetValue(tp, out var ls)) return (null, null);
        var len = ls.Count;
        List<TResult> ret = new(len);
        T? retObj = null;
        lock (dictLock)
        {
            foreach (var obj in ls)
            {
                ret.Add(func(obj));
                if (cond(obj))
                {
                    retObj = obj;
                    break;
                }
            }
        }
        return (ret, retObj);
    }
    public T? Until(TType tp, Action<T> func, Func<T, bool> cond)
    {
        if (!objs.TryGetValue(tp, out var ls)) return null;
        lock (dictLock)
        {
            foreach (var obj in ls)
            {
                func(obj);
                if (cond(obj)) return obj;
            }
        }
        return null;
    }

    #endregion

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
    public void Clear(TType tp)
    {
        if (!objs.TryGetValue(tp, out var ls)) return;
        lock (dictLock)
        {
            ls.Clear();
            objs.Remove(tp);
        }
    }

    #endregion
}

