using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocher.Infrastructure.MyBatis.Interface
{
    public interface IDelete
    {
        int Delete<TPrimary>(TPrimary Id);
        int DeleteList(string Ids);
    }
}
