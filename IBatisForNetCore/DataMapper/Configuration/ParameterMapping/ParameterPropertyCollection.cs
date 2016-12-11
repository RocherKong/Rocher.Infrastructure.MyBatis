namespace IBatisNet.DataMapper.Configuration.ParameterMapping
{
    using System;
    using System.Reflection;

    public class ParameterPropertyCollection
    {
        private int _count;
        private ParameterProperty[] _innerList;
        private const int CAPACITY_MULTIPLIER = 2;
        private const int DEFAULT_CAPACITY = 4;

        public ParameterPropertyCollection()
        {
            this._innerList = new ParameterProperty[4];
            this._count = 0;
        }

        public ParameterPropertyCollection(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("Capacity", "The size of the list must be >0.");
            }
            this._innerList = new ParameterProperty[capacity];
        }

        public int Add(ParameterProperty value)
        {
            this.Resize(this._count + 1);
            int index = this._count++;
            this._innerList[index] = value;
            return index;
        }

        public void AddRange(ParameterProperty[] value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                this.Add(value[i]);
            }
        }

        public void AddRange(ParameterPropertyCollection value)
        {
            for (int i = 0; i < value.Count; i++)
            {
                this.Add(value[i]);
            }
        }

        public bool Contains(ParameterProperty value)
        {
            for (int i = 0; i < this._count; i++)
            {
                if (this._innerList[i].PropertyName == value.PropertyName)
                {
                    return true;
                }
            }
            return false;
        }

        public void Insert(int index, ParameterProperty value)
        {
            if ((index < 0) || (index > this._count))
            {
                throw new ArgumentOutOfRangeException("index");
            }
            this.Resize(this._count + 1);
            Array.Copy(this._innerList, index, this._innerList, index + 1, this._count - index);
            this._innerList[index] = value;
            this._count++;
        }

        public void Remove(ParameterProperty value)
        {
            for (int i = 0; i < this._count; i++)
            {
                if (this._innerList[i].PropertyName == value.PropertyName)
                {
                    this.RemoveAt(i);
                    return;
                }
            }
        }

        public void RemoveAt(int index)
        {
            if ((index < 0) || (index >= this._count))
            {
                throw new ArgumentOutOfRangeException("index");
            }
            int length = (this._count - index) - 1;
            if (length > 0)
            {
                Array.Copy(this._innerList, index + 1, this._innerList, index, length);
            }
            this._count--;
            this._innerList[this._count] = null;
        }

        private void Resize(int minSize)
        {
            int length = this._innerList.Length;
            if (minSize > length)
            {
                ParameterProperty[] sourceArray = this._innerList;
                int num2 = sourceArray.Length * 2;
                if (num2 < minSize)
                {
                    num2 = minSize;
                }
                this._innerList = new ParameterProperty[num2];
                Array.Copy(sourceArray, 0, this._innerList, 0, this._count);
            }
        }

        public int Count
        {
            get
            {
                return this._count;
            }
        }

        public ParameterProperty this[int index]
        {
            get
            {
                if ((index < 0) || (index >= this._count))
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                return this._innerList[index];
            }
            set
            {
                if ((index < 0) || (index >= this._count))
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                this._innerList[index] = value;
            }
        }

        public int Length
        {
            get
            {
                return this._innerList.Length;
            }
        }
    }
}

