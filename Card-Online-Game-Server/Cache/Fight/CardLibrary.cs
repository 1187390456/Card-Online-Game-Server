using Protocol.Constant;
using Protocol.Dto.Fight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card_Online_Game_Server.Cache.Fight
{
    /// <summary>
    /// 牌库
    /// </summary>
    public class CardLibrary
    {
        public Queue<CardDto> CardQueue; // 卡片队列

        public CardLibrary()
        {
            CreateNormalCard();
        }

        // 创建普通牌
        private void CreateNormalCard()
        {
            CardQueue = new Queue<CardDto>();

            for (int color = CardColor.Clue; color <= CardColor.Square; color++) // 遍历花色
            {
                for (int weight = CardWeight.Three; color <= CardWeight.Two; weight++) // 遍历权重
                {
                    CardDto card = new CardDto
                    {
                        Color = color,
                        Weight = weight,
                        Name = $"{color}{weight}"
                    }; // 生成卡牌
                    CardQueue.Enqueue(card); // 入队 52张
                }
            }

            CardDto Sjoker = new CardDto // 小王
            {
                Color = CardColor.None,
                Weight = CardWeight.SJoker,
                Name = $"{CardColor.None}{CardWeight.SJoker}"
            };
            CardDto LJoker = new CardDto // 大王
            {
                Color = CardColor.None,
                Weight = CardWeight.LJoker,
                Name = $"{CardColor.None}{CardWeight.LJoker}"
            };

            CardQueue.Enqueue(Sjoker);
            CardQueue.Enqueue(LJoker);
        }
    }
}