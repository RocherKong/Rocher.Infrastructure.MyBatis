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

    public class SetAccessorFactory : ISetAccessorFactory
    {
        private AssemblyBuilder _assemblyBuilder;
        private IDictionary _cachedISetAccessor = new HybridDictionary();
        private CreateFieldSetAccessor _createFieldSetAccessor;
        private CreatePropertySetAccessor _createPropertySetAccessor;
        private ModuleBuilder _moduleBuilder;
        private object _syncObject = new object();

        public SetAccessorFactory(bool allowCodeGeneration)
        {
            if (allowCodeGeneration)
            {
                if (Environment.Version.Major >= 2)
                {
                    this._createPropertySetAccessor = new CreatePropertySetAccessor(this.CreateDynamicPropertySetAccessor);
                    this._createFieldSetAccessor = new CreateFieldSetAccessor(this.CreateDynamicFieldSetAccessor);
                }
                else
                {
                    AssemblyName name = new AssemblyName {
                        Name = "iBATIS.FastSetAccessor" + HashCodeProvider.GetIdentityHashCode(this).ToString()
                    };
                    this._assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
                    this._moduleBuilder = this._assemblyBuilder.DefineDynamicModule(name.Name + ".dll");
                    this._createPropertySetAccessor = new CreatePropertySetAccessor(this.CreatePropertyAccessor);
                    this._createFieldSetAccessor = new CreateFieldSetAccessor(this.CreateFieldAccessor);
                }
            }
            else
            {
                this._createPropertySetAccessor = new CreatePropertySetAccessor(this.CreateReflectionPropertySetAccessor);
                this._createFieldSetAccessor = new CreateFieldSetAccessor(this.CreateReflectionFieldSetAccessor);
            }
        }

        private ISetAccessor CreateDynamicFieldSetAccessor(Type targetType, string fieldName)
        {
            FieldInfo setter = (FieldInfo) ReflectionInfo.GetInstance(targetType).GetSetter(fieldName);
            if (setter.IsPublic)
            {
                return new DelegateFieldSetAccessor(targetType, fieldName);
            }
            return new ReflectionFieldSetAccessor(targetType, fieldName);
        }

        private ISetAccessor CreateDynamicPropertySetAccessor(Type targetType, string propertyName)
        {
            PropertyInfo setter = (PropertyInfo) ReflectionInfo.GetInstance(targetType).GetSetter(propertyName);
            if (!setter.CanWrite)
            {
                throw new NotSupportedException(string.Format("Property \"{0}\" on type {1} cannot be set.", setter.Name, targetType));
            }
            if (setter.GetSetMethod() != null)
            {
                return new DelegatePropertySetAccessor(targetType, propertyName);
            }
            return new ReflectionPropertySetAccessor(targetType, propertyName);
        }

        private ISetAccessor CreateFieldAccessor(Type targetType, string fieldName)
        {
            FieldInfo setter = (FieldInfo) ReflectionInfo.GetInstance(targetType).GetSetter(fieldName);
            if (setter.IsPublic)
            {
                return new EmitFieldSetAccessor(targetType, fieldName, this._assemblyBuilder, this._moduleBuilder);
            }
            return new ReflectionFieldSetAccessor(targetType, fieldName);
        }

        private ISetAccessor CreatePropertyAccessor(Type targetType, string propertyName)
        {
            PropertyInfo setter = (PropertyInfo) ReflectionInfo.GetInstance(targetType).GetSetter(propertyName);
            if (!setter.CanWrite)
            {
                throw new NotSupportedException(string.Format("Property \"{0}\" on type {1} cannot be set.", setter.Name, targetType));
            }
            if (setter.GetSetMethod() != null)
            {
                return new EmitPropertySetAccessor(targetType, propertyName, this._assemblyBuilder, this._moduleBuilder);
            }
            return new ReflectionPropertySetAccessor(targetType, propertyName);
        }

        private ISetAccessor CreateReflectionFieldSetAccessor(Type targetType, string fieldName)
        {
            return new ReflectionFieldSetAccessor(targetType, fieldName);
        }

        private ISetAccessor CreateReflectionPropertySetAccessor(Type targetType, string propertyName)
        {
            return new ReflectionPropertySetAccessor(targetType, propertyName);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ISetAccessor CreateSetAccessor(Type targetType, string name)
        {
            string key = new StringBuilder(targetType.FullName).Append(".").Append(name).ToString();
            if (this._cachedISetAccessor.Contains(key))
            {
                return (ISetAccessor) this._cachedISetAccessor[key];
            }
            ISetAccessor accessor = null;
            lock (this._syncObject)
            {
                if (!this._cachedISetAccessor.Contains(key))
                {
                    MemberInfo setter = ReflectionInfo.GetInstance(targetType).GetSetter(name);
                    if (setter == null)
                    {
                        throw new ProbeException(string.Format("No property or field named \"{0}\" exists for type {1}.", name, targetType));
                    }
                    if (setter is PropertyInfo)
                    {
                        accessor = this._createPropertySetAccessor(targetType, name);
                        this._cachedISetAccessor[key] = accessor;
                        return accessor;
                    }
                    accessor = this._createFieldSetAccessor(targetType, name);
                    this._cachedISetAccessor[key] = accessor;
                    return accessor;
                }
                return (ISetAccessor) this._cachedISetAccessor[key];
            }
        }

        private delegate ISetAccessor CreateFieldSetAccessor(Type targetType, string fieldName);

        private delegate ISetAccessor CreatePropertySetAccessor(Type targetType, string propertyName);
    }
}

