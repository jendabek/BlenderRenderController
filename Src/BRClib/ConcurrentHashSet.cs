using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace System.Collections.Concurrent
{
    [DebuggerDisplay("Count = {Count}")]
    public class ConcurrentHashSet<T> : ICollection<T>, ISet<T>
    {
        private ConcurrentDictionary<T, byte> _data;

        public int Count => _data.Keys.Count;
        public bool IsReadOnly => false;


        public ConcurrentHashSet()
        {
            _data = new ConcurrentDictionary<T, byte>();
        }
        public ConcurrentHashSet(IEnumerable<T> collection)
        {
            var init = collection.Select(i => new KeyValuePair<T, byte>(i, byte.MinValue));
            _data = new ConcurrentDictionary<T, byte>(init);
        }
        public ConcurrentHashSet(IEqualityComparer<T> comparer)
        {
            _data = new ConcurrentDictionary<T, byte>(comparer);
        }
        public ConcurrentHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            var kvps = collection.Select(c => new KeyValuePair<T, byte>(c, byte.MinValue));
            _data = new ConcurrentDictionary<T, byte>(kvps, comparer);
        }


        #region ICollection
        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public void Clear()
        {
            _data.Clear();
        }

        public bool Contains(T item)
        {
            return _data.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _data.Keys.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _data.TryRemove(item, out byte ph);
        } 
        #endregion

        #region ISet
        // For set operations, create new hashset, clear _data and re-add
        public bool Add(T item)
        {
            return _data.TryAdd(item, byte.MinValue);
        }

        public void UnionWith(IEnumerable<T> other)
        {
            var hash = new HashSet<T>(_data.Keys);
            hash.UnionWith(other);

            Clear();

            foreach (var item in hash)
            {
                Add(item);
            }
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            var hash = new HashSet<T>(_data.Keys);
            hash.IntersectWith(other);

            Clear();

            foreach (var item in hash)
            {
                Add(item);
            }
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            var hash = new HashSet<T>(_data.Keys);
            hash.ExceptWith(other);

            Clear();

            foreach (var item in hash)
            {
                Add(item);
            }
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            var hash = new HashSet<T>(_data.Keys);
            hash.SymmetricExceptWith(other);

            Clear();

            foreach (var item in hash)
            {
                Add(item);
            }
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            var hash = new HashSet<T>(_data.Keys);
            return hash.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            var hash = new HashSet<T>(_data.Keys);
            return hash.IsSupersetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            var hash = new HashSet<T>(_data.Keys);
            return hash.IsProperSupersetOf(other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            var hash = new HashSet<T>(_data.Keys);
            return hash.IsProperSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            var hash = new HashSet<T>(_data.Keys);
            return hash.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            var hash = new HashSet<T>(_data.Keys);
            return hash.SetEquals(other);
        }
        #endregion

        public IEnumerator<T> GetEnumerator()
        {
            return _data.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

}
