namespace IBatisNet.Common.Utilities.Objects
{
    using IBatisNet.Common.Exceptions;
    using IBatisNet.Common.Utilities;
    using IBatisNet.Common.Utilities.Objects.Members;
    using System;
    using System.Collections;
    using System.Reflection;

    public sealed class ObjectProbe
    {
        private static ArrayList _simpleTypeMap = new ArrayList();

        static ObjectProbe()
        {
            _simpleTypeMap.Add(typeof(string));
            _simpleTypeMap.Add(typeof(byte));
            _simpleTypeMap.Add(typeof(short));
            _simpleTypeMap.Add(typeof(char));
            _simpleTypeMap.Add(typeof(int));
            _simpleTypeMap.Add(typeof(long));
            _simpleTypeMap.Add(typeof(float));
            _simpleTypeMap.Add(typeof(double));
            _simpleTypeMap.Add(typeof(bool));
            _simpleTypeMap.Add(typeof(DateTime));
            _simpleTypeMap.Add(typeof(decimal));
            _simpleTypeMap.Add(typeof(sbyte));
            _simpleTypeMap.Add(typeof(ushort));
            _simpleTypeMap.Add(typeof(uint));
            _simpleTypeMap.Add(typeof(ulong));
            _simpleTypeMap.Add(typeof(IEnumerator));
        }

        private static object GetArrayMember(object obj, string indexedName, AccessorFactory accessorFactory)
        {
            object obj2 = null;
            try
            {
                int index = indexedName.IndexOf("[");
                int num2 = indexedName.IndexOf("]");
                string memberName = indexedName.Substring(0, index);
                int num3 = Convert.ToInt32(indexedName.Substring(index + 1, num2 - (index + 1)));
                if (memberName.Length > 0)
                {
                    obj2 = GetMember(obj, memberName, accessorFactory);
                }
                else
                {
                    obj2 = obj;
                }
                if (!(obj2 is IList))
                {
                    throw new ProbeException("The '" + memberName + "' member of the " + obj.GetType().Name + " class is not a List or Array.");
                }
                return ((IList) obj2)[num3];
            }
            catch (ProbeException exception)
            {
                throw exception;
            }
            catch (Exception exception2)
            {
                throw new ProbeException("Error getting ordinal value from .net object. Cause" + exception2.Message, exception2);
            }
            return obj2;
        }

        public static object GetMember(object obj, string memberName, AccessorFactory accessorFactory)
        {
            object obj3;
            try
            {
                object obj2 = null;
                if (memberName.IndexOf("[") > -1)
                {
                    obj2 = GetArrayMember(obj, memberName, accessorFactory);
                }
                else if (obj is IDictionary)
                {
                    obj2 = ((IDictionary) obj)[memberName];
                }
                else
                {
                    Type targetType = obj.GetType();
                    IGetAccessor accessor = accessorFactory.GetAccessorFactory.CreateGetAccessor(targetType, memberName);
                    if (accessor == null)
                    {
                        throw new ProbeException("No Get method for member " + memberName + " on instance of " + obj.GetType().Name);
                    }
                    try
                    {
                        obj2 = accessor.Get(obj);
                    }
                    catch (Exception exception)
                    {
                        throw new ProbeException(exception);
                    }
                }
                obj3 = obj2;
            }
            catch (ProbeException exception2)
            {
                throw exception2;
            }
            catch (Exception exception3)
            {
                throw new ProbeException("Could not Set property '" + memberName + "' for " + obj.GetType().Name + ".  Cause: " + exception3.Message, exception3);
            }
            return obj3;
        }

        public static MemberInfo GetMemberInfoForSetter(Type type, string memberName)
        {
            if (memberName.IndexOf('.') > -1)
            {
                IEnumerator enumerator = new StringTokenizer(memberName, ".").GetEnumerator();
                Type type2 = null;
                while (enumerator.MoveNext())
                {
                    memberName = (string) enumerator.Current;
                    type2 = type;
                    type = ReflectionInfo.GetInstance(type).GetSetterType(memberName);
                }
                return ReflectionInfo.GetInstance(type2).GetSetter(memberName);
            }
            return ReflectionInfo.GetInstance(type).GetSetter(memberName);
        }

        public static Type GetMemberTypeForGetter(object obj, string memberName)
        {
            Type getterType = obj.GetType();
            if (obj is IDictionary)
            {
                IDictionary dictionary = (IDictionary) obj;
                object obj2 = dictionary[memberName];
                if (obj2 == null)
                {
                    return typeof(object);
                }
                return obj2.GetType();
            }
            if (memberName.IndexOf('.') > -1)
            {
                IEnumerator enumerator = new StringTokenizer(memberName, ".").GetEnumerator();
                while (enumerator.MoveNext())
                {
                    memberName = (string) enumerator.Current;
                    getterType = ReflectionInfo.GetInstance(getterType).GetGetterType(memberName);
                }
                return getterType;
            }
            return ReflectionInfo.GetInstance(getterType).GetGetterType(memberName);
        }

        public static Type GetMemberTypeForGetter(Type type, string memberName)
        {
            if (memberName.IndexOf('.') > -1)
            {
                IEnumerator enumerator = new StringTokenizer(memberName, ".").GetEnumerator();
                while (enumerator.MoveNext())
                {
                    memberName = (string) enumerator.Current;
                    type = ReflectionInfo.GetInstance(type).GetGetterType(memberName);
                }
                return type;
            }
            type = ReflectionInfo.GetInstance(type).GetGetterType(memberName);
            return type;
        }

        public static Type GetMemberTypeForSetter(object obj, string memberName)
        {
            Type setterType = obj.GetType();
            if (obj is IDictionary)
            {
                IDictionary dictionary = (IDictionary) obj;
                object obj2 = dictionary[memberName];
                if (obj2 == null)
                {
                    return typeof(object);
                }
                return obj2.GetType();
            }
            if (memberName.IndexOf('.') > -1)
            {
                IEnumerator enumerator = new StringTokenizer(memberName, ".").GetEnumerator();
                while (enumerator.MoveNext())
                {
                    memberName = (string) enumerator.Current;
                    setterType = ReflectionInfo.GetInstance(setterType).GetSetterType(memberName);
                }
                return setterType;
            }
            return ReflectionInfo.GetInstance(setterType).GetSetterType(memberName);
        }

        public static Type GetMemberTypeForSetter(Type type, string memberName)
        {
            Type setterType = type;
            if (memberName.IndexOf('.') > -1)
            {
                IEnumerator enumerator = new StringTokenizer(memberName, ".").GetEnumerator();
                while (enumerator.MoveNext())
                {
                    memberName = (string) enumerator.Current;
                    setterType = ReflectionInfo.GetInstance(setterType).GetSetterType(memberName);
                }
                return setterType;
            }
            return ReflectionInfo.GetInstance(type).GetSetterType(memberName);
        }

        public static object GetMemberValue(object obj, string memberName, AccessorFactory accessorFactory)
        {
            if (memberName.IndexOf('.') <= -1)
            {
                return GetMember(obj, memberName, accessorFactory);
            }
            IEnumerator enumerator = new StringTokenizer(memberName, ".").GetEnumerator();
            object obj2 = obj;
            string current = null;
            while (enumerator.MoveNext())
            {
                current = (string) enumerator.Current;
                obj2 = GetMember(obj2, current, accessorFactory);
                if (obj2 == null)
                {
                    return obj2;
                }
            }
            return obj2;
        }

        public static string[] GetReadablePropertyNames(object obj)
        {
            return ReflectionInfo.GetInstance(obj.GetType()).GetReadableMemberNames();
        }

        public static string[] GetWriteableMemberNames(object obj)
        {
            return ReflectionInfo.GetInstance(obj.GetType()).GetWriteableMemberNames();
        }

        public static bool HasReadableProperty(object obj, string propertyName)
        {
            bool flag = false;
            if (obj is IDictionary)
            {
                return ((IDictionary) obj).Contains(propertyName);
            }
            if (propertyName.IndexOf('.') > -1)
            {
                IEnumerator enumerator = new StringTokenizer(propertyName, ".").GetEnumerator();
                Type getterType = obj.GetType();
                while (enumerator.MoveNext())
                {
                    propertyName = (string) enumerator.Current;
                    getterType = ReflectionInfo.GetInstance(getterType).GetGetterType(propertyName);
                    flag = ReflectionInfo.GetInstance(getterType).HasReadableMember(propertyName);
                }
                return flag;
            }
            return ReflectionInfo.GetInstance(obj.GetType()).HasReadableMember(propertyName);
        }

        public static bool HasWritableProperty(object obj, string propertyName)
        {
            bool flag = false;
            if (obj is IDictionary)
            {
                return ((IDictionary) obj).Contains(propertyName);
            }
            if (propertyName.IndexOf('.') > -1)
            {
                IEnumerator enumerator = new StringTokenizer(propertyName, ".").GetEnumerator();
                Type getterType = obj.GetType();
                while (enumerator.MoveNext())
                {
                    propertyName = (string) enumerator.Current;
                    getterType = ReflectionInfo.GetInstance(getterType).GetGetterType(propertyName);
                    flag = ReflectionInfo.GetInstance(getterType).HasWritableMember(propertyName);
                }
                return flag;
            }
            return ReflectionInfo.GetInstance(obj.GetType()).HasWritableMember(propertyName);
        }

        public static bool IsSimpleType(Type type)
        {
            return (_simpleTypeMap.Contains(type) || (typeof(ICollection).IsAssignableFrom(type) || (typeof(IDictionary).IsAssignableFrom(type) || (typeof(IList).IsAssignableFrom(type) || typeof(IEnumerable).IsAssignableFrom(type)))));
        }

        private static void SetArrayMember(object obj, string indexedName, object value, AccessorFactory accessorFactory)
        {
            try
            {
                int index = indexedName.IndexOf("[");
                int num2 = indexedName.IndexOf("]");
                string memberName = indexedName.Substring(0, index);
                int num3 = Convert.ToInt32(indexedName.Substring(index + 1, num2 - (index + 1)));
                object obj2 = null;
                if (memberName.Length > 0)
                {
                    obj2 = GetMember(obj, memberName, accessorFactory);
                }
                else
                {
                    obj2 = obj;
                }
                if (!(obj2 is IList))
                {
                    throw new ProbeException("The '" + memberName + "' member of the " + obj.GetType().Name + " class is not a List or Array.");
                }
                ((IList) obj2)[num3] = value;
            }
            catch (ProbeException exception)
            {
                throw exception;
            }
            catch (Exception exception2)
            {
                throw new ProbeException("Error getting ordinal value from .net object. Cause" + exception2.Message, exception2);
            }
        }

        public static void SetMember(object obj, string memberName, object memberValue, AccessorFactory accessorFactory)
        {
            try
            {
                if (memberName.IndexOf("[") > -1)
                {
                    SetArrayMember(obj, memberName, memberValue, accessorFactory);
                }
                else if (obj is IDictionary)
                {
                    ((IDictionary) obj)[memberName] = memberValue;
                }
                else
                {
                    Type targetType = obj.GetType();
                    ISetAccessorFactory setAccessorFactory = accessorFactory.SetAccessorFactory;
                    if (setAccessorFactory.CreateSetAccessor(targetType, memberName) == null)
                    {
                        throw new ProbeException("No Set method for member " + memberName + " on instance of " + obj.GetType().Name);
                    }
                    try
                    {
                        setAccessorFactory.CreateSetAccessor(targetType, memberName).Set(obj, memberValue);
                    }
                    catch (Exception exception)
                    {
                        throw new ProbeException(exception);
                    }
                }
            }
            catch (ProbeException exception2)
            {
                throw exception2;
            }
            catch (Exception exception3)
            {
                throw new ProbeException("Could not Get property '" + memberName + "' for " + obj.GetType().Name + ".  Cause: " + exception3.Message, exception3);
            }
        }

        public static void SetMemberValue(object obj, string memberName, object memberValue, IObjectFactory objectFactory, AccessorFactory accessorFactory)
        {
            if (memberName.IndexOf('.') > -1)
            {
                IEnumerator enumerator = new StringTokenizer(memberName, ".").GetEnumerator();
                enumerator.MoveNext();
                string current = (string) enumerator.Current;
                object obj2 = obj;
                while (enumerator.MoveNext())
                {
                    Type memberTypeForSetter = GetMemberTypeForSetter(obj2, current);
                    object obj3 = obj2;
                    obj2 = GetMember(obj3, current, accessorFactory);
                    if (obj2 == null)
                    {
                        try
                        {
                            obj2 = objectFactory.CreateFactory(memberTypeForSetter, Type.EmptyTypes).CreateInstance(Type.EmptyTypes);
                            SetMemberValue(obj3, current, obj2, objectFactory, accessorFactory);
                        }
                        catch (Exception exception)
                        {
                            throw new ProbeException("Cannot set value of property '" + memberName + "' because '" + current + "' is null and cannot be instantiated on instance of " + memberTypeForSetter.Name + ". Cause:" + exception.Message, exception);
                        }
                    }
                    current = (string) enumerator.Current;
                }
                SetMember(obj2, current, memberValue, accessorFactory);
            }
            else
            {
                SetMember(obj, memberName, memberValue, accessorFactory);
            }
        }
    }
}

