using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocher.Infrastructure.MyBatis.Interface
{
   public interface IInsert<TEntity> where TEntity:class
    {
        TPrimary Insert<TPrimary>(TEntity entity);
    }
}
