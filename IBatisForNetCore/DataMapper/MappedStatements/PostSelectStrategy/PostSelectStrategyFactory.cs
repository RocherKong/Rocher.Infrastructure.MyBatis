namespace IBatisNet.DataMapper.MappedStatements.PostSelectStrategy
{
    using IBatisNet.DataMapper.MappedStatements;
    using System;
    using System.Collections;

    public sealed class PostSelectStrategyFactory
    {
        private static IDictionary _strategies = new HybridDictionary();

        static PostSelectStrategyFactory()
        {
            _strategies.Add(PostBindind.ExecuteMethod.ExecuteQueryForArrayList, new ArrayStrategy());
            _strategies.Add(PostBindind.ExecuteMethod.ExecuteQueryForIList, new ListStrategy());
            _strategies.Add(PostBindind.ExecuteMethod.ExecuteQueryForObject, new ObjectStrategy());
            _strategies.Add(PostBindind.ExecuteMethod.ExecuteQueryForStrongTypedIList, new StrongTypedListStrategy());
            _strategies.Add(PostBindind.ExecuteMethod.ExecuteQueryForGenericIList, new GenericListStrategy());
        }

        public static IPostSelectStrategy Get(PostBindind.ExecuteMethod method)
        {
            return (IPostSelectStrategy) _strategies[method];
        }
    }
}

