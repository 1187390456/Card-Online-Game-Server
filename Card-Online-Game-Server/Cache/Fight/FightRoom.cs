using AhpilyServer;
using Protocol.Constant;
using Protocol.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Card_Online_Game_Server.Cache
{
    public class FightRoom
    {
        public int Id; // 标识id

        public int Multiple; // 倍数

        public List<CardDto> TableCardList; // 底牌

        public List<PlayerDto> PlayerList; // 玩家列表

        public List<int> LeavePlayerLists; // 逃跑玩家列表

        public CardLibrary CardLibrary; // 牌库

        public FightRound FightRound; // 战斗回合

        public FightRoom(int id, List<int> uidList)
        {
            Id = id;
            Multiple = 15;
            PlayerList = new List<PlayerDto>();
            LeavePlayerLists = new List<int>();
            TableCardList = new List<CardDto>();
            CardLibrary = new CardLibrary();
            FightRound = new FightRound();
            Init(uidList);   // 遍历玩家列表 转换为玩家信息存储
        }

        // 初始化
        public void Init(List<int> uidList)
        {
            foreach (int uid in uidList)
            {
                PlayerDto player = new PlayerDto(uid);
                PlayerList.Add(player);
            }
        }

        // 转换出牌
        public int Turn()
        {
            int currentUid = FightRound.CurrentUid;
            int nextUid = GetNextUid(currentUid);

            // 更改回合信息
            FightRound.Turn(nextUid);
            return nextUid;
        }

        // 判断是否可以管牌 可以则处理
        public bool JudgeCanDeal(int length, int type, int weight, int userId, List<CardDto> cardList)
        {
            bool canDeal = false;
            if (type == FightRound.LastCardType && weight > FightRound.LastCardWeight)
            {
                if (type == CardType.Straight || type == CardType.Double_Straight) // 顺子和连对 加判断长度
                {
                    if (length >= FightRound.LastCardLength)
                    {
                        canDeal = true;
                    }
                }
                else canDeal = true;
            }
            else if (type == CardType.Boom && FightRound.LastCardType != CardType.Boom)
            {
                Multiple *= 4;
                canDeal = true; // 普通炸弹
            }
            else if (type == CardType.Joker_Boom)
            {
                Multiple *= 8;
                canDeal = true; // 王炸
            }

            if (canDeal)
            {
                RemoveCards(userId, cardList);
                FightRound.Change(length, type, weight, userId);
            }

            return canDeal;
        }

        // 判断是否逃跑
        public bool JudgeIsLeave(int uid) => LeavePlayerLists.Contains(uid);

        // 移除玩家手牌
        public void RemoveCards(int userId, List<CardDto> removeCardList)
        {
            var currentCardList = GetUserCards(userId);

            for (int i = currentCardList.Count - 1; i >= 0; i--)
            {
                foreach (var card in removeCardList)
                {
                    if (currentCardList[i].Name == card.Name) currentCardList.RemoveAt(i);
                }
            }
        }

        // 移除指定玩家
        public void RemocePlayerById(int userId) => PlayerList.Remove(PlayerList.Find(item => item.Id == userId));

        // 开始发牌
        public void StatDeal()
        {
            for (int i = 0; i < PlayerList.Count; i++) // 遍历玩家
            {
                for (int j = 0; j < 17; j++) // 遍历发牌次数 17次
                {
                    CardDto card = CardLibrary.Deal();
                    PlayerList[i].AddCard(card);
                }
            }

            for (int i = 0; i < 3; i++)  // 底牌
            {
                CardDto card = CardLibrary.Deal();
                TableCardList.Add(card);
            }
        }

        // 设置地主 分发底牌 开始回合
        public void SetLandowner(int userId)
        {
            var landownerPlayer = PlayerList.Find(item => item.Id == userId);
            landownerPlayer.Identify = FightIdentity.Landowner;
            landownerPlayer.AddCard(TableCardList);
            FightRound.Start(userId);
        }

        public PlayerDto GetPlayerDto(int userId) => PlayerList.Find(item => item.Id == userId);  // 获取玩家数据模型

        public int GetPlayerIndentity(int userId) => GetPlayerDto(userId).Identify; // 获取玩家身份

        public List<int> GetSameIdentityUids(int indentity) // 获取相同身份的uid列表
        {
            List<int> result = new List<int>();
            foreach (var item in PlayerList)
            {
                if (item.Identify == indentity) result.Add(item.Id);
            }
            return result;
        }

        public List<int> GetDifferentIdentityUids(int indentity) // 获取不身份的uid列表
        {
            List<int> result = new List<int>();
            foreach (var item in PlayerList)
            {
                if (item.Identify != indentity) result.Add(item.Id);
            }
            return result;
        }

        public int GetStartUid()// 返回玩家id 叫地主 后期修改随机
        {
            return PlayerList[0].Id;
        }

        public void SordAllCard(bool order = true) // 给所有牌排序
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                SortCard(PlayerList[i].Cards, order);
            }
            SortCard(TableCardList, order);
        }

        public void SortCard(List<CardDto> cardList, bool order) // 手牌排序
        {
            cardList.Sort((cardPer, cardNext) => // 根据前后牌权重分类
            {
                if (order) return cardPer.Weight.CompareTo(cardNext.Weight);
                else return cardPer.Weight.CompareTo(cardNext.Weight) * -1;
            });
        }

        public List<CardDto> GetUserCards(int userId) => PlayerList.Find(item => item.Id == userId).Cards;  // 获取当前玩家的手牌

        // 获取下一轮出牌者id
        public int GetNextUid(int currentUid)
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                if (PlayerList[i].Id == currentUid)
                {
                    if (i == 2) return PlayerList[0].Id;
                    else return PlayerList[i + 1].Id;
                }
            }
            throw new Exception("出牌者id错误!");
        }
    }
}