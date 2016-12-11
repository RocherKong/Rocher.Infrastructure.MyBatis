namespace IBatisNet.Common.Utilities.Objects
{
    using IBatisNet.Common.Exceptions;
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Reflection;

    public sealed class ReflectionInfo
    {
        private string _className = string.Empty;
        private static readonly string[] _emptyStringArray = new string[0];
        private Hashtable _getMembers = new Hashtable();
        private Hashtable _getTypes = new Hashtable();
        private string[] _readableMemberNames = _emptyStringArray;
        private static Hashtable _reflectionInfoMap = Hashtable.Synchronized(new Hashtable());
        private Hashtable _setMembers = new Hashtable();
        private Hashtable _setTypes = new Hashtable();
        private static ArrayList _simleTypeMap = new ArrayList();
        private string[] _writeableMemberNames = _emptyStringArray;
        public static BindingFlags BINDING_FLAGS_FIELD = (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        public static BindingFlags BINDING_FLAGS_PROPERTY = (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        static ReflectionInfo()
        {
            _simleTypeMap.Add(typeof(string));
            _simleTypeMap.Add(typeof(byte));
            _simleTypeMap.Add(typeof(char));
            _simleTypeMap.Add(typeof(bool));
            _simleTypeMap.Add(typeof(Guid));
            _simleTypeMap.Add(typeof(short));
            _simleTypeMap.Add(typeof(int));
            _simleTypeMap.Add(typeof(long));
            _simleTypeMap.Add(typeof(float));
            _simleTypeMap.Add(typeof(double));
            _simleTypeMap.Add(typeof(decimal));
            _simleTypeMap.Add(typeof(DateTime));
            _simleTypeMap.Add(typeof(TimeSpan));
            _simleTypeMap.Add(typeof(Hashtable));
            _simleTypeMap.Add(typeof(SortedList));
            _simleTypeMap.Add(typeof(ListDictionary));
            _simleTypeMap.Add(typeof(HybridDictionary));
            _simleTypeMap.Add(typeof(ArrayList));
            _simleTypeMap.Add(typeof(IEnumerator));
        }

        private ReflectionInfo(Type type)
        {
            this._className = type.Name;
            this.AddMembers(type);
            string[] array = new string[this._getMembers.Count];
            this._getMembers.Keys.CopyTo(array, 0);
            this._readableMemberNames = array;
            string[] strArray2 = new string[this._setMembers.Count];
            this._setMembers.Keys.CopyTo(strArray2, 0);
            this._writeableMemberNames = strArray2;
        }

        private void AddMembers(Type type)
        {
            PropertyInfo[] properties = type.GetProperties(BINDING_FLAGS_PROPERTY);
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].CanWrite)
                {
                    string name = properties[i].Name;
                    this._setMembers[name] = properties[i];
                    this._setTypes[name] = properties[i].PropertyType;
                }
                if (properties[i].CanRead)
                {
                    string str2 = properties[i].Name;
                    this._getMembers[str2] = properties[i];
                    this._getTypes[str2] = properties[i].PropertyType;
                }
            }
            FieldInfo[] fields = type.GetFields(BINDING_FLAGS_FIELD);
            for (int j = 0; j < fields.Length; j++)
            {
                string str3 = fields[j].Name;
                this._setMembers[str3] = fields[j];
                this._setTypes[str3] = fields[j].FieldType;
                this._getMembers[str3] = fields[j];
                this._getTypes[str3] = fields[j].FieldType;
            }
            if (type.IsInterface)
            {
                foreach (Type type2 in type.GetInterfaces())
                {
                    this.AddMembers(type2);
                }
            }
        }

        public MemberInfo GetGetter(string memberName)
        {
            MemberInfo info = this._getMembers[memberName] as MemberInfo;
            if (info == null)
            {
                throw new ProbeException("There is no Get member named '" + memberName + "' in class '" + this._className + "'");
            }
            return info;
        }

        public Type GetGetterType(string memberName)
        {
            Type type = (Type) this._getTypes[memberName];
            if (type == null)
            {
                throw new ProbeException("There is no Get member named '" + memberName + "' in class '" + this._className + "'");
            }
            return type;
        }

        public static ReflectionInfo GetInstance(Type type)
        {
            lock (type)
            {
                ReflectionInfo info = (ReflectionInfo) _reflectionInfoMap[type];
                if (info == null)
                {
                    info = new ReflectionInfo(type);
                    _reflectionInfoMap.Add(type, info);
                }
                return info;
            }
        }

        public string[] GetReadableMemberNames()
        {
            return this._readableMemberNames;
        }

        public MemberInfo GetSetter(string memberName)
        {
            MemberInfo info = (MemberInfo) this._setMembers[memberName];
            if (info == null)
            {
                throw new ProbeException("There is no Set member named '" + memberName + "' in class '" + this._className + "'");
            }
            return info;
        }

        public Type GetSetterType(string memberName)
        {
            Type type = (Type) this._setTypes[memberName];
            if (type == null)
            {
                throw new ProbeException("There is no Set member named '" + memberName + "' in class '" + this._className + "'");
            }
            return type;
        }

        public string[] GetWriteableMemberNames()
        {
            return this._writeableMemberNames;
        }

        public bool HasReadableMember(string memberName)
        {
            return this._getMembers.ContainsKey(memberName);
        }

        public bool HasWritableMember(string memberName)
        {
            return this._setMembers.ContainsKey(memberName);
        }

        public static bool IsKnownType(Type type)
        {
            return (_simleTypeMap.Contains(type) || (typeof(IList).IsAssignableFrom(type) || (typeof(IDictionary).IsAssignableFrom(type) || typeof(IEnumerator).IsAssignableFrom(type))));
        }

        public string ClassName
        {
            get
            {
                return this._className;
            }
        }
    }
}

