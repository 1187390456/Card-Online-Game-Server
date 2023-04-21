using Protocol.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Constant
{
    /// <summary>
    /// 卡牌权值
    /// </summary>
    public class CardWeight
    {
        public const int Three = 3;
        public const int Four = 4;
        public const int Five = 5;
        public const int Six = 6;
        public const int Seven = 7;
        public const int Eight = 8;
        public const int Nine = 9;
        public const int Ten = 10;

        public const int Jack = 11;
        public const int Queen = 12;
        public const int King = 13;

        public const int One = 14;
        public const int Two = 15;

        public const int SJoker = 16;
        public const int LJoker = 17;

        /// <summary>
        /// 获取卡牌权值
        /// </summary>
        /// <param name="cardList">选中卡牌</param>
        /// <param name="cardType">卡牌类型</param>
        /// <returns></returns>
        public static int GetWeight(List<CardDto> cardList, int cardType)
        {
            int totalWeight = 0;

            // 三带一 或 三带二
            if (cardType == CardType.Three_One || cardType == CardType.Three_Two)
            {
                // 找出连续的卡牌
                for (int i = 0; i < cardList.Count - 2; i++)
                {
                    // 当连续的三个权值相等即找到
                    if (cardList[i].Weight == cardList[i + 1].Weight && cardList[i].Weight == cardList[i + 2].Weight)
                    {
                        totalWeight += (cardList[i].Weight * 3);
                    }
                }
            }
            else // 其他类型 直接计算每张卡片的权值总合
            {
                for (int i = 0; i < cardList.Count; i++)
                {
                    totalWeight += cardList[i].Weight;
                }
            }
            return totalWeight;
        }
    }
}