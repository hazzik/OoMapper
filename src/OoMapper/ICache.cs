using System;

namespace OoMapper
{
    public interface ICache<TKey, TValue>
    {
        TValue GetOrAdd(TKey key, Func<TKey, TValue> func);
    }
}