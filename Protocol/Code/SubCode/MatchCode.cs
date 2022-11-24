using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Code.SubCode
{
    /// <summary>
    /// 匹配相关操作码
    /// </summary>
    public class MatchCode
    {
        // 进入匹配

        public const int Enter_Cres = 0;
        public const int Enter_Sres = 1;
        public const int Enter_Bro = 2;

        // 离开匹配

        public const int Leave_Cres = 3;
        public const int Leave_Bro = 4;

        // 准备

        public const int Ready_Cres = 5;
        public const int Ready_Bro = 6;

        // 开始游戏

        public const int Start_Bro = 7;
    }
}