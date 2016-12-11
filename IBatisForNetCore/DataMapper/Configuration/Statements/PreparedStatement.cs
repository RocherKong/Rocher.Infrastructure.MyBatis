namespace IBatisNet.DataMapper.Configuration.Statements
{
    using System;
    using System.Collections.Specialized;
    using System.Data;

    public class PreparedStatement
    {
        private IDbDataParameter[] _dbParameters;
        private StringCollection _dbParametersName = new StringCollection();
        private string _preparedSsql = string.Empty;

        public IDbDataParameter[] DbParameters
        {
            get
            {
                return this._dbParameters;
            }
            set
            {
                this._dbParameters = value;
            }
        }

        public StringCollection DbParametersName
        {
            get
            {
                return this._dbParametersName;
            }
        }

        public string PreparedSql
        {
            get
            {
                return this._preparedSsql;
            }
            set
            {
                this._preparedSsql = value;
            }
        }
    }
}

