using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Constant
{
    /// <summary>
    ///  卡牌类型
    /// </summary>
    public class CardType
    {
        public const int None = 0; // 无
        public const int Single = 1; // 单牌
        public const int Double = 2; // 对子
        public const int Straight = 3; // 顺子
        public const int Double_Straight = 4; // 连对
        public const int Triple_Straight = 5; // 飞机
        public const int Three = 6; // 三带
        public const int Three_One = 7; // 三带一
        public const int Three_Two = 8; // 三带二
        public const int Boom = 9; // 炸弹
        public const int Joker_Boom = 10; // 王炸
    }
}