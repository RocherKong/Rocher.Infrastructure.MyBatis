namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers
{
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
    using System;
    using System.Text;

    public interface ISqlTagHandler
    {
        int DoEndFragment(SqlTagContext ctx, SqlTag tag, object parameterObject, StringBuilder bodyContent);
        void DoPrepend(SqlTagContext ctx, SqlTag tag, object parameterObject, StringBuilder bodyContent);
        int DoStartFragment(SqlTagContext ctx, SqlTag tag, object parameterObject);

        bool IsPostParseRequired { get; }
    }
}

