namespace IBatisNet.DataMapper.TypeHandlers
{
    using System;

    public interface ITypeHandlerCallback
    {
        object GetResult(IResultGetter getter);
        void SetParameter(IParameterSetter setter, object parameter);
        object ValueOf(string s);

        object NullValue { get; }
    }
}

