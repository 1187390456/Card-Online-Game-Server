using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
///  操作码 S 服务器请求 C 客户端请求 Bro 服务器广播
/// </summary>
namespace Protocol.Code
{
    /// <summary>
    /// 账号相关操作码
    /// </summary>
    public class AccountCode
    {
        // 注册

        public const int Regist_Cres = 0;
        public const int Regist_Sres = 1;

        // 登录

        public const int Login = 2;
    }
}