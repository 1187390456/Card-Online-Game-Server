﻿using AhpilyServer;
using Card_Online_Game_Server.Cache;
using Card_Online_Game_Server.Model;
using Protocol.Code;
using Protocol.Constant;
using Protocol.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
            if (!userCache.IsOnline(client)) return; // 玩家不在线

            int uid = userCache.GetIdByClient(client);

            if (!fightCache.IsUserHaveFightRoom(uid)) return; // 玩家没有战斗房间

            FightRoom room = fightCache.GetFightRoomByUid(uid);

            Leave(uid, room);
        }

        public void OnReceive(ClientPeer client, int subCode, object value)
        {
            if (!userCache.IsOnline(client)) return; // 玩家不在线

            int uid = userCache.GetIdByClient(client);

            if (!fightCache.IsUserHaveFightRoom(uid)) return; // 玩家没有战斗房间

            FightRoom room = fightCache.GetFightRoomByUid(uid);

            switch (subCode)
            {
                case FightCode.Grab_Landowner_Cres:
                    GrabLandowner((bool)value, uid, room);
                    break;

                case FightCode.Deal_Cres:

                    Deal(client, (DealDto)value, uid, room);
                    break;

                case FightCode.Pass_Cres:
                    Pass(client, uid, room);
                    break;

                case FightCode.Leave_Cres:
                    Leave(uid, room);
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

                var startID = room.GetStartUid();

                int nextId = room.GetNextUid(startID);
                TurnDto turnDto = new TurnDto(startID, nextId, true);

                Borcast(room, OpCode.Fight, FightCode.Turn_Grad_Bro, turnDto); // 广播轮换第一次
            });
        }

        // 抢地主
        private void GrabLandowner(bool isGrabe, int uid, FightRoom room)
        {
            SingleExecute.Instance.Execute(() =>
            {
                // 是否抢地主 这里抢到直接给 后期优化
                if (isGrabe)
                {
                    var playerCardList = room.SetLandowner(uid);
                    GrabDto grabDto = new GrabDto(uid, room.TableCardList, playerCardList);
                    Borcast(room, OpCode.Fight, FightCode.Grab_Landowner_Bro, grabDto); // 广播抢地主消息 成功结果

                    //  出牌
                    Borcast(room, OpCode.Fight, FightCode.Turn_Deal_Bro, uid);
                }
                else
                {
                    int nextId = room.GetNextUid(uid);
                    TurnDto turnDto = new TurnDto(uid, nextId, false);

                    if (room.JudgeCanTurnGrab()) //两次轮换后直接强制
                    {
                        Borcast(room, OpCode.Fight, FightCode.Turn_Grad_Bro, turnDto); // 不抢地主 转换 发送下一个玩家id 
                        room.GrabTurnCount++;
                    }
                    else
                    {
                        // 直接抢强制抢成功  下一位

                        room.SetLandowner(nextId);
                        var playerCardList = room.SetLandowner(uid);
                        GrabDto grabDto = new GrabDto(nextId, room.TableCardList, playerCardList);
                        Borcast(room, OpCode.Fight, FightCode.Grab_Landowner_Bro, grabDto); // 广播抢地主消息 成功结果

                        //  出牌
                        Borcast(room, OpCode.Fight, FightCode.Turn_Deal_Bro, nextId); // 出牌
                    }
                }
            });
        }

        // 出牌处理
        private void Deal(ClientPeer client, DealDto dealDto, int uid, FightRoom room)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (room.LeavePlayerLists.Contains(uid)) Turn(room); // 玩家逃跑 转换出牌

                var canDeal = room.JudgeCanDeal(dealDto.Length, dealDto.Type, dealDto.Weight, dealDto.Uid, dealDto.SelectCardList);

                var cardType = CardType.GetCardType(dealDto.SelectCardList);

                if (!canDeal || cardType == CardType.None)
                {
                    client.Send(OpCode.Fight, FightCode.Deal_Bro, null); // 不能出牌 自身响应
                }
                else
                {
                    Borcast(room, OpCode.Fight, FightCode.Deal_Bro, dealDto); // 出牌成功 响应

                    // 判断玩家手牌
                    if (room.GetPlayerDto(uid).Cards.Count == 0) GameOver(uid, room); // 游戏结束
                    else Turn(room); // 转换出牌
                }
            });
        }

        // 不出的处理

        private void Pass(ClientPeer client, int uid, FightRoom room)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (room.FightRound.LastUid == uid)
                {
                    client.Send(OpCode.Fight, FightCode.Pass_Bro, null); // 当前玩家为最大出牌者 不可以不出
                    return;
                }

                Borcast(room, OpCode.Fight, FightCode.Pass_Bro, uid); // 可以不出 广播响应

                Turn(room);
            });
        }

        // 离开处理
        private void Leave(int uid, FightRoom room)
        {
            SingleExecute.Instance.Execute(() =>
            {
                room.LeavePlayerLists.Add(uid); // 添加离开列表

                room.RemocePlayerById(uid); // 移除房间玩家
                fightCache.RemoveUidRoom(uid); // 移除玩家房间绑定

                Borcast(room, OpCode.Fight, FightCode.Leave_Bro, uid);

                if (room.LeavePlayerLists.Count == 3) // 所有人走了
                {
                    foreach (var leaveId in room.LeavePlayerLists) Update(leaveId, room, OverIdentity.Leaver); // 所有逃跑者更新

                    fightCache.ClearRoom(room);
                }
            });
        }

        // 游戏结束
        private void GameOver(int winnerId, FightRoom room)
        {
            var winIdentity = room.GetPlayerIndentity(winnerId); // 获取获胜身份
            var winLists = room.GetSameIdentityUids(winIdentity); // 获胜人id列表
            var failLists = room.GetDifferentIdentityUids(winIdentity); // 失败者id列表

            // 获胜者
            foreach (var winId in winLists) Update(winId, room, OverIdentity.Winner);

            // 失败者
            foreach (var failId in failLists) Update(failId, room, OverIdentity.Loser);

            //逃跑者
            foreach (var leaveId in room.LeavePlayerLists) Update(leaveId, room, OverIdentity.Leaver);

            // 客户端发送消息
            OverDto overDto = new OverDto(room.Multiple * 50, winLists, failLists, room.LeavePlayerLists);
            Borcast(room, OpCode.Fight, FightCode.Over_Bro, overDto);

            // 清除房间
            fightCache.ClearRoom(room);
        }

        // 转换出牌
        private void Turn(FightRoom room)
        {
            int nextUid = room.Turn();

            if (room.JudgeIsLeave(nextUid)) Turn(room); // 下一个玩家掉线则跳过
            else
            {
                Borcast(room, OpCode.Fight, FightCode.Turn_Deal_Bro, nextUid);  // 下一个玩家出牌 广播

                if (nextUid == room.FightRound.LastUid) // 下一个出牌是最大出牌者 即其他两人不出
                {
                    Borcast(room, OpCode.Fight, FightCode.Pass_Round_Bro, nextUid);  // 通知轮空一回合 无人管牌
                }
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

        public void Update(int uid, FightRoom room, int overIdentity) // 更新角色方法
        {
            UserModel userModel = userCache.GetUserModelByUid(uid);

            switch (overIdentity)
            {
                case OverIdentity.Winner:
                    userModel.BeanCount += room.Multiple * 50;

                    break;

                case OverIdentity.Loser:
                    userModel.BeanCount -= room.Multiple * 50;

                    break;

                case OverIdentity.Leaver:
                    userModel.BeanCount -= room.Multiple * 100;

                    break;

                default:
                    break;
            }

            userCache.UpdateUserModel(userModel);
        }
    }
}