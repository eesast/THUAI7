using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Preparation.Utility.Value.SafeValue
{
    public class LockedClassList<T> where T : class?
    {
        private readonly ReaderWriterLockSlim listLock = new();
        private readonly List<T> list;

        #region 构造
        public LockedClassList()
        {
            list = [];
        }
        public LockedClassList(int capacity)
        {
            list = new(capacity);
        }
        public LockedClassList(IEnumerable<T> collection)
        {
            list = new(collection);
        }
        #endregion

        #region 修改
        public TResult WriteLock<TResult>(Func<TResult> func)
        {
            listLock.EnterWriteLock();
            try
            {
                return func();
            }
            finally
            {
                listLock.ExitWriteLock();
            }

        }
        public void WriteLock(Action func)
        {
            listLock.EnterWriteLock();
            try
            {
                func();
            }
            finally
            {
                listLock.ExitWriteLock();
            }

        }

        public void Add(T item)
        {
            WriteLock(() => { list.Add(item); });
        }
        public void AddRange(IEnumerable<T> lt)
        {
            WriteLock(() => { list.AddRange(lt); });
        }

        public void Insert(int index, T item)
        {
            WriteLock(() => { list.Insert(index, item); });
        }

        public void Clear()
        {
            WriteLock(list.Clear);
        }

        public bool Remove(T item)
        {
            return WriteLock(() => list.Remove(item));
        }

        public int RemoveAll(T item)
            => WriteLock(() => list.RemoveAll((t) => t == item));

        public T? RemoveOne(Predicate<T> match) =>
            WriteLock(() =>
            {
                int index = list.FindIndex(match);
                if (index == -1) return null;
                T ans = list[index];
                list.RemoveAt(index);
                return ans;
            });

        public int RemoveAll(Predicate<T> match)
            => WriteLock(() => list.RemoveAll(match));

        public bool RemoveAt(int index)
        {
            return WriteLock(() =>
            {
                if (index > list.Count) return false;
                list.RemoveAt(index);
                return true;
            });
        }
        #endregion

        #region 读取与对类操作
        public TResult? ReadLock<TResult>(Func<TResult?> func)
        {
            listLock.EnterReadLock();
            try
            {
                return func();
            }
            finally
            {
                listLock.ExitReadLock();
            }
        }
        public void ReadLock(Action func)
        {
            listLock.EnterReadLock();
            try
            {
                func();
            }
            finally
            {
                listLock.ExitReadLock();
            }

        }

        public T? this[int index]
        {
            get
            {
                return ReadLock(() => { return list[index]; });
            }
            set
            {
                ReadLock(() => { list[index] = value!; });
            }
        }
        public int Count => ReadLock(() => list.Count);

        public int IndexOf(T item)
        {
            return ReadLock(() => list.IndexOf(item));
        }

        public bool Contains(T item)
        {
            return ReadLock(() => list.Contains(item));
        }

        public T? Find(Predicate<T> match)
        {
            return ReadLock(() => list.Find(match));
        }

        public List<T>? FindAll(Predicate<T> match)
        {
            return ReadLock(() => list.FindAll(match));
        }

        public int FindIndex(Predicate<T> match)
            => ReadLock(() => list.FindIndex(match));

        public void ForEach(Action<T> action)
            => ReadLock(() => { list.ForEach(action); });

        public Array? ToArray()
        {
            return ReadLock(list.ToArray);
        }
        public List<T>? ToNewList()
        {
            List<T> lt = [];
            return ReadLock(() => { lt.AddRange(list); return lt; });
        }

        public LockedClassList<TResult>? Cast<TResult>() where TResult : class?
        {
            LockedClassList<TResult> lt = new();
            return ReadLock(() => { lt.AddRange(list.Cast<TResult>()); return lt; });
        }
        #endregion
    }
}