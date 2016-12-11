using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocher.Infrastructure.MyBatis.Interface
{
   public  interface IUpdate<TEntity> where TEntity:class
    {
        int Update(TEntity entity);
    }
}
