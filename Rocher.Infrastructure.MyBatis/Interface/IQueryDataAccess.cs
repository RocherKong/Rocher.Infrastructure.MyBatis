using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocher.Infrastructure.MyBatis.Interface
{
    public interface IQueryDataAccess<TEntity> : IIsExist, IGetEntity<TEntity>, IGetList, IGetTop, IGetListByPage, IGetRecord where TEntity : class
    {

    }
}
