using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Code
{
    public class AccountCode
    {
        /// <summary>
        /// 注册子操作码
        /// </summary>
        public const int Regist_Cres = 0; // 客户端注册 参数AccountDto

        public const int Regist_Sres = 1; // 服务器响应

        /// <summary>
        /// 登录子操作码
        /// </summary>
        public const int LOGIN = 2;
    }
}