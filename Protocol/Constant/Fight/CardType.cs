using Protocol.Dto;
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

        public static bool IsSingle(List<CardDto> cards) => cards.Count == 1; // 单牌

        public static bool IsDouble(List<CardDto> cards) => cards.Count == 2 && cards[0].Weight == cards[1].Weight; // 对子

        public static bool IsStraight(List<CardDto> cards) // 顺子
        {
            if (cards.Count < 5 || cards.Count > 12) return false; // 不满足长度
            for (int i = 0; i < cards.Count - 1; i++) // 边界限制
            {
                var preCardWeight = cards[i].Weight;
                var nextCardWeight = cards[i + 1].Weight;
                if (nextCardWeight - preCardWeight != 1) return false; // 不连续
                if (preCardWeight > CardWeight.One || nextCardWeight > CardWeight.One) return false; // 大于A了
            }
            return true;
        }

        public static bool IsDoubleStraight(List<CardDto> cards) // 连对
        {
            if (cards.Count < 6 || cards.Count % 2 != 0) return false; // 没有三个连对 或 不为2的倍数

            for (int i = 0; i < cards.Count - 2; i += 2)  // 边界限制
            {
                var preCardWeight = cards[i].Weight;
                var preSecondCardSWeight = cards[i + 1].Weight;
                var nextCardWeight = cards[i + 2].Weight;
                if (preCardWeight != preSecondCardSWeight) return false; // 两张不相同
                if (nextCardWeight - preCardWeight != 1) return false; // 不连续
                if (preCardWeight > CardWeight.One || nextCardWeight > CardWeight.One) return false; // 大于A了
            }
            return true;
        }

        public static bool IsTripleStraight(List<CardDto> cards) // 飞机
        {
            if (cards.Count < 6 || cards.Count % 3 != 0) return false; // 没有两个三对 或 不为3的倍数

            for (int i = 0; i < cards.Count - 3; i += 3)  // 边界限制
            {
                var preCardWeight = cards[i].Weight;
                var preSecondCardSWeight = cards[i + 1].Weight;
                var preThirdCardSWeight = cards[i + 2].Weight;
                var nextCardWeight = cards[i + 3].Weight;
                if (preCardWeight != preSecondCardSWeight || preCardWeight != preThirdCardSWeight) return false; // 三张不相同
                if (nextCardWeight - preCardWeight != 1) return false; // 不连续
                if (preCardWeight > CardWeight.One || nextCardWeight > CardWeight.One) return false; // 大于A了
            }
            return true;
        }

        public static bool IsThree(List<CardDto> cards) // 三不带
        {
            if (cards.Count != 3) return false; // 不满足长度
            if (cards[0].Weight != cards[1].Weight || cards[0].Weight != cards[2].Weight) return false; // 不相同
            return true;
        }

        public static bool IsThreeOne(List<CardDto> cards) // 三带一
        {
            if (cards.Count != 4) return false; // 不满足长度

            var one = cards[0].Weight;
            var two = cards[1].Weight;
            var three = cards[2].Weight;
            var four = cards[3].Weight;

            if (one == two && one == three) return true; // 前三张相同 满足
            if (two == three && two == four) return true; // 后三张相同 满足
            return false;
        }

        public static bool IsThreeTwo(List<CardDto> cards) // 三带二
        {
            if (cards.Count != 5) return false; // 不满足长度

            var one = cards[0].Weight;
            var two = cards[1].Weight;
            var three = cards[2].Weight;
            var four = cards[3].Weight;
            var five = cards[4].Weight;

            if (one == two && one == three && four == five) return true; // 前三张相同 后两张相同 满足

            if (three == four && three == five && one == two) return true; // 后三张相同 前两张相同 满足

            return false;
        }

        public static bool IsBoom(List<CardDto> cards) // 是否是炸弹
        {
            if (cards.Count != 4) return false; // 长度不满足

            var one = cards[0].Weight;
            var two = cards[1].Weight;
            var three = cards[2].Weight;
            var four = cards[3].Weight;

            if (one == two && one == three && one == four) return true; // 全相同 满足

            return false;
        }

        public static bool IsJokerBoom(List<CardDto> cards) // 是否是王炸
        {
            if (cards.Count != 2) return false; // 长度不满足

            var one = cards[0].Weight;
            var two = cards[1].Weight;

            if (one == CardWeight.LJoker && two == CardWeight.SJoker) return true; // 满足
            if (one == CardWeight.SJoker && two == CardWeight.LJoker) return true; // 满足

            return false;
        }
    }
}