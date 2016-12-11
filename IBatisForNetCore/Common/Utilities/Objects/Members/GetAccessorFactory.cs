namespace IBatisNet.Common.Utilities.Objects.Members
{
    using IBatisNet.Common.Exceptions;
    using IBatisNet.Common.Utilities;
    using IBatisNet.Common.Utilities.Objects;
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Text;

    public class GetAccessorFactory : IGetAccessorFactory
    {
        private AssemblyBuilder _assemblyBuilder;
        private IDictionary _cachedIGetAccessor = new HybridDictionary();
        private CreateFieldGetAccessor _createFieldGetAccessor;
        private CreatePropertyGetAccessor _createPropertyGetAccessor;
        private ModuleBuilder _moduleBuilder;
        private object _syncObject = new object();

        public GetAccessorFactory(bool allowCodeGeneration)
        {
            if (allowCodeGeneration)
            {
                if (Environment.Version.Major >= 2)
                {
                    this._createPropertyGetAccessor = new CreatePropertyGetAccessor(this.CreateDynamicPropertyGetAccessor);
                    this._createFieldGetAccessor = new CreateFieldGetAccessor(this.CreateDynamicFieldGetAccessor);
                }
                else
                {
                    AssemblyName name = new AssemblyName {
                        Name = "iBATIS.FastGetAccessor" + HashCodeProvider.GetIdentityHashCode(this).ToString()
                    };
                    this._assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
                    this._moduleBuilder = this._assemblyBuilder.DefineDynamicModule(name.Name + ".dll");
                    this._createPropertyGetAccessor = new CreatePropertyGetAccessor(this.CreatePropertyAccessor);
                    this._createFieldGetAccessor = new CreateFieldGetAccessor(this.CreateFieldAccessor);
                }
            }
            else
            {
                this._createPropertyGetAccessor = new CreatePropertyGetAccessor(this.CreateReflectionPropertyGetAccessor);
                this._createFieldGetAccessor = new CreateFieldGetAccessor(this.CreateReflectionFieldGetAccessor);
            }
        }

        private IGetAccessor CreateDynamicFieldGetAccessor(Type targetType, string fieldName)
        {
            FieldInfo getter = (FieldInfo) ReflectionInfo.GetInstance(targetType).GetGetter(fieldName);
            if (getter.IsPublic)
            {
                return new DelegateFieldGetAccessor(targetType, fieldName);
            }
            return new ReflectionFieldGetAccessor(targetType, fieldName);
        }

        private IGetAccessor CreateDynamicPropertyGetAccessor(Type targetType, string propertyName)
        {
            PropertyInfo getter = (PropertyInfo) ReflectionInfo.GetInstance(targetType).GetGetter(propertyName);
            if (!getter.CanRead)
            {
                throw new NotSupportedException(string.Format("Property \"{0}\" on type {1} cannot be get.", getter.Name, targetType));
            }
            if (getter.GetGetMethod() != null)
            {
                return new DelegatePropertyGetAccessor(targetType, propertyName);
            }
            return new ReflectionPropertyGetAccessor(targetType, propertyName);
        }

        private IGetAccessor CreateFieldAccessor(Type targetType, string fieldName)
        {
            FieldInfo getter = (FieldInfo) ReflectionInfo.GetInstance(targetType).GetGetter(fieldName);
            if (getter.IsPublic)
            {
                return new EmitFieldGetAccessor(targetType, fieldName, this._assemblyBuilder, this._moduleBuilder);
            }
            return new ReflectionFieldGetAccessor(targetType, fieldName);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IGetAccessor CreateGetAccessor(Type targetType, string name)
        {
            string key = new StringBuilder(targetType.FullName).Append(".").Append(name).ToString();
            if (this._cachedIGetAccessor.Contains(key))
            {
                return (IGetAccessor) this._cachedIGetAccessor[key];
            }
            IGetAccessor accessor = null;
            lock (this._syncObject)
            {
                if (!this._cachedIGetAccessor.Contains(key))
                {
                    MemberInfo getter = ReflectionInfo.GetInstance(targetType).GetGetter(name);
                    if (getter == null)
                    {
                        throw new ProbeException(string.Format("No property or field named \"{0}\" exists for type {1}.", name, targetType));
                    }
                    if (getter is PropertyInfo)
                    {
                        accessor = this._createPropertyGetAccessor(targetType, name);
                        this._cachedIGetAccessor[key] = accessor;
                        return accessor;
                    }
                    accessor = this._createFieldGetAccessor(targetType, name);
                    this._cachedIGetAccessor[key] = accessor;
                    return accessor;
                }
                return (IGetAccessor) this._cachedIGetAccessor[key];
            }
        }

        private IGetAccessor CreatePropertyAccessor(Type targetType, string propertyName)
        {
            PropertyInfo getter = (PropertyInfo) ReflectionInfo.GetInstance(targetType).GetGetter(propertyName);
            if (!getter.CanRead)
            {
                throw new NotSupportedException(string.Format("Property \"{0}\" on type {1} cannot be get.", getter.Name, targetType));
            }
            if (getter.GetGetMethod() != null)
            {
                return new EmitPropertyGetAccessor(targetType, propertyName, this._assemblyBuilder, this._moduleBuilder);
            }
            return new ReflectionPropertyGetAccessor(targetType, propertyName);
        }

        private IGetAccessor CreateReflectionFieldGetAccessor(Type targetType, string fieldName)
        {
            return new ReflectionFieldGetAccessor(targetType, fieldName);
        }

        private IGetAccessor CreateReflectionPropertyGetAccessor(Type targetType, string propertyName)
        {
            return new ReflectionPropertyGetAccessor(targetType, propertyName);
        }

        private delegate IGetAccessor CreateFieldGetAccessor(Type targetType, string fieldName);

        private delegate IGetAccessor CreatePropertyGetAccessor(Type targetType, string propertyName);
    }
}

