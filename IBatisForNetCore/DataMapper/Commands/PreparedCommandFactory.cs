namespace IBatisNet.DataMapper.Commands
{
    using System;

    internal sealed class PreparedCommandFactory
    {
        public static IPreparedCommand GetPreparedCommand(bool isEmbedStatementParams)
        {
            return new DefaultPreparedCommand();
        }
    }
}

