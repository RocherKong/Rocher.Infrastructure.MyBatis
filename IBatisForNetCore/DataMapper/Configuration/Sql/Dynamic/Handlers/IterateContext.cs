namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers
{
    using IBatisNet.DataMapper.Exceptions;
    using System;
    using System.Collections;

    public sealed class IterateContext : IEnumerator
    {
        private ICollection _collection;
        private int _index = -1;
        private ArrayList _items = new ArrayList();

        public IterateContext(object collection)
        {
            if (collection is ICollection)
            {
                this._collection = (ICollection) collection;
            }
            else
            {
                if (!collection.GetType().IsArray)
                {
                    throw new DataMapperException("ParameterObject or property was not a Collection, Array or Iterator.");
                }
                object[] objArray = (object[]) collection;
                ArrayList list = new ArrayList();
                int length = objArray.Length;
                for (int i = 0; i < length; i++)
                {
                    list.Add(objArray[i]);
                }
                this._collection = list;
            }
            IEnumerator enumerator = ((IEnumerable) collection).GetEnumerator();
            while (enumerator.MoveNext())
            {
                this._items.Add(enumerator.Current);
            }
            IDisposable disposable = enumerator as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
            this._index = -1;
        }

        public bool MoveNext()
        {
            this._index++;
            if (this._index == this._items.Count)
            {
                return false;
            }
            return true;
        }

        public void Remove()
        {
            if (this._collection is IList)
            {
                ((IList) this._collection).Remove(this.Current);
            }
            else if (this._collection is IDictionary)
            {
                ((IDictionary) this._collection).Remove(this.Current);
            }
        }

        public void Reset()
        {
            this._index = -1;
        }

        public object Current
        {
            get
            {
                return this._items[this._index];
            }
        }

        public bool HasNext
        {
            get
            {
                return ((this._index >= -1) && (this._index < (this._items.Count - 1)));
            }
        }

        public int Index
        {
            get
            {
                return this._index;
            }
        }

        public bool IsFirst
        {
            get
            {
                return (this._index == 0);
            }
        }

        public bool IsLast
        {
            get
            {
                return (this._index == (this._items.Count - 1));
            }
        }
    }
}

