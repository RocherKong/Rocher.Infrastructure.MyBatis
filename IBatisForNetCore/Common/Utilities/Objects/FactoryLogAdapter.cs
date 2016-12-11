namespace IBatisNet.Common.Utilities.Objects
{
    using IBatisNet.Common.Logging;
    using System;
    using System.Text;

    public class FactoryLogAdapter : IFactory
    {
        private IFactory _factory;
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private string _parametersTypeName = string.Empty;
        private string _typeName = string.Empty;

        public FactoryLogAdapter(Type type, Type[] paramtersTypes, IFactory factory)
        {
            this._factory = factory;
            this._typeName = type.FullName;
            this._parametersTypeName = this.GenerateParametersName(paramtersTypes);
        }

        public object CreateInstance(object[] parameters)
        {
            object obj2 = null;
            try
            {
                obj2 = this._factory.CreateInstance(parameters);
            }
            catch
            {
                _logger.Debug("Enabled to create instance for type '" + this._typeName);
                _logger.Debug("  using parameters type : " + this._parametersTypeName);
                _logger.Debug("  using parameters value : " + this.GenerateLogInfoForParameterValue(parameters));
                throw;
            }
            return obj2;
        }

        private string GenerateLogInfoForParameterValue(object[] arguments)
        {
            StringBuilder builder = new StringBuilder();
            if ((arguments != null) && (arguments.Length != 0))
            {
                for (int i = 0; i < arguments.Length; i++)
                {
                    if (arguments[i] != null)
                    {
                        builder.Append("[").Append(arguments[i].ToString()).Append("] ");
                    }
                    else
                    {
                        builder.Append("[null] ");
                    }
                }
            }
            return builder.ToString();
        }

        private string GenerateParametersName(object[] arguments)
        {
            StringBuilder builder = new StringBuilder();
            if ((arguments != null) && (arguments.Length != 0))
            {
                for (int i = 0; i < arguments.Length; i++)
                {
                    builder.Append("[").Append(arguments[i]).Append("] ");
                }
            }
            return builder.ToString();
        }
    }
}

