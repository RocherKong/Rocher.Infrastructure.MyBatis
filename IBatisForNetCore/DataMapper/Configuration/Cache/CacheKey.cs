namespace IBatisNet.DataMapper.Configuration.Cache
{
    using IBatisNet.Common.Utilities;
    using System;
    using System.Collections;
    using System.Text;

    public class CacheKey
    {
        private long _checksum;
        private int _count;
        private int _hashCode;
        private int _multiplier;
        private IList _paramList;
        private const int DEFAULT_HASHCODE = 0x11;
        private const int DEFAULT_MULTIPLYER = 0x25;

        public CacheKey()
        {
            this._multiplier = 0x25;
            this._hashCode = 0x11;
            this._checksum = -9223372036854775808L;
            this._paramList = new ArrayList();
            this._hashCode = 0x11;
            this._multiplier = 0x25;
            this._count = 0;
        }

        public CacheKey(int initialNonZeroOddNumber)
        {
            this._multiplier = 0x25;
            this._hashCode = 0x11;
            this._checksum = -9223372036854775808L;
            this._paramList = new ArrayList();
            this._hashCode = initialNonZeroOddNumber;
            this._multiplier = 0x25;
            this._count = 0;
        }

        public CacheKey(int initialNonZeroOddNumber, int multiplierNonZeroOddNumber)
        {
            this._multiplier = 0x25;
            this._hashCode = 0x11;
            this._checksum = -9223372036854775808L;
            this._paramList = new ArrayList();
            this._hashCode = initialNonZeroOddNumber;
            this._multiplier = multiplierNonZeroOddNumber;
            this._count = 0;
        }

        public override bool Equals(object obj)
        {
            if (this != obj)
            {
                if (!(obj is CacheKey))
                {
                    return false;
                }
                CacheKey key = (CacheKey) obj;
                if (this._hashCode != key._hashCode)
                {
                    return false;
                }
                if (this._checksum != key._checksum)
                {
                    return false;
                }
                if (this._count != key._count)
                {
                    return false;
                }
                int count = this._paramList.Count;
                for (int i = 0; i < count; i++)
                {
                    object obj2 = this._paramList[i];
                    object obj3 = key._paramList[i];
                    if (obj2 == null)
                    {
                        if (obj3 != null)
                        {
                            return false;
                        }
                    }
                    else if (!obj2.Equals(obj3))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override string ToString()
        {
            return new StringBuilder().Append(this._hashCode).Append('|').Append(this._checksum).ToString();
        }

        public CacheKey Update(object obj)
        {
            int identityHashCode = HashCodeProvider.GetIdentityHashCode(obj);
            this._count++;
            this._checksum += identityHashCode;
            identityHashCode *= this._count;
            this._hashCode = (this._multiplier * this._hashCode) + identityHashCode;
            this._paramList.Add(obj);
            return this;
        }
    }
}

