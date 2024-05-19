using System;
using System.Collections.Generic;

namespace TinyUrl.Cache
{
    public class LRUCache<TKey, TValue> where TKey : notnull
    {
        private readonly int _capacity;
        private readonly Dictionary<TKey, LinkedListNode<CacheItem>> _cache;
        private readonly LinkedList<CacheItem> _lruList;

        public LRUCache(int capacity)
        {
            _capacity = capacity;
            _cache = new Dictionary<TKey, LinkedListNode<CacheItem>>(capacity);
            _lruList = new LinkedList<CacheItem>();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_cache.TryGetValue(key, out var node))
            {
                value = node.Value.Value;
                _lruList.Remove(node);
                _lruList.AddFirst(node!);
                return true;
            }
            value = default!;
            return false;
        }

        public void Add(TKey key, TValue value)
        {
            if (_cache.TryGetValue(key, out var existingNode))
            {
                if (existingNode != null)
                {
                    _lruList.Remove(existingNode);
                    _cache.Remove(key);
                }
            }
            else if (_cache.Count >= _capacity)
            {
                var lruItem = _lruList.Last?.Value;
                if (lruItem != null)
                {
                    _cache.Remove(lruItem.Key);
                    _lruList.RemoveLast();
                }
            }

            var newNode = new LinkedListNode<CacheItem>(new CacheItem(key, value));
            _lruList.AddFirst(newNode);
            _cache[key] = newNode;
        }

        private class CacheItem
        {
            public TKey Key { get; }
            public TValue Value { get; }

            public CacheItem(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}
