using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Collections.Concurrent
{
    public class ConcurrentHashSet<T> : IDisposable, ICollection<T>
    {
        private readonly ReaderWriterLockSlim _lock;
        private readonly HashSet<T> _data;

        public int Count
        {
            get
            {
                _lock.EnterReadLock();

                try
                {
                    return _data.Count;
                }
                finally
                {
                    if (_lock.IsReadLockHeld)
                        _lock.ExitReadLock();
                }
            }
        }

        public bool IsReadOnly => false;


        public ConcurrentHashSet()
        {
            _data = new HashSet<T>();
            _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }


        public bool Add(T item)
        {
            _lock.EnterWriteLock();

            try
            {
                return _data.Add(item);
            }
            finally
            {
                if (_lock.IsWriteLockHeld)
                    _lock.ExitWriteLock();
            }
        }
        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public void Clear()
        {
            _lock.EnterWriteLock();
            try
            {
                _data.Clear();
            }
            finally
            {
                if (_lock.IsWriteLockHeld)
                    _lock.ExitWriteLock();
            }
        }

        public bool Contains(T item)
        {
            _lock.EnterReadLock();
            try
            {
                return _data.Contains(item);
            }
            finally
            {
                if (_lock.IsReadLockHeld)
                    _lock.ExitReadLock();
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _lock.EnterReadLock();
            try
            {
                _data.CopyTo(array, arrayIndex);
            }
            finally
            {
                if (_lock.IsReadLockHeld)
                    _lock.ExitReadLock();
            }
        }


        public IEnumerator<T> GetEnumerator()
        {
            _lock.EnterReadLock();
            try
            {
                return _data.GetEnumerator();
            }
            finally
            {
                if (_lock.IsReadLockHeld)
                    _lock.ExitReadLock();
            }
        }

        public bool Remove(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                return _data.Remove(item);
            }
            finally
            {
                if (_lock.IsWriteLockHeld)
                    _lock.ExitWriteLock();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }



        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_lock != null)
                {
                    _lock.Dispose();
                }
            }
        }

        ~ConcurrentHashSet()
        {
            Dispose(false);
        }
    }
}
