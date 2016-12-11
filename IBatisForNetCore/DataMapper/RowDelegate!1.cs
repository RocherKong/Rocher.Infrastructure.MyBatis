namespace IBatisNet.DataMapper
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public delegate void RowDelegate<T>(object obj, object parameterObject, IList<T> list);
}

