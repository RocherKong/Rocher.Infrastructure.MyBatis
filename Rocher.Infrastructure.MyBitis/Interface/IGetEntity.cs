using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocher.Infrastructure.MyBatis.Interface
{
   public interface IGetEntity<TEntity> where TEntity:class
    {
        TEntity GetEntity(Hashtable reqParams);
        TEntity GetEntity<TPrimary>(TPrimary Id);
    }
}
