using AhpilyServer;
using Card_Online_Game_Server.Cache;
using Card_Online_Game_Server.Model;
using Protocol.Code;
using Protocol.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card_Online_Game_Server.Logic
{
    public class FightHandler : IHandler
    {
        public FightCache fightCache = Caches.FightCache;
        public UserCache userCache = Caches.UserCache;

        public void OnDisconnect(ClientPeer client)
        {
        }

        public void OnReceive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case FightCode.Grab_Landowner_Cres:
                    GrabLandowner(client, (bool)value);
                    break;

                case FightCode.Deal_Cres:
                    // GrabLandowner(client, (bool)value);
                    Deal(client, (DealDto)value);
                    break;

                default:
                    break;
            }
        }

        // 开始战斗
        public void StartFight(List<int> uidList)
        {
            SingleExecute.Instance.Execute(() =>
            {
                FightRoom room = fightCache.CrateRoom(uidList);

                // 开始发牌 玩家手牌排序
                room.StatDeal();
                room.SordAllCard();

                foreach (int uid in uidList) // 向每个玩家客户端发送玩家卡牌信息
                {
                    var client = userCache.GetClientById(uid);
                    var cardList = room.GetUserCards(uid);
                    client.Send(OpCode.Fight, FightCode.Get_Card_Sres, cardList);
                }

                // 开始抢地主
                Borcast(room, OpCode.Fight, FightCode.Grab_Landowner_Bro, room.GetStartUid());
            });
        }

        // 抢地主
        private void GrabLandowner(ClientPeer client, bool isGrabe)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (!userCache.IsOnline(client)) return; // 玩家不在线

                int uid = userCache.GetIdByClient(client);

                if (!fightCache.IsUserHaveFightRoom(uid)) return; // 玩家没有战斗房间

                FightRoom room = fightCache.GetFightRoomByUid(uid);

                // 是否抢地主 这里抢到直接给 后期优化
                if (isGrabe)
                {
                    room.SetLandowner(uid);
                    GrabDto grabDto = new GrabDto(uid, room.TableCardList);
                    Borcast(room, OpCode.Fight, FightCode.Grab_Landowner_Bro, grabDto); // 广播抢地主消息 成功结果
                }
                else
                {
                    int nextId = room.GetNextUid(uid);
                    Borcast(room, OpCode.Fight, FightCode.Turn_Grad_Bro, nextId); // 不抢地主 转换 发送下一个玩家id
                }
            });
        }

        // 出牌处理
        private void Deal(ClientPeer client, DealDto dealDto)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (!userCache.IsOnline(client)) return; // 玩家不在线

                int uid = userCache.GetIdByClient(client);

                if (!fightCache.IsUserHaveFightRoom(uid)) return; // 玩家没有战斗房间

                FightRoom room = fightCache.GetFightRoomByUid(uid);

                if (room.RunAwayLists.Contains(uid)) Turn(room); // 玩家逃跑 转换出牌

                var canDeal = room.JudgeCanDeal(dealDto.Length, dealDto.Type, dealDto.Weight, dealDto.Uid, dealDto.SelectCardList);

                if (!canDeal)
                {
                    client.Send(OpCode.Fight, FightCode.Deal_Sres, -1); // 不能出牌 自身响应
                }
                else
                {
                    client.Send(OpCode.Fight, FightCode.Deal_Sres, 0); // 出牌成功 自身响应

                    Borcast(room, OpCode.Fight, FightCode.Deal_Bro, dealDto, client); // 出牌成功 其他人响应

                    // 判断玩家手牌
                    if (room.GetPlayerDto(uid).Cards.Count == 0)
                    {
                        // 游戏结束
                    }
                    else Turn(room); // 转换出牌
                }
            });
        }

        // 游戏结束
        private void GameOver(int winnerId, FightRoom room)
        {
            List<UserModel> formarList = new List<UserModel>();
            UserModel Landowner = null;

            // 获取地主和农民的数据模型
            for (int i = 0; i < room.PlayerList.Count; i++)
            {
                if (room.GetLandownerUid() == room.PlayerList[i].Id)
                {
                    Landowner = userCache.GetUserModelByUid(room.PlayerList[i].Id);
                }
                else
                {
                    var model = userCache.GetUserModelByUid(room.PlayerList[i].Id);
                    formarList.Add(model);
                }
            }

            if (room.GetLandownerUid() == winnerId) // 地主获胜
            {
                // 地主胜场+1 加豆子
                var userModel = userCache.GetUserModelByUid(winnerId);
                userModel.BeanCount += 3000;

                // 农民负场+1 减豆子
            }
            else // 农民获胜
            {
            }
        }

        // 转换出牌
        private void Turn(FightRoom room)
        {
            int nextUid = room.Turn();

            if (room.JudgeIsRunAway(nextUid)) Turn(room); // 下一个玩家掉线则跳过
            else
            {
                ClientPeer nextClient = userCache.GetClientById(nextUid);  // 下一个玩家的客户端
                nextClient.Send(OpCode.Fight, FightCode.Turn_Deal_Bro, nextUid); // 下一个玩家出牌 广播
            }
        }

        public void Borcast(FightRoom room, int opCode, int subCode, object value, ClientPeer currentClient = null)     // 广播
        {
            SocketMsg msg = new SocketMsg(opCode, subCode, value); // 构造发送消息类
            byte[] data = EncodeTool.EncodeMsg(msg); // 将消息类转成 字节数组
            byte[] packet = EncodeTool.EncodePacket(data); // 将字节数组 转成指定的 数据包字节数组

            foreach (var player in room.PlayerList)
            {
                var client = userCache.GetClientById(player.Id);
                if (currentClient == client) continue;
                client.Send(packet);
            }
        }
    }
}