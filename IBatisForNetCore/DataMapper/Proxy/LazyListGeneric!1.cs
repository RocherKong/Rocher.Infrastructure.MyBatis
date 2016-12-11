namespace IBatisNet.DataMapper.Proxy
{
    using IBatisNet.Common.Logging;
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper;
    using IBatisNet.DataMapper.MappedStatements;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    [Serializable]
    public class LazyListGeneric<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable where T: class
    {
        private IList<T> _list;
        private bool _loaded;
        private object _loadLock;
        private static readonly ILog _logger;
        private object _param;
        private ISetAccessor _setAccessor;
        private ISqlMapper _sqlMap;
        private string _statementId;
        private object _target;

        static LazyListGeneric()
        {
            LazyListGeneric<T>._logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        public LazyListGeneric(IMappedStatement mappedSatement, object param, object target, ISetAccessor setAccessor)
        {
            this._statementId = string.Empty;
            this._loadLock = new object();
            this._param = param;
            this._statementId = mappedSatement.Id;
            this._sqlMap = mappedSatement.SqlMap;
            this._target = target;
            this._setAccessor = setAccessor;
            this._list = new List<T>();
        }

        public void Add(T item)
        {
            this.Load("Add");
            this._list.Add(item);
        }

        public void Clear()
        {
            this.Load("Clear");
            this._list.Clear();
        }

        public bool Contains(T item)
        {
            this.Load("Contains");
            return this._list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.Load("CopyTo");
            this._list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            this.Load("GetEnumerator<T>");
            return this._list.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            this.Load("IndexOf");
            return this._list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            this.Load("Insert");
            this._list.Insert(index, item);
        }

        private void Load(string methodName)
        {
            if (LazyListGeneric<T>._logger.IsDebugEnabled)
            {
                LazyListGeneric<T>._logger.Debug("Proxyfying call to " + methodName);
            }
            lock (this._loadLock)
            {
                if (!this._loaded)
                {
                    if (LazyListGeneric<T>._logger.IsDebugEnabled)
                    {
                        LazyListGeneric<T>._logger.Debug("Proxyfying call, query statement " + this._statementId);
                    }
                    this._list = this._sqlMap.QueryForList<T>(this._statementId, this._param);
                    this._loaded = true;
                    this._setAccessor.Set(this._target, this._list);
                }
            }
            if (LazyListGeneric<T>._logger.IsDebugEnabled)
            {
                LazyListGeneric<T>._logger.Debug("End of proxyfied call to " + methodName);
            }
        }

        public bool Remove(T item)
        {
            this.Load("Remove");
            return this._list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            this.Load("RemoveAt");
            this._list.RemoveAt(index);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.Load("CopyTo");
            ((IList) this._list).CopyTo(array, index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            this.Load("GetEnumerator");
            return this._list.GetEnumerator();
        }

        int IList.Add(object value)
        {
            this.Load("Add");
            return ((IList) this._list).Add(value);
        }

        void IList.Clear()
        {
            this.Clear();
        }

        bool IList.Contains(object value)
        {
            this.Load("Contains");
            return ((IList) this._list).Contains(value);
        }

        int IList.IndexOf(object value)
        {
            this.Load("IndexOf");
            return ((IList) this._list).IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            this.Load("IndexOf");
            ((IList) this._list).Insert(index, value);
        }

        void IList.Remove(object value)
        {
            this.Load("Remove");
            ((IList) this._list).Remove(value);
        }

        void IList.RemoveAt(int index)
        {
            this.RemoveAt(index);
        }

        public int Count
        {
            get
            {
                this.Load("Count");
                return this._list.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public T this[int index]
        {
            get
            {
                this.Load("this");
                return this._list[index];
            }
            set
            {
                this.Load("this");
                this._list[index] = value;
            }
        }

        int ICollection.Count
        {
            get
            {
                return this.Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        object IList.this[int index]
        {
            get
            {
                this.Load("this");
                return this[index];
            }
            set
            {
                this.Load("this");
                ((IList) this._list)[index] = value;
            }
        }
    }
}

