using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocher.Infrastructure.MyBatis
{
    public class DefaultCommand
    {
        public static readonly string Delete = "Delete";
        public static readonly string DeleteList = "DeleteList";
        public static readonly string GetEntity = "GetEntity";
        public static readonly string GetList = "GetList";
        public static readonly string GetListByPage = "GetListByPage";
        public static readonly string GetRecord = "GetRecord";
        public static readonly string GetTop = "GetTop";
        public static readonly string Insert = "Insert";
        public static readonly string IsExist = "IsExist";
        public static readonly string Update = "Update";
    }
}
