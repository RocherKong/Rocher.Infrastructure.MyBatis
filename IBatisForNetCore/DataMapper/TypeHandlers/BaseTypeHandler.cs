namespace IBatisNet.DataMapper.TypeHandlers
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using System;
    using System.Data;

    public abstract class BaseTypeHandler : ITypeHandler
    {
        protected BaseTypeHandler()
        {
        }

        public virtual bool Equals(object obj, string str)
        {
            if ((obj == null) || (str == null))
            {
                return (((string) obj) == str);
            }
            object obj2 = this.ValueOf(obj.GetType(), str);
            return obj.Equals(obj2);
        }

        public abstract object GetDataBaseValue(object outputValue, Type parameterType);
        public abstract object GetValueByIndex(ResultProperty mapping, IDataReader dataReader);
        public abstract object GetValueByName(ResultProperty mapping, IDataReader dataReader);
        public virtual void SetParameter(IDataParameter dataParameter, object parameterValue, string dbType)
        {
            if (parameterValue != null)
            {
                dataParameter.Value = parameterValue;
            }
            else
            {
                dataParameter.Value = DBNull.Value;
            }
        }

        public abstract object ValueOf(Type type, string s);

        public abstract bool IsSimpleType { get; }

        public virtual object NullValue
        {
            get
            {
                return null;
            }
        }
    }
}

