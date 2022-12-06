using Protocol.Constant;
using Protocol.Dto.Fight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card_Online_Game_Server.Cache
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
            Shuffle();
        }

        // 初始化 下一轮开始 洗牌
        public void Init()
        {
            CreateNormalCard();
            Shuffle();
        }

        public CardDto Deal() => CardQueue.Dequeue(); // 发牌

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

        // 洗牌 通过随机数插入实现
        private void Shuffle()
        {
            List<CardDto> newCard = new List<CardDto>(); // 新的卡牌

            Random random = new Random(); // 随机数

            foreach (CardDto card in CardQueue) // 遍历之前的卡片
            {
                int index = random.Next(0, newCard.Count + 1); // 从新卡牌中获取随机插入索引
                newCard.Insert(index, card); // 插入
            }
            CardQueue.Clear(); // 清空之前的卡牌

            foreach (CardDto card in newCard) // 遍历新的卡牌 入队完成洗牌
            {
                CardQueue.Enqueue(card);
            }
        }
    }
}