namespace IBatisNet.Common.Utilities.TypesResolver
{
    using System;
    using System.Collections;

    public class CachedTypeResolver : ITypeResolver
    {
        private IDictionary _typeCache = new HybridDictionary();
        private ITypeResolver _typeResolver;

        public CachedTypeResolver(ITypeResolver typeResolver)
        {
            this._typeResolver = typeResolver;
        }

        public Type Resolve(string typeName)
        {
            if ((typeName == null) || (typeName.Trim().Length == 0))
            {
                throw new TypeLoadException("Could not load type from string value '" + typeName + "'.");
            }
            Type type = null;
            try
            {
                type = this._typeCache[typeName] as Type;
                if (type == null)
                {
                    type = this._typeResolver.Resolve(typeName);
                    this._typeCache[typeName] = type;
                }
            }
            catch (Exception exception)
            {
                if (exception is TypeLoadException)
                {
                    throw;
                }
                throw new TypeLoadException("Could not load type from string value '" + typeName + "'.", exception);
            }
            return type;
        }
    }
}

