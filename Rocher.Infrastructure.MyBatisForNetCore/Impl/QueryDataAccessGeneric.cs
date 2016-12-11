using Rocher.Infrastructure.MyBatis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Rocher.Infrastructure.MyBatis
{
    public class QueryDataAccessGeneric<TEntity> : DataAccess, IQueryDataAccess<TEntity> where TEntity : class
    {
        public QueryDataAccessGeneric(string sqlMapConfigPath = "SqlMap.config") : base(sqlMapConfigPath)
        {
        }

        public virtual TEntity GetEntity(Hashtable reqParams)
        {
            string getEntity = DefaultCommand.GetEntity;
            return base.SqlEntity.QueryForObject<TEntity>(base.GetStatementName(getEntity), reqParams);
        }

        public virtual TEntity GetEntity<TPrimary>(TPrimary Id)
        {
            Hashtable reqParams = new Hashtable();
            reqParams.Add("Id", Id);
            return this.GetEntity(reqParams);
        }

        public virtual IList<TResponse> GetList<TResponse>(Hashtable reqParams)
        {
            string getList = DefaultCommand.GetList;
            return base.SqlEntity.QueryForList<TResponse>(base.GetStatementName(getList), reqParams);
        }

        public virtual IList<TResponse> GetListByPage<TResponse>(Hashtable reqParams)
        {
            string getListByPage = DefaultCommand.GetListByPage;
            return base.SqlEntity.QueryForList<TResponse>(base.GetStatementName(getListByPage), reqParams);
        }

        public virtual int GetRecord(Hashtable reqParams)
        {
            string getRecord = DefaultCommand.GetRecord;
            return base.SqlEntity.QueryForObject<int>(base.GetStatementName(getRecord), reqParams);
        }

        public virtual IList<TResponse> GetTop<TResponse>(int topNum, Hashtable reqParams)
        {
            if (reqParams == null)
            {
                reqParams = new Hashtable();
            }
            reqParams.Add("Num", topNum);
            string getTop = DefaultCommand.GetTop;
            return base.SqlEntity.QueryForList<TResponse>(base.GetStatementName(getTop), reqParams);
        }

        public virtual bool IsExist(Hashtable reqParams)
        {
            string isExist = DefaultCommand.IsExist;
            return (base.SqlEntity.QueryForObject<int>(base.GetStatementName(isExist), reqParams) > 0);
        }

        protected override void InitSqlNameSpace()
        {
            base.SqlNameSpace = typeof(TEntity).Name;
        }
    }
}
