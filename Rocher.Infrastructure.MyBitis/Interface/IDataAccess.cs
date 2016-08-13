using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocher.Infrastructure.MyBatis.Interface
{
    public interface IDataAccess<TEntity>:IQueryDataAccess<TEntity>, IInsert<TEntity>, IDelete, IUpdate<TEntity>,ITransaction where TEntity:class
    {
    }
}
