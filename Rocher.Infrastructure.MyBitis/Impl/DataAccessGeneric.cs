using Rocher.Infrastructure.MyBatis.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocher.Infrastructure.MyBatis
{
    public class DataAccessGeneric<TEntity> : QueryDataAccessGeneric<TEntity>, IDataAccess<TEntity>, IQueryDataAccess<TEntity> where TEntity : class
    {
        public DataAccessGeneric(string sqlMapConfigPath = "SqlMap.config") : base(sqlMapConfigPath)
        {
            this. PrimaryKeyName = "Id";
        }

        public virtual int Delete<TPrimary>(TPrimary Id)
        {
            string delete = DefaultCommand.Delete;
            Hashtable parameterObject = new Hashtable();
            parameterObject.Add(this.PrimaryKeyName, Id);
            if (this.IsPhysicalDeletion)
            {
                return base.SqlEntity.Delete(base.GetStatementName(delete), parameterObject);
            }
            return base.SqlEntity.Update(base.GetStatementName(delete), parameterObject);
        }

        public virtual int DeleteList(string Ids)
        {
            string deleteList = DefaultCommand.DeleteList;
            Hashtable parameterObject = new Hashtable();
            char[] separator = new char[] { ',' };
            parameterObject.Add(this.PrimaryKeyName + "s", Ids.Split(separator, StringSplitOptions.RemoveEmptyEntries));
            if (this.IsPhysicalDeletion)
            {
                return base.SqlEntity.Delete(base.GetStatementName(deleteList), parameterObject);
            }
            return base.SqlEntity.Update(base.GetStatementName(deleteList), parameterObject);
        }

        public virtual TPrimary Insert<TPrimary>(TEntity entity)
        {
            string insert = DefaultCommand.Insert;
            if (typeof(TPrimary) == typeof(NonePrimaryResponse))
            {
                base.SqlEntity.Insert(base.GetStatementName(insert), entity);
                return default(TPrimary);
            }
            return (TPrimary)base.SqlEntity.Insert(base.GetStatementName(insert), entity);
        }

        public virtual int Update(TEntity entity)
        {
            string update = DefaultCommand.Update;
            return base.SqlEntity.Update(base.GetStatementName(update), entity);
        }

        protected bool IsPhysicalDeletion { get; set; }

        protected string PrimaryKeyName { get; set; }
    }
}
