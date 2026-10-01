using System.Collections;
using System.ComponentModel;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.Utility.Collections
{
    public class BindingListSmart<T> : IList<T>, IBindingList
    {

        public bool AllowEdit => true;
        public bool AllowNew => true;
        public bool AllowRemove => true;

        public bool IsSynchronized => ((ICollection)_items).IsSynchronized;
        public object SyncRoot => ((ICollection)_items).SyncRoot;

        public bool IsSorted => false;
        public ListSortDirection SortDirection => ListSortDirection.Ascending;
        public PropertyDescriptor? SortProperty => null;

        public bool SupportsChangeNotification => true;
        public bool SupportsSearching => true;
        public bool SupportsSorting => false;

        public bool IsFixedSize => false;
        public bool IsReadOnly => false;
        public int Count => _items.Count;

        public bool RaiseListChangedEvents
        {
            get => _raiseEvents;
            set => _raiseEvents = value;
        }


        public event ListChangedEventHandler? ListChanged;

        private readonly IList<T> _items;
        private bool _raiseEvents = true;

        public BindingListSmart(IList<T> list)
        {
            this._items = list;
        }

        public BindingListSmart() : this([]) { }

        public T this[int index]
        {
            get => _items[index];
            set
            {
                if (index < 0 || index >= _items.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                T oldItem = this._items[index];
                this.UnregisterItem(oldItem);

                this._items[index] = value;
                this.RegisterItem(value);

                this.OnListChanged(new(ListChangedType.ItemChanged, index));
            }
        }

        object? IList.this[int index]
        {
            get => this[index];
            set => this[index] = (T)value!;
        }

        T IList<T>.this[int index]
        {
            get => this[index];
            set => this[index] = value;
        }

        public void ResetBindings()
        {
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected virtual void OnListChanged(ListChangedEventArgs e)
        {
            if (_raiseEvents)
            {
                ListChanged?.Invoke(this, e);
            }
        }

        private void RegisterItem(T item)
        {
            if (item is INotifyPropertyChanged notifyingItem)
            {
                notifyingItem.PropertyChanged += OnItemPropertyChanged;
            }
        }

        private void UnregisterItem(T item)
        {
            if (item is INotifyPropertyChanged notifyingItem)
            {
                notifyingItem.PropertyChanged -= OnItemPropertyChanged;
            }
        }

        private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (!_raiseEvents || sender is not T item)
            {
                return;
            }

            int index = _items.IndexOf(item);
            if (index >= 0)
            {
                PropertyDescriptor? propDesc = string.IsNullOrEmpty(e.PropertyName)
                    ? null
                    : TypeDescriptor.GetProperties(typeof(T))[e.PropertyName];

                this.OnListChanged(new(ListChangedType.ItemChanged, index, propDesc));
            }
        }

        public void AddRange(IEnumerable<T> collection)
        {
            ArgumentNullException.ThrowIfNull(collection);

            bool wasRaising = _raiseEvents;
            _raiseEvents = false;

            try
            {
                foreach (var item in collection)
                {
                    this._items.Add(item);
                    this.RegisterItem(item);
                }
            }
            finally
            {
                _raiseEvents = wasRaising;
            }

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        public void RemoveRange(IEnumerable<T> collection)
        {
            ArgumentNullException.ThrowIfNull(collection);

            bool wasRaising = _raiseEvents;
            _raiseEvents = false;

            try
            {
                foreach (var item in collection)
                {
                    if (_items.Remove(item))
                    {
                        UnregisterItem(item);
                    }
                }
            }
            finally
            {
                _raiseEvents = wasRaising;
            }

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        // --- METODI ILIST / IBINDINGLIST ---

        public int Add(object? value)
        {
            if (value is T t)
            {
                this.Add(t);
                return Count - 1;
            }

            return -1;
        }

        public void Add(T item)
        {
            this._items.Add(item);
            this.RegisterItem(item);

            this.OnListChanged(new(ListChangedType.ItemAdded, this._items.Count - 1));
        }

        public object? AddNew()
        {
            try
            {
                T item = Activator.CreateInstance<T>();
                this.Add(item);
                return item;
            }
            catch (MissingMethodException) { }

            return default;
        }

        public void Clear()
        {
            bool wasRaising = this._raiseEvents;
            this._raiseEvents = false;

            try
            {
                this._items.ForEach(this.UnregisterItem);
                this._items.Clear();
            }
            finally
            {
                this._raiseEvents = wasRaising;
            }

            this.OnListChanged(new(ListChangedType.Reset, -1));
        }

        public bool Contains(object? value) => value is T item && this.Contains(item);
        public bool Contains(T item) => this._items.Contains(item);

        public void CopyTo(Array array, int index) => ((ICollection)this._items).CopyTo(array, index);
        public void CopyTo(T[] array, int arrayIndex) => this._items.CopyTo(array, arrayIndex);

        public int IndexOf(object? value) => value is T item ? this.IndexOf(item) : -1;
        public int IndexOf(T item) => this._items.IndexOf(item);

        public void Insert(int index, object? value)
        {
            if (value is T t)
            {
                this.Insert(index, t);
            }
        }

        public void Insert(int index, T item)
        {
            _items.Insert(index, item);
            RegisterItem(item);

            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
        }

        public void Remove(object? value)
        {
            if (value is T item)
            {
                Remove(item);
            }
        }

        public bool Remove(T item)
        {
            int index = this._items.IndexOf(item);
            if (index >= 0)
            {
                this.UnregisterItem(item);
                this._items.RemoveAt(index);

                this.OnListChanged(new(ListChangedType.ItemDeleted, index));
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            if (index >= 0 || index < this._items.Count)
            {
                T item = this._items[index];
                this.UnregisterItem(item);
                this._items.RemoveAt(index);

                this.OnListChanged(new(ListChangedType.ItemDeleted, index));
            }

        }

        public IEnumerator<T> GetEnumerator() => this._items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this._items.GetEnumerator();

        // --- OPERAZIONI DI ORDINAMENTO & RICERCA NON SUPPORTATE ---

        public void AddIndex(PropertyDescriptor property) { }
        public void RemoveIndex(PropertyDescriptor property) { }
        public void ApplySort(PropertyDescriptor property, ListSortDirection direction) => throw new NotSupportedException();

        public int Find(PropertyDescriptor property, object key)
        {
            if (property != null && key != null)
            {
                for (int i = 0; i < this._items.Count; i++)
                {
                    object? itemValue = property.GetValue(this._items[i]);
                    if (Object.Equals(itemValue, key))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        public void RemoveSort() => throw new NotSupportedException();
    }
}
