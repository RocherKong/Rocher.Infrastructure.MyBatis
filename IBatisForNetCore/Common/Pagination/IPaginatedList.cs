namespace IBatisNet.Common.Pagination
{
    using System;
    using System.Collections;

    public interface IPaginatedList : IList, ICollection, IEnumerable, IEnumerator
    {
        void GotoPage(int pageIndex);
        bool NextPage();
        bool PreviousPage();

        bool IsFirstPage { get; }

        bool IsLastPage { get; }

        bool IsMiddlePage { get; }

        bool IsNextPageAvailable { get; }

        bool IsPreviousPageAvailable { get; }

        int PageIndex { get; }

        int PageSize { get; }
    }
}

