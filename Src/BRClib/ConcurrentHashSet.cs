using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace System.Collections.Concurrent
{
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(ConcurrentHashSetDebugView<>))]
    public class ConcurrentHashSet<T> : ICollection<T>
    {
        private ConcurrentDictionary<T, byte> _data;
        static readonly KeyValuePair<T, byte>[] _emptyArray =
            new KeyValuePair<T, byte>[0];

        IEqualityComparer<T> m_comparer;

        public int Count => _data.Keys.Count;
        public bool IsReadOnly => false;


        public ConcurrentHashSet()
        {
            _data = new ConcurrentDictionary<T, byte>();
        }
        public ConcurrentHashSet(IEnumerable<T> collection)
        {
            InitInternalData(collection);
        }
        public ConcurrentHashSet(IEqualityComparer<T> comparer)
        {
            InitInternalData(comp: comparer);
        }
        public ConcurrentHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            InitInternalData(collection, comparer);
        }

        
        // initialize _data, if a collection was passed, convert it to a KeyValuePair
        // and save a copy of the EqualityComparer
        void InitInternalData(IEnumerable<T> col = null, IEqualityComparer<T> comp = null)
        {
            var kvps = col != null ? col.Select(c => new KeyValuePair<T, byte>(c, 0)) : _emptyArray;
            m_comparer = comp ?? EqualityComparer<T>.Default;

            _data = new ConcurrentDictionary<T, byte>(kvps, m_comparer);
        }


        public bool Add(T item)
        {
            return _data.TryAdd(item, 0);
        }

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


        public IEnumerator<T> GetEnumerator()
        {
            return _data.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // for debug view
        internal T[] ToArray()
        {
            T[] newArray = new T[_data.Keys.Count];
            CopyTo(newArray, 0);
            return newArray;
        }
    }

    internal class ConcurrentHashSetDebugView<T>
    {
        ConcurrentHashSet<T> _set;

        public ConcurrentHashSetDebugView(ConcurrentHashSet<T> set)
        {
            _set = set ?? throw new ArgumentNullException("set");
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get => _set.ToArray();
        }
    }
}
