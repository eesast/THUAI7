using System;
using System.Threading;

namespace Preparation.Utility
{
    public interface ISafeAddable<T>
    {
        public void Add(T value);
    }
}