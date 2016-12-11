namespace IBatisNet.Common.Transaction
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct TransactionOptions
    {
        public TimeSpan TimeOut;
        public IBatisNet.Common.Transaction.IsolationLevel IsolationLevel;
    }
}

