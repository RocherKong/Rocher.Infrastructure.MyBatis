namespace IBatisNet.Common.Utilities
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct StateConfig
    {
        public string FileName;
        public IBatisNet.Common.Utilities.ConfigureHandler ConfigureHandler;
    }
}

