using System;
using System.Collections.Generic;

namespace Logitech
{
    public static class DictionaryExtensions
    {
        public static TValue EnsureItem<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key,
            Func<TKey, TValue> factory)
        {
            bool created;
            return EnsureItem(source, key, factory, out created);
        }

        public static TValue EnsureItem<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key,
            Func<TKey, TValue> factory, out bool created)
        {
            created = false;
            TValue value;
            if (!source.TryGetValue(key, out value))
            {
                value = factory(key);
                if (!Equals(value, default(TValue)))
                {
                    created = true;
                    source[key] = value;
                }
            }
            return value;
        }
    }
}