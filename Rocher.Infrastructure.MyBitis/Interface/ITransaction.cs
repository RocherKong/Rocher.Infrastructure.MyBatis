using IBatisNet.DataMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocher.Infrastructure.MyBatis.Interface
{
    public interface ITransaction
    {
        ISqlMapSession BeginTransaction();
        void CommitTransaction();
        void RollBackTransaction();
    }
}
