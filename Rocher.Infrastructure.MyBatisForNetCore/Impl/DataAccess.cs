using IBatisNet.DataMapper;
using IBatisNet.DataMapper.MappedStatements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocher.Infrastructure.MyBatis
{
    public abstract class DataAccess
    {
        public DataAccess(string sqlMapConfigPath = "SqlMap.config")
        {
            this.SqlMapConfigPath = sqlMapConfigPath;
            this.InitSqlNameSpace();
        }

        public virtual ISqlMapSession BeginTransaction()
        {
            return this.SqlEntity.BeginTransaction();
        }

        public virtual void CommitTransaction()
        {
            this.SqlEntity.CommitTransaction();
        }


        protected abstract void InitSqlNameSpace();

        public ISqlMapper SqlEntity
        {
            get
            {
                return MapperContainer.GetInstance(this.SqlMapConfigPath);
            }
        }

        public string GetRuntimeSql(string StatementName, object reqParams)
        {
            IMappedStatement mappedStatement = this.SqlEntity.GetMappedStatement(StatementName);
            return mappedStatement.Statement.Sql.GetRequestScope(mappedStatement, reqParams, this.SqlEntity.LocalSession).PreparedStatement.PreparedSql;
        }

        public virtual void RollBackTransaction()
        {
            this.SqlEntity.RollBackTransaction();
        }

        protected string GetStatementName(string sqlId)
        {
            return (this.SqlNameSpace + "." + sqlId);
        }

        public string SqlMapConfigPath { get; private set; }

        public string SqlNameSpace { get; protected set; }
    }
}
