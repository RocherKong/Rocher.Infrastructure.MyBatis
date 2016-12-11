namespace IBatisNet.Common.Utilities
{
    using IBatisNet.Common;
    using IBatisNet.Common.Exceptions;
    using System;
    using System.Collections;
    using System.Data;
    using System.Reflection;

    public sealed class DBHelperParameterCache
    {
        private Hashtable _paramCache = Hashtable.Synchronized(new Hashtable());

        public void CacheParameterSet(string connectionString, string commandText, params IDataParameter[] commandParameters)
        {
            string str = connectionString + ":" + commandText;
            this._paramCache[str] = commandParameters;
        }

        public void Clear()
        {
            this._paramCache.Clear();
        }

        private IDataParameter[] CloneParameters(IDataParameter[] originalParameters)
        {
            IDataParameter[] parameterArray = new IDataParameter[originalParameters.Length];
            int length = originalParameters.Length;
            int index = 0;
            int num3 = length;
            while (index < num3)
            {
                parameterArray[index] = (IDataParameter) ((ICloneable) originalParameters[index]).Clone();
                index++;
            }
            return parameterArray;
        }

        private void DeriveParameters(IDbProvider provider, IDbCommand command)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            if ((provider.CommandBuilderClass == null) || (provider.CommandBuilderClass.Length < 1))
            {
                throw new Exception(string.Format("CommandBuilderClass not defined for provider \"{0}\".", provider.Name));
            }
            Type commandBuilderType = provider.CommandBuilderType;
            try
            {
                commandBuilderType.InvokeMember("DeriveParameters", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new object[] { command });
            }
            catch (Exception exception)
            {
                throw new IBatisNetException("Could not retrieve parameters for the store procedure named " + command.CommandText, exception);
            }
        }

        private IDataParameter[] DiscoverSpParameterSet(IDalSession session, string spName, bool includeReturnValueParameter)
        {
            return this.InternalDiscoverSpParameterSet(session, spName, includeReturnValueParameter);
        }

        public IDataParameter[] GetCachedParameterSet(string connectionString, string commandText)
        {
            string str = connectionString + ":" + commandText;
            IDataParameter[] originalParameters = (IDataParameter[]) this._paramCache[str];
            if (originalParameters == null)
            {
                return null;
            }
            return this.CloneParameters(originalParameters);
        }

        public IDataParameter[] GetSpParameterSet(IDalSession session, string spName)
        {
            return this.GetSpParameterSet(session, spName, false);
        }

        public IDataParameter[] GetSpParameterSet(IDalSession session, string spName, bool includeReturnValueParameter)
        {
            string str = session.DataSource.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");
            IDataParameter[] originalParameters = (IDataParameter[]) this._paramCache[str];
            if (originalParameters == null)
            {
                this._paramCache[str] = this.DiscoverSpParameterSet(session, spName, includeReturnValueParameter);
                originalParameters = (IDataParameter[]) this._paramCache[str];
            }
            return this.CloneParameters(originalParameters);
        }

        private IDataParameter[] InternalDiscoverSpParameterSet(IDalSession session, string spName, bool includeReturnValueParameter)
        {
            using (IDbCommand command = session.CreateCommand(CommandType.StoredProcedure))
            {
                command.CommandText = spName;
                session.OpenConnection();
                this.DeriveParameters(session.DataSource.DbProvider, command);
                if (command.Parameters.Count > 0)
                {
                    IDataParameter parameter = (IDataParameter) command.Parameters[0];
                    if ((parameter.Direction == ParameterDirection.ReturnValue) && !includeReturnValueParameter)
                    {
                        command.Parameters.RemoveAt(0);
                    }
                }
                IDataParameter[] array = new IDataParameter[command.Parameters.Count];
                command.Parameters.CopyTo(array, 0);
                return array;
            }
        }
    }
}

