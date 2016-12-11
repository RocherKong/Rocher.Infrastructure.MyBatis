namespace IBatisNet.DataMapper.Commands
{
    using IBatisNet.Common;
    using System;
    using System.Data;

    public sealed class DataReaderTransformer
    {
        public static IDataReader Transform(IDataReader reader, IDbProvider dbProvider)
        {
            if (!dbProvider.AllowMARS && !(reader is InMemoryDataReader))
            {
                return new InMemoryDataReader(reader);
            }
            return reader;
        }
    }
}

