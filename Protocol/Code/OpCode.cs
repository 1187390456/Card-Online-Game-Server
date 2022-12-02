using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Code
{
    public class OpCode
    {
        public const int Account = 0; // 账号模块
        public const int User = 1; // 角色模块
        public const int Match = 3; // 匹配房间模块
        public const int Chat = 4; // 聊天模块
    }
}