using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Code
{
    /// <summary>
    /// 战斗操作码
    /// </summary>
    public class FightCode
    {
        // 抢地主

        public const int Grad_Landowner_Cres = 0; // 抢地主
        public const int Grad_Landowner_Bro = 1; // 抢地主广播
        public const int Turn_Grad_Bro = 2; // 抢地主轮换广播

        // 出牌

        public const int Deal_Cres = 3; // 出牌
        public const int Deal_Sres = 4; // 出牌响应
        public const int Deal_Bro = 5; // 出牌结果广播

        // 不出

        public const int Pass_Cres = 6; // 不出
        public const int Pass_Sres = 7; // 不出响应

        public const int Turn_Deal_Bro = 8; // 出牌转换广播

        public const int Leave_Bro = 9; // 有人离开

        public const int Over_Bro = 10; //游戏结束

        public const int Get_Card_Sres = 11; // 服务器卡牌响应
    }
}