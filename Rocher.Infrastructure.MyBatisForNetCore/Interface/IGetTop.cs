using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocher.Infrastructure.MyBatis.Interface
{
   public interface IGetTop
    {
        IList<TResponse> GetTop<TResponse>(int topNum, Hashtable reqParams);
    }
}
