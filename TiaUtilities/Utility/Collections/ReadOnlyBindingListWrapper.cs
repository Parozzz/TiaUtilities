using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.Utility.Collections
{
    public class ReadOnlyBindingListWrapper<T> : IBindingList, IList<T>, IDisposable
    {

        // --- NOTIFICHE E PROPRIETÀ READ-ONLY ---
        public event ListChangedEventHandler ListChanged;

        public bool IsReadOnly => true;
        public bool AllowEdit => false;
        public bool AllowNew => false;
        public bool AllowRemove => false;

        public int Count => this._source.Count;

        public bool SupportsChangeNotification => true;
        public bool SupportsSearching => ((IBindingList)this._source).SupportsSearching;
        public bool SupportsSorting => ((IBindingList)this._source).SupportsSorting;
        public bool IsSorted => ((IBindingList)this._source).IsSorted;
        public ListSortDirection SortDirection => ((IBindingList)this._source).SortDirection;
        public PropertyDescriptor? SortProperty => ((IBindingList)this._source).SortProperty;

        public bool IsFixedSize => true;
        public bool IsSynchronized => ((ICollection)this._source).IsSynchronized;
        public object SyncRoot => ((ICollection)this._source).SyncRoot;

        private readonly BindingList<T> _source;
        private readonly ListChangedEventHandler _listChangedEvent;

        public ReadOnlyBindingListWrapper(BindingList<T> source)
        {
            Validate.NotNull(source);

            this._source = source;
            this._listChangedEvent = (sender, args) => ListChanged?.Invoke(this, args);

            this._source.ListChanged += this._listChangedEvent;
        }

        // --- ACCESSO IN LETTURA ---
        public T this[int index]
        {
            get => this._source[index];
            set => throw new NotSupportedException("Read-Only list.");
        }

        object? IList.this[int index]
        {
            get => this._source[index];
            set => throw new NotSupportedException("Read-Only list.");
        }

        public bool Contains(T item) => this._source.Contains(item);
        public int IndexOf(T item) => this._source.IndexOf(item);
        public void CopyTo(T[] array, int arrayIndex) => this._source.CopyTo(array, arrayIndex);
        public IEnumerator<T> GetEnumerator() => this._source.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this._source.GetEnumerator();

        // --- OPERAZIONI DI MODIFICA (BLOCCATE) ---
        public void Add(T item) => throw new NotSupportedException("Read-Only list.");
        public void Clear() => throw new NotSupportedException("Read-Only list.");
        public bool Remove(T item) => throw new NotSupportedException("Read-Only list.");
        public void Insert(int index, T item) => throw new NotSupportedException("Read-Only list.");
        public void RemoveAt(int index) => throw new NotSupportedException("Read-Only list.");

        int IList.Add(object? value) => throw new NotSupportedException("Read-Only list.");
        void IList.Clear() => throw new NotSupportedException("Read-Only list.");
        void IList.Insert(int index, object? value) => throw new NotSupportedException("Read-Only list.");
        void IList.Remove(object? value) => throw new NotSupportedException("Read-Only list.");
        void IList.RemoveAt(int index) => throw new NotSupportedException("Read-Only list.");
        bool IList.Contains(object? value) => ((IList)this._source).Contains(value);
        int IList.IndexOf(object? value) => ((IList)this._source).IndexOf(value);

        public object AddNew() => throw new NotSupportedException("Read-Only list.");
        public void ApplySort(PropertyDescriptor property, ListSortDirection direction) => ((IBindingList)this._source).ApplySort(property, direction);
        public int Find(PropertyDescriptor property, object key) => ((IBindingList)this._source).Find(property, key);
        public void RemoveSort() => ((IBindingList)this._source).RemoveSort();
        public void AddIndex(PropertyDescriptor property) => ((IBindingList)this._source).AddIndex(property);
        public void RemoveIndex(PropertyDescriptor property) => ((IBindingList)this._source).RemoveIndex(property);

        public void CopyTo(Array array, int index) => ((ICollection)this._source).CopyTo(array, index);

        public void Dispose()
        {
            this._source.ListChanged -= this._listChangedEvent;
            GC.SuppressFinalize(this);
        }
    }
}
