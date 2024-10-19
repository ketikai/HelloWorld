using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

namespace HelloWorld.Models.Util
{
    [Serializable]
    public class ObservableSortedSet<T> : ICollection<T>, ICollection, ISet<T>, IReadOnlyCollection<T>, IReadOnlySet<T>, ISerializable, IDeserializationCallback, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly SortedSet<T> _set;

        public int Count => _set.Count;

        bool ICollection<T>.IsReadOnly => ((ICollection<T>)_set).IsReadOnly;

        bool ICollection.IsSynchronized => ((ICollection)_set).IsSynchronized;

        object ICollection.SyncRoot => ((ICollection)_set).SyncRoot;

        public ObservableSortedSet()
        {
            _set = new SortedSet<T>();
        }

        public ObservableSortedSet(IComparer<T>? comparer)
        {
            _set = new SortedSet<T>(comparer);
        }

        public ObservableSortedSet(IEnumerable<T> collection)
        {
            _set = new SortedSet<T>(collection);
        }
        
        public ObservableSortedSet(IEnumerable<T> collection, IComparer<T>? comparer)
        {
            _set = new SortedSet<T>(collection, comparer);
        }

        protected ObservableSortedSet(SerializationInfo info, StreamingContext context)
        {
            _set = (SortedSet<T>) Activator.CreateInstance(typeof(SortedSet<T>),
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance,
                null,
                new object[] { info, context },
                null,
                null
                )!;
        }

        public void Add(T item)
        {
            CheckReentrancy();

            if (_set.Add(item))
            {
                OnCountPropertyChanged();
                OnIndexerPropertyChanged();
                OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
            }
        }

        bool ISet<T>.Add(T item)
        {
            CheckReentrancy();

            bool ret = _set.Add(item);

            if (ret)
            {
                OnCountPropertyChanged();
                OnIndexerPropertyChanged();
                OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
            }

            return ret;
        }

        public bool Remove(T item)
        {
            CheckReentrancy();

            bool ret = _set.Remove(item);

            if (ret)
            {
                OnCountPropertyChanged();
                OnIndexerPropertyChanged();
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);
            }

            return ret;
        }

        public void Clear()
        {
            CheckReentrancy();

            _set.Clear();

            OnCountPropertyChanged();
            OnIndexerPropertyChanged();
            OnCollectionReset();
        }

        public bool Contains(T item)
        {
            return _set.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _set.CopyTo(array, arrayIndex);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)_set).CopyTo(array, index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _set.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _set.GetEnumerator();
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            _set.ExceptWith(other);
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            _set.IntersectWith(other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return _set.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return _set.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return _set.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return _set.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return _set.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return _set.SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            _set.SymmetricExceptWith(other);
        }

        public void UnionWith(IEnumerable<T> other)
        {
            _set.UnionWith(other);
        }

        private static readonly MethodInfo GetObjectDataMethod = typeof(SortedSet<T>).GetMethod("GetObjectData", BindingFlags.Instance | BindingFlags.NonPublic)!;

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectDataMethod.Invoke(_set, new object[] { info, context });
        }

        private static readonly MethodInfo OnDeserializationMethod = typeof(SortedSet<T>).GetMethod("OnDeserialization", BindingFlags.Instance | BindingFlags.NonPublic)!;

        public virtual void OnDeserialization(object? sender)
        {
            OnDeserializationMethod.Invoke(_set, new object?[] { sender });
        }

        [field: NonSerialized]
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        [field: NonSerialized]
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        private readonly string ObservableLimitedDequeReentrancyNotAllowed = $"Cannot change {nameof(ObservableSortedSet<T>)} during a CollectionChanged event.";

        protected void CheckReentrancy()
        {
            if (_blockReentrancyCount > 0)
            {
                if (CollectionChanged?.GetInvocationList().Length > 1)
                    throw new InvalidOperationException(ObservableLimitedDequeReentrancyNotAllowed);
            }
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler? handler = CollectionChanged;
            if (handler != null)
            {
                _blockReentrancyCount++;
                try
                {
                    handler(this, e);
                }
                finally
                {
                    _blockReentrancyCount--;
                }
            }
        }

        private void OnCountPropertyChanged() => OnPropertyChanged(EventArgsCache.CountPropertyChanged);

        private void OnIndexerPropertyChanged() => OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object? item, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object? item)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item));
        }

        private void OnCollectionReset() => OnCollectionChanged(EventArgsCache.ResetCollectionChanged);

        private SimpleMonitor? _monitor;
        private SimpleMonitor EnsureMonitorInitialized() => _monitor ??= new SimpleMonitor(this);

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            EnsureMonitorInitialized();
            _monitor!._busyCount = _blockReentrancyCount;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_monitor != null)
            {
                _blockReentrancyCount = _monitor._busyCount;
                _monitor._observableSet = this;
            }
        }

        [NonSerialized]
        private int _blockReentrancyCount;

        protected IDisposable BlockReentrancy()
        {
            _blockReentrancyCount++;
            return EnsureMonitorInitialized();
        }

        [Serializable]
        private sealed class SimpleMonitor : IDisposable
        {
            internal int _busyCount;

            [NonSerialized]
            internal ObservableSortedSet<T> _observableSet;

            public SimpleMonitor(ObservableSortedSet<T> observableSet)
            {
                Debug.Assert(observableSet != null);
                _observableSet = observableSet;
            }

            public void Dispose() => _observableSet._blockReentrancyCount--;
        }
    }
}
