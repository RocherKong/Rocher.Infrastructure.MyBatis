namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic;
    using System;

    public interface IDynamicParent
    {
        void AddChild(ISqlChild child);
    }
}

