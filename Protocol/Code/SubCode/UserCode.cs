using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Code.SubCode
{
    /// <summary>
    /// 角色相关操作码
    /// </summary>
    public class UserCode
    {
        // 获取信息
        public const int Get_Cres = 0; // 客户端请求为C 服务器请求为S

        public const int Get_Sres = 1;

        // 创建角色

        public const int Create_Cres = 2;
        public const int Create_Sres = 3;

        // 角色上线
        public const int Onine_Cres = 4;

        public const int Onine_Sres = 5;
    }
}