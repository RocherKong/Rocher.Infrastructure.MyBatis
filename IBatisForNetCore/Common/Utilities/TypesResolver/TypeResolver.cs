namespace IBatisNet.Common.Utilities.TypesResolver
{
    using IBatisNet.Common.Utilities;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;

    public class TypeResolver : ITypeResolver
    {
        private const string NULLABLE_TYPE = "System.Nullable";

        private static TypeLoadException BuildTypeLoadException(string typeName)
        {
            return new TypeLoadException("Could not load type from string value '" + typeName + "'.");
        }

        private static TypeLoadException BuildTypeLoadException(string typeName, Exception ex)
        {
            return new TypeLoadException("Could not load type from string value '" + typeName + "'.", ex);
        }

        private static Type LoadTypeByIteratingOverAllLoadedAssemblies(TypeAssemblyInfo typeInfo)
        {
            Type type = null;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(typeInfo.TypeName, false, false);
                if (type != null)
                {
                    return type;
                }
            }
            return type;
        }

        private static Type LoadTypeDirectlyFromAssembly(TypeAssemblyInfo typeInfo)
        {
            Type type = null;
            Assembly assembly = null;
            assembly = Assembly.Load(typeInfo.AssemblyName);
            if (assembly != null)
            {
                type = assembly.GetType(typeInfo.TypeName, true, true);
            }
            return type;
        }

        public virtual Type Resolve(string typeName)
        {
            Type type = this.ResolveGenericType(typeName.Replace(" ", string.Empty));
            if (type == null)
            {
                type = this.ResolveType(typeName.Replace(" ", string.Empty));
            }
            return type;
        }

        private Type ResolveGenericType(string typeName)
        {
            if ((typeName == null) || (typeName.Trim().Length == 0))
            {
                throw BuildTypeLoadException(typeName);
            }
            if (typeName.StartsWith("System.Nullable"))
            {
                return null;
            }
            GenericArgumentsInfo info = new GenericArgumentsInfo(typeName);
            Type type = null;
            try
            {
                if (!info.ContainsGenericArguments)
                {
                    return type;
                }
                type = TypeUtils.ResolveType(info.GenericTypeName);
                if (info.IsGenericDefinition)
                {
                    return type;
                }
                string[] genericArguments = info.GetGenericArguments();
                Type[] typeArguments = new Type[genericArguments.Length];
                for (int i = 0; i < genericArguments.Length; i++)
                {
                    typeArguments[i] = TypeUtils.ResolveType(genericArguments[i]);
                }
                type = type.MakeGenericType(typeArguments);
            }
            catch (Exception exception)
            {
                if (exception is TypeLoadException)
                {
                    throw;
                }
                throw BuildTypeLoadException(typeName, exception);
            }
            return type;
        }

        private Type ResolveType(string typeName)
        {
            if ((typeName == null) || (typeName.Trim().Length == 0))
            {
                throw BuildTypeLoadException(typeName);
            }
            TypeAssemblyInfo typeInfo = new TypeAssemblyInfo(typeName);
            Type type = null;
            try
            {
                type = typeInfo.IsAssemblyQualified ? LoadTypeDirectlyFromAssembly(typeInfo) : LoadTypeByIteratingOverAllLoadedAssemblies(typeInfo);
            }
            catch (Exception exception)
            {
                throw BuildTypeLoadException(typeName, exception);
            }
            if (type == null)
            {
                throw BuildTypeLoadException(typeName);
            }
            return type;
        }

        internal class GenericArgumentsInfo
        {
            private string[] _unresolvedGenericArguments;
            private string _unresolvedGenericTypeName = string.Empty;
            private static readonly Regex generic = new Regex(@"`\d*\[\[", RegexOptions.Compiled);
            public const string GENERIC_ARGUMENTS_PREFIX = "[[";
            public const string GENERIC_ARGUMENTS_SEPARATOR = "],[";
            public const string GENERIC_ARGUMENTS_SUFFIX = "]]";

            public GenericArgumentsInfo(string value)
            {
                this.ParseGenericArguments(value);
            }

            public string[] GetGenericArguments()
            {
                if (this._unresolvedGenericArguments == null)
                {
                    return new string[0];
                }
                return this._unresolvedGenericArguments;
            }

            private IList<string> Parse(string args)
            {
                StringBuilder builder = new StringBuilder();
                IList<string> list = new List<string>();
                TextReader reader = new StringReader(args);
                int num = 0;
                bool flag = false;
                do
                {
                    char ch = (char) reader.Read();
                    switch (ch)
                    {
                        case '[':
                            num++;
                            flag = true;
                            break;

                        case ']':
                            num--;
                            break;
                    }
                    builder.Append(ch);
                    if (flag && (num == 0))
                    {
                        string item = builder.ToString();
                        item = item.Substring(1, item.Length - 2);
                        list.Add(item);
                        reader.Read();
                        builder = new StringBuilder();
                    }
                }
                while (reader.Peek() != -1);
                return list;
            }

            private void ParseGenericArguments(string originalString)
            {
                if (!generic.IsMatch(originalString))
                {
                    this._unresolvedGenericTypeName = originalString;
                }
                else
                {
                    int index = originalString.IndexOf("[[");
                    int num2 = originalString.LastIndexOf("]]");
                    if (num2 != -1)
                    {
                        this.SplitGenericArguments(originalString.Substring(index + 1, num2 - index));
                        this._unresolvedGenericTypeName = originalString.Remove(index, (num2 - index) + 2);
                    }
                }
            }

            private void SplitGenericArguments(string originalArgs)
            {
                IList<string> list = new List<string>();
                if (originalArgs.Contains("],["))
                {
                    list = this.Parse(originalArgs);
                }
                else
                {
                    string item = originalArgs.Substring(1, originalArgs.Length - 2).Trim();
                    list.Add(item);
                }
                this._unresolvedGenericArguments = new string[list.Count];
                list.CopyTo(this._unresolvedGenericArguments, 0);
            }

            public bool ContainsGenericArguments
            {
                get
                {
                    return ((this._unresolvedGenericArguments != null) && (this._unresolvedGenericArguments.Length > 0));
                }
            }

            public string GenericTypeName
            {
                get
                {
                    return this._unresolvedGenericTypeName;
                }
            }

            public bool IsGenericDefinition
            {
                get
                {
                    if (this._unresolvedGenericArguments == null)
                    {
                        return false;
                    }
                    foreach (string str in this._unresolvedGenericArguments)
                    {
                        if (str.Length > 0)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
        }

        internal class TypeAssemblyInfo
        {
            private string _unresolvedAssemblyName = string.Empty;
            private string _unresolvedTypeName = string.Empty;
            public const string NULLABLE_TYPE = "System.Nullable";
            public const string NULLABLE_TYPE_ASSEMBLY_SEPARATOR = "]],";
            public const string TYPE_ASSEMBLY_SEPARATOR = ",";

            public TypeAssemblyInfo(string unresolvedTypeName)
            {
                this.SplitTypeAndAssemblyNames(unresolvedTypeName);
            }

            private bool HasLength(string target)
            {
                return ((target != null) && (target.Length > 0));
            }

            private bool HasText(string target)
            {
                if (target == null)
                {
                    return false;
                }
                return this.HasLength(target.Trim());
            }

            private void SplitTypeAndAssemblyNames(string originalTypeName)
            {
                if (originalTypeName.StartsWith("System.Nullable"))
                {
                    int index = originalTypeName.IndexOf("]],");
                    if (index < 0)
                    {
                        this._unresolvedTypeName = originalTypeName;
                    }
                    else
                    {
                        this._unresolvedTypeName = originalTypeName.Substring(0, index + 2).Trim();
                        this._unresolvedAssemblyName = originalTypeName.Substring(index + 3).Trim();
                    }
                }
                else
                {
                    int length = originalTypeName.IndexOf(",");
                    if (length < 0)
                    {
                        this._unresolvedTypeName = originalTypeName;
                    }
                    else
                    {
                        this._unresolvedTypeName = originalTypeName.Substring(0, length).Trim();
                        this._unresolvedAssemblyName = originalTypeName.Substring(length + 1).Trim();
                    }
                }
            }

            public string AssemblyName
            {
                get
                {
                    return this._unresolvedAssemblyName;
                }
            }

            public bool IsAssemblyQualified
            {
                get
                {
                    return this.HasText(this.AssemblyName);
                }
            }

            public string TypeName
            {
                get
                {
                    return this._unresolvedTypeName;
                }
            }
        }
    }
}

