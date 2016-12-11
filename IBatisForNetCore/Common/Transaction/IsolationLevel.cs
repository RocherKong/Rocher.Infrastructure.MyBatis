namespace IBatisNet.Common.Transaction
{
    using System;

    public enum IsolationLevel
    {
        Serializable,
        RepeatableRead,
        ReadCommitted,
        ReadUncommitted,
        Unspecified
    }
}

