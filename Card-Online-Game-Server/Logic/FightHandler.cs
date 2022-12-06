using AhpilyServer;
using Card_Online_Game_Server.Cache;
using Card_Online_Game_Server.Model;
using Protocol.Code;
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
                Borcast(room, OpCode.Fight, FightCode.Grad_Landowner_Bro, room.GetStartUid());
            });
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