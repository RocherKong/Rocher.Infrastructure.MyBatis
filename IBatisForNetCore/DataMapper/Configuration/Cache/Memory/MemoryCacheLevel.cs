namespace IBatisNet.DataMapper.Configuration.Cache.Memory
{
    using IBatisNet.DataMapper.Exceptions;
    using System;
    using System.Collections;

    public class MemoryCacheLevel
    {
        private static Hashtable _cacheLevelMap = new Hashtable();
        private string _referenceType;
        public static MemoryCacheLevel Strong = new MemoryCacheLevel("STRONG");
        public static MemoryCacheLevel Weak = new MemoryCacheLevel("WEAK");

        static MemoryCacheLevel()
        {
            _cacheLevelMap[Weak.ReferenceType] = Weak;
            _cacheLevelMap[Strong.ReferenceType] = Strong;
        }

        private MemoryCacheLevel(string type)
        {
            this._referenceType = type;
        }

        public static MemoryCacheLevel GetByRefenceType(string referenceType)
        {
            MemoryCacheLevel level = (MemoryCacheLevel) _cacheLevelMap[referenceType];
            if (level == null)
            {
                throw new DataMapperException("Error getting CacheLevel (reference type) for name: '" + referenceType + "'.");
            }
            return level;
        }

        public string ReferenceType
        {
            get
            {
                return this._referenceType;
            }
        }
    }
}

