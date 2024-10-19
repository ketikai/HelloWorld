using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

namespace HelloWorld.Models.Util
{
    [Serializable]
    public class LimitedDeque<T> : ICollection<T>, ICollection, IReadOnlyCollection<T>, ISerializable, IDeserializationCallback
    {
        public readonly int MaxSize;
        private readonly LinkedList<T> _linkedList;

        public T? First
        {
            get
            {
                var first = _linkedList.First;
                return first == null ? default : first.Value;
            }
        }

        public T? Last
        {
            get
            {
                var last = _linkedList.Last;
                return last == null ? default : last.Value;
            }
        }

        private static readonly int DEFAULT_MAX_SIZE = -1;

        public LimitedDeque() : this(DEFAULT_MAX_SIZE)
        {
        }

        public LimitedDeque(int maxSize)
        {
            _linkedList = new LinkedList<T>();
            MaxSize = maxSize;
        }

        protected LimitedDeque(SerializationInfo info, StreamingContext context)
        {
            MaxSize = info.GetInt32(MaxSizeName);
            _linkedList = (LinkedList<T>)Activator.CreateInstance(typeof(LinkedList<T>),
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance,
                null,
                new object[] { info, context },
                null,
                null
                )!;
        }

        public int Count => _linkedList.Count;

        bool ICollection<T>.IsReadOnly => ((ICollection<T>)_linkedList).IsReadOnly;

        bool ICollection.IsSynchronized => ((ICollection)_linkedList).IsSynchronized;

        object ICollection.SyncRoot => this;

        protected bool IsFull => MaxSize < 0 || Count >= MaxSize;
        protected bool IsZeroSize => MaxSize == 0;

        public virtual void AddFirst(T item)
        {
            if (IsZeroSize)
            {
                return;
            }
            if (IsFull && _linkedList.Count > 0)
            {
                RemoveLast();
            }
            _linkedList.AddFirst(item);
        }

        public virtual void RemoveFirst()
        {
            if (IsZeroSize)
            {
                return;
            }
            _linkedList.RemoveFirst();
        }

        public virtual void AddLast(T item)
        {
            if (IsZeroSize)
            {
                return;
            }
            if (IsFull && _linkedList.Count > 0)
            {
                RemoveFirst();
            }
            _linkedList.AddLast(item);
        }

        public virtual void RemoveLast()
        {
            if (IsZeroSize)
            {
                return;
            }
            _linkedList.RemoveLast();
        }

        public virtual void Clear()
        {
            if (IsZeroSize)
            {
                return;
            }
            _linkedList.Clear();
        }

        public virtual bool Remove(T item)
        {
            if (IsZeroSize)
            {
                return false;
            }
            return _linkedList.Remove(item);
        }

        void ICollection<T>.Add(T item)
        {
            AddLast(item);
        }

        public bool Contains(T item)
        {
            return _linkedList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _linkedList.CopyTo(array, arrayIndex);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_linkedList).CopyTo(array, index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _linkedList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _linkedList.GetEnumerator();
        }

        private const string MaxSizeName = "MaxSize";

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }
            info.AddValue(MaxSizeName, MaxSize);

            _linkedList.GetObjectData(info, context);
        }

        public virtual void OnDeserialization(object? sender)
        {
            _linkedList.OnDeserialization(sender);
        }
    }

    public class ObservableLimitedDeque<T> : LimitedDeque<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {

        public ObservableLimitedDeque(int maxSize) : base(maxSize)
        {
        }

        protected ObservableLimitedDeque(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ObservableLimitedDeque()
        {
        }

        private int FirstIndex => 0;
        private int LastIndex => Count - 1;

        public override void AddFirst(T item)
        {
            CheckReentrancy();

            var index = FirstIndex;
            base.AddFirst(item);

            OnCountPropertyChanged();
            OnIndexerPropertyChanged();
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        public override void RemoveFirst()
        {
            CheckReentrancy();

            var index = LastIndex;
            T? removedItem = First;

            base.RemoveFirst();

            OnCountPropertyChanged();
            OnIndexerPropertyChanged();
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, removedItem, index);
        }

        public override void AddLast(T item)
        {
            CheckReentrancy();

            var index = LastIndex;
            base.AddLast(item);

            OnCountPropertyChanged();
            OnIndexerPropertyChanged();
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        public override void RemoveLast()
        {
            CheckReentrancy();
            var index = LastIndex;
            T? removedItem = Last;

            base.RemoveLast();

            OnCountPropertyChanged();
            OnIndexerPropertyChanged();
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, removedItem, index);
        }

        public override void Clear()
        {
            CheckReentrancy();

            base.Clear();

            OnCountPropertyChanged();
            OnIndexerPropertyChanged();
            OnCollectionReset();
        }

        public override bool Remove(T item)
        {
            CheckReentrancy();
            T removedItem = item;

            var ret = base.Remove(item);

            if (ret)
            {
                OnCountPropertyChanged();
                OnIndexerPropertyChanged();
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, removedItem);
            }

            return ret;
        }

        [field: NonSerialized]
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        [field: NonSerialized]
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        private readonly string ObservableLimitedDequeReentrancyNotAllowed = $"Cannot change {nameof(ObservableLimitedDeque<T>)} during a CollectionChanged event.";

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
                _monitor._limitedList = this;
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
            internal ObservableLimitedDeque<T> _limitedList;

            public SimpleMonitor(ObservableLimitedDeque<T> limitedList)
            {
                Debug.Assert(limitedList != null);
                _limitedList = limitedList;
            }

            public void Dispose() => _limitedList._blockReentrancyCount--;
        }
    }

    internal static class EventArgsCache
    {
        internal static readonly PropertyChangedEventArgs CountPropertyChanged = new("Count");
        internal static readonly PropertyChangedEventArgs IndexerPropertyChanged = new("Item[]");
        internal static readonly NotifyCollectionChangedEventArgs ResetCollectionChanged = new(NotifyCollectionChangedAction.Reset);
    }
}
