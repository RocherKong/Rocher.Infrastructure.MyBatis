namespace IBatisNet.DataMapper.Scope
{
    using IBatisNet.DataMapper.DataExchange;

    public interface IScope
    {
        IBatisNet.DataMapper.DataExchange.DataExchangeFactory DataExchangeFactory { get; }

        IBatisNet.DataMapper.Scope.ErrorContext ErrorContext { get; }
    }
}

