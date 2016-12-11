namespace IBatisNet.DataMapper.MappedStatements
{
    using IBatisNet.Common.Pagination;
    using IBatisNet.DataMapper;
    using IBatisNet.DataMapper.Exceptions;
    using System;
    using System.Collections;
    using System.Reflection;

    public class PaginatedList : IPaginatedList, IList, ICollection, IEnumerable, IEnumerator
    {
        private IList _currentPageList;
        private int _index;
        private IMappedStatement _mappedStatement;
        private IList _nextPageList;
        private int _pageSize;
        private object _parameterObject;
        private IList _prevPageList;

        public PaginatedList(IMappedStatement mappedStatement, object parameterObject, int pageSize)
        {
            this._mappedStatement = mappedStatement;
            this._parameterObject = parameterObject;
            this._pageSize = pageSize;
            this._index = 0;
            this.PageTo(0);
        }

        public int Add(object value)
        {
            return this._currentPageList.Add(value);
        }

        public void Clear()
        {
            this._currentPageList.Clear();
        }

        public bool Contains(object value)
        {
            return this._currentPageList.Contains(value);
        }

        public void CopyTo(Array array, int index)
        {
            this._currentPageList.CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return this._currentPageList.GetEnumerator();
        }

        private IList GetList(int index, int localPageSize)
        {
            bool flag = false;
            ISqlMapSession localSession = this._mappedStatement.SqlMap.LocalSession;
            if (localSession == null)
            {
                localSession = new SqlMapSession(this._mappedStatement.SqlMap);
                localSession.OpenConnection();
                flag = true;
            }
            IList list = null;
            try
            {
                list = this._mappedStatement.ExecuteQueryForList(localSession, this._parameterObject, index * this._pageSize, localPageSize);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (flag)
                {
                    localSession.CloseConnection();
                }
            }
            return list;
        }

        public void GotoPage(int pageIndex)
        {
            this.SafePageTo(pageIndex);
        }

        public int IndexOf(object value)
        {
            return this._currentPageList.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            this._currentPageList.Insert(index, value);
        }

        public bool MoveNext()
        {
            return this._currentPageList.GetEnumerator().MoveNext();
        }

        public bool NextPage()
        {
            if (this.IsNextPageAvailable)
            {
                this._index++;
                this.PageForward();
                return true;
            }
            return false;
        }

        private void PageBack()
        {
            try
            {
                this._nextPageList = this._currentPageList;
                this._currentPageList = this._prevPageList;
                if (this._index > 0)
                {
                    this._prevPageList = this.GetList(this._index - 1, this._pageSize);
                }
                else
                {
                    this._prevPageList = new ArrayList();
                }
            }
            catch (DataMapperException exception)
            {
                throw new DataMapperException("Unexpected error while repaginating paged list.  Cause: " + exception.Message, exception);
            }
        }

        private void PageForward()
        {
            try
            {
                this._prevPageList = this._currentPageList;
                this._currentPageList = this._nextPageList;
                this._nextPageList = this.GetList(this._index + 1, this._pageSize);
            }
            catch (DataMapperException exception)
            {
                throw new DataMapperException("Unexpected error while repaginating paged list.  Cause: " + exception.Message, exception);
            }
        }

        public void PageTo(int index)
        {
            this._index = index;
            IList list = null;
            if (index < 1)
            {
                list = this.GetList(this._index, this._pageSize * 2);
            }
            else
            {
                list = this.GetList(index - 1, this._pageSize * 3);
            }
            if (list.Count < 1)
            {
                this._prevPageList = new ArrayList();
                this._currentPageList = new ArrayList();
                this._nextPageList = new ArrayList();
            }
            else if (index < 1)
            {
                this._prevPageList = new ArrayList();
                if (list.Count <= this._pageSize)
                {
                    this._currentPageList = this.SubList(list, 0, list.Count);
                    this._nextPageList = new ArrayList();
                }
                else
                {
                    this._currentPageList = this.SubList(list, 0, this._pageSize);
                    this._nextPageList = this.SubList(list, this._pageSize, list.Count);
                }
            }
            else if (list.Count <= this._pageSize)
            {
                this._prevPageList = this.SubList(list, 0, list.Count);
                this._currentPageList = new ArrayList();
                this._nextPageList = new ArrayList();
            }
            else if (list.Count <= (this._pageSize * 2))
            {
                this._prevPageList = this.SubList(list, 0, this._pageSize);
                this._currentPageList = this.SubList(list, this._pageSize, list.Count);
                this._nextPageList = new ArrayList();
            }
            else
            {
                this._prevPageList = this.SubList(list, 0, this._pageSize);
                this._currentPageList = this.SubList(list, this._pageSize, this._pageSize * 2);
                this._nextPageList = this.SubList(list, this._pageSize * 2, list.Count);
            }
        }

        public bool PreviousPage()
        {
            if (this.IsPreviousPageAvailable)
            {
                this._index--;
                this.PageBack();
                return true;
            }
            return false;
        }

        public void Remove(object value)
        {
            this._currentPageList.Remove(value);
        }

        public void RemoveAt(int index)
        {
            this._currentPageList.RemoveAt(index);
        }

        public void Reset()
        {
            this._currentPageList.GetEnumerator().Reset();
        }

        private void SafePageTo(int index)
        {
            try
            {
                this.PageTo(index);
            }
            catch (DataMapperException exception)
            {
                throw new DataMapperException("Unexpected error while repaginating paged list.  Cause: " + exception.Message, exception);
            }
        }

        private IList SubList(IList list, int fromIndex, int toIndex)
        {
            return ((ArrayList) list).GetRange(fromIndex, toIndex - fromIndex);
        }

        public int Count
        {
            get
            {
                return this._currentPageList.Count;
            }
        }

        public object Current
        {
            get
            {
                return this._currentPageList.GetEnumerator().Current;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return (this._currentPageList.Count == 0);
            }
        }

        public bool IsFirstPage
        {
            get
            {
                return (this._index == 0);
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return this._currentPageList.IsFixedSize;
            }
        }

        public bool IsLastPage
        {
            get
            {
                return (this._nextPageList.Count < 1);
            }
        }

        public bool IsMiddlePage
        {
            get
            {
                return (!this.IsFirstPage && !this.IsLastPage);
            }
        }

        public bool IsNextPageAvailable
        {
            get
            {
                return (this._nextPageList.Count > 0);
            }
        }

        public bool IsPreviousPageAvailable
        {
            get
            {
                return (this._prevPageList.Count > 0);
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this._currentPageList.IsReadOnly;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return this._currentPageList.IsSynchronized;
            }
        }

        public object this[int index]
        {
            get
            {
                return this._currentPageList[index];
            }
            set
            {
                this._currentPageList[index] = value;
            }
        }

        public int PageIndex
        {
            get
            {
                return this._index;
            }
        }

        public int PageSize
        {
            get
            {
                return this._pageSize;
            }
        }

        public object SyncRoot
        {
            get
            {
                return this._currentPageList.SyncRoot;
            }
        }
    }
}

