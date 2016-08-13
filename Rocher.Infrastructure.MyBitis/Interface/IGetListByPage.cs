using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocher.Infrastructure.MyBatis.Interface
{
    public  interface IGetListByPage
    {
        IList<TResponse> GetListByPage<TResponse>(Hashtable reqParams);
    }
}
