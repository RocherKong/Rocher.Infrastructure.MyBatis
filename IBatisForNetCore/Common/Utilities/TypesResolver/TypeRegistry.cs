namespace IBatisNet.Common.Utilities.TypesResolver
{
    using System;
    using System.Collections;

    public class TypeRegistry
    {
        private static IDictionary _types = new Hashtable();
        public const string ArrayListAlias1 = "arraylist";
        public const string ArrayListAlias2 = "list";
        public const string BoolAlias = "bool";
        public const string BooleanAlias = "boolean";
        public const string ByteAlias = "byte";
        public const string CharAlias = "char";
        public const string DateAlias1 = "datetime";
        public const string DateAlias2 = "date";
        public const string DecimalAlias = "decimal";
        public const string DoubleAlias = "double";
        public const string FloatAlias = "float";
        public const string GuidAlias = "guid";
        public const string HashtableAlias1 = "hashtable";
        public const string HashtableAlias2 = "map";
        public const string HashtableAlias3 = "hashmap";
        public const string Int16Alias1 = "int16";
        public const string Int16Alias2 = "short";
        public const string Int32Alias1 = "int32";
        public const string Int32Alias2 = "int";
        public const string Int32Alias3 = "integer";
        public const string Int64Alias1 = "int64";
        public const string Int64Alias2 = "long";
        public const string NullableBoolAlias = "bool?";
        public const string NullableBoolArrayAlias = "bool?[]";
        public const string NullableCharAlias = "char?";
        public const string NullableCharArrayAlias = "char?[]";
        public const string NullableDecimalAlias = "decimal?";
        public const string NullableDecimalArrayAlias = "decimal?[]";
        public const string NullableDoubleAlias = "double?";
        public const string NullableDoubleArrayAlias = "double?[]";
        public const string NullableFloatAlias = "float?";
        public const string NullableFloatArrayAlias = "float?[]";
        public const string NullableInt16Alias = "short?";
        public const string NullableInt16ArrayAlias = "short?[]";
        public const string NullableInt32Alias = "int?";
        public const string NullableInt32ArrayAlias = "int?[]";
        public const string NullableInt64Alias = "long?";
        public const string NullableInt64ArrayAlias = "long?[]";
        public const string NullableUInt16Alias = "ushort?";
        public const string NullableUInt16ArrayAlias = "ushort?[]";
        public const string NullableUInt32Alias = "uint?";
        public const string NullableUInt32ArrayAlias = "uint?[]";
        public const string NullableUInt64Alias = "ulong?";
        public const string NullableUInt64ArrayAlias = "ulong?[]";
        public const string SByteAlias = "sbyte";
        public const string SingleAlias = "single";
        public const string StringAlias = "string";
        public const string TimeSpanAlias = "timespan";
        public const string UInt16Alias1 = "uint16";
        public const string UInt16Alias2 = "ushort";
        public const string UInt32Alias1 = "uint32";
        public const string UInt32Alias2 = "uint";
        public const string UInt64Alias1 = "uint64";
        public const string UInt64Alias2 = "ulong";

        static TypeRegistry()
        {
            _types["arraylist"] = typeof(ArrayList);
            _types["list"] = typeof(ArrayList);
            _types["bool"] = typeof(bool);
            _types["boolean"] = typeof(bool);
            _types["byte"] = typeof(byte);
            _types["char"] = typeof(char);
            _types["datetime"] = typeof(DateTime);
            _types["date"] = typeof(DateTime);
            _types["decimal"] = typeof(decimal);
            _types["double"] = typeof(double);
            _types["float"] = typeof(float);
            _types["single"] = typeof(float);
            _types["guid"] = typeof(Guid);
            _types["hashtable"] = typeof(Hashtable);
            _types["map"] = typeof(Hashtable);
            _types["hashmap"] = typeof(Hashtable);
            _types["int16"] = typeof(short);
            _types["short"] = typeof(short);
            _types["int32"] = typeof(int);
            _types["int"] = typeof(int);
            _types["integer"] = typeof(int);
            _types["int64"] = typeof(long);
            _types["long"] = typeof(long);
            _types["uint16"] = typeof(ushort);
            _types["ushort"] = typeof(ushort);
            _types["uint32"] = typeof(uint);
            _types["uint"] = typeof(uint);
            _types["uint64"] = typeof(ulong);
            _types["ulong"] = typeof(ulong);
            _types["sbyte"] = typeof(sbyte);
            _types["string"] = typeof(string);
            _types["timespan"] = typeof(string);
            _types["int?"] = typeof(int?);
            _types["int?[]"] = typeof(int?[]);
            _types["decimal?"] = typeof(decimal?);
            _types["decimal?[]"] = typeof(decimal?[]);
            _types["char?"] = typeof(char?);
            _types["char?[]"] = typeof(char?[]);
            _types["long?"] = typeof(long?);
            _types["long?[]"] = typeof(long?[]);
            _types["short?"] = typeof(short?);
            _types["short?[]"] = typeof(short?[]);
            _types["uint?"] = typeof(uint?);
            _types["uint?[]"] = typeof(uint?[]);
            _types["ulong?"] = typeof(ulong?);
            _types["ulong?[]"] = typeof(ulong?[]);
            _types["ushort?"] = typeof(ushort?);
            _types["ushort?[]"] = typeof(ushort?[]);
            _types["double?"] = typeof(double?);
            _types["double?[]"] = typeof(double?[]);
            _types["float?"] = typeof(float?);
            _types["float?[]"] = typeof(float?[]);
            _types["bool?"] = typeof(bool?);
            _types["bool?[]"] = typeof(bool?[]);
        }

        private TypeRegistry()
        {
        }

        public static Type ResolveType(string alias)
        {
            return (Type) _types[alias.ToLower()];
        }
    }
}

