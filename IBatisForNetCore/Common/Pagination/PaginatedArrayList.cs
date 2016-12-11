namespace IBatisNet.Common.Pagination
{
    using System;
    using System.Collections;
    using System.Reflection;

    public class PaginatedArrayList : IPaginatedList, IList, ICollection, IEnumerable, IEnumerator
    {
        private static ArrayList _emptyList = new ArrayList();
        private IList _list;
        private IList _page;
        private int _pageIndex;
        private int _pageSize;

        public PaginatedArrayList(int pageSize)
        {
            this._pageSize = pageSize;
            this._pageIndex = 0;
            this._list = new ArrayList();
            this.Repaginate();
        }

        public PaginatedArrayList(ICollection c, int pageSize)
        {
            this._pageSize = pageSize;
            this._pageIndex = 0;
            this._list = new ArrayList(c);
            this.Repaginate();
        }

        public PaginatedArrayList(int initialCapacity, int pageSize)
        {
            this._pageSize = pageSize;
            this._pageIndex = 0;
            this._list = new ArrayList(initialCapacity);
            this.Repaginate();
        }

        public int Add(object value)
        {
            int num = this._list.Add(value);
            this.Repaginate();
            return num;
        }

        public void Clear()
        {
            this._list.Clear();
            this.Repaginate();
        }

        public bool Contains(object value)
        {
            return this._page.Contains(value);
        }

        public void CopyTo(Array array, int index)
        {
            this._page.CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return this._page.GetEnumerator();
        }

        public void GotoPage(int pageIndex)
        {
            this._pageIndex = pageIndex;
            this.Repaginate();
        }

        public int IndexOf(object value)
        {
            return this._page.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            this._list.Insert(index, value);
            this.Repaginate();
        }

        public bool MoveNext()
        {
            return this._page.GetEnumerator().MoveNext();
        }

        public bool NextPage()
        {
            if (this.IsNextPageAvailable)
            {
                this._pageIndex++;
                this.Repaginate();
                return true;
            }
            return false;
        }

        public bool PreviousPage()
        {
            if (this.IsPreviousPageAvailable)
            {
                this._pageIndex--;
                this.Repaginate();
                return true;
            }
            return false;
        }

        public void Remove(object value)
        {
            this._list.Remove(value);
            this.Repaginate();
        }

        public void RemoveAt(int index)
        {
            this._list.RemoveAt(index);
            this.Repaginate();
        }

        private void Repaginate()
        {
            if (this._list.Count == 0)
            {
                this._page = _emptyList;
            }
            else
            {
                int fromIndex = this._pageIndex * this._pageSize;
                int num2 = (fromIndex + this._pageSize) - 1;
                if (num2 >= this._list.Count)
                {
                    num2 = this._list.Count - 1;
                }
                if (fromIndex >= this._list.Count)
                {
                    this._pageIndex = 0;
                    this.Repaginate();
                }
                else if (fromIndex < 0)
                {
                    this._pageIndex = this._list.Count / this._pageSize;
                    if ((this._list.Count % this._pageSize) == 0)
                    {
                        this._pageIndex--;
                    }
                    this.Repaginate();
                }
                else
                {
                    this._page = this.SubList(this._list, fromIndex, num2 + 1);
                }
            }
        }

        public void Reset()
        {
            this._page.GetEnumerator().Reset();
        }

        private IList SubList(IList list, int fromIndex, int toIndex)
        {
            return ((ArrayList) list).GetRange(fromIndex, toIndex - fromIndex);
        }

        public int Count
        {
            get
            {
                return this._page.Count;
            }
        }

        public object Current
        {
            get
            {
                return this._page.GetEnumerator().Current;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return (this._page.Count == 0);
            }
        }

        public bool IsFirstPage
        {
            get
            {
                return (this._pageIndex == 0);
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return this._list.IsFixedSize;
            }
        }

        public bool IsLastPage
        {
            get
            {
                return ((this._list.Count - ((this._pageIndex + 1) * this._pageSize)) < 1);
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
                return !this.IsLastPage;
            }
        }

        public bool IsPreviousPageAvailable
        {
            get
            {
                return !this.IsFirstPage;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this._list.IsReadOnly;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return this._page.IsSynchronized;
            }
        }

        public object this[int index]
        {
            get
            {
                return this._page[index];
            }
            set
            {
                this._list[index] = value;
                this.Repaginate();
            }
        }

        public int PageIndex
        {
            get
            {
                return this._pageIndex;
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
                return this._page.SyncRoot;
            }
        }
    }
}

