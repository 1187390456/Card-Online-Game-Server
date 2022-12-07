using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Constant
{
    /// <summary>
    /// 结束身份
    /// </summary>
    public class OverIdentity
    {
        public const int Winner = 0; // 获胜者
        public const int Loser = 1; // 失败者
        public const int Leaver = 2;//逃跑者
    }
}