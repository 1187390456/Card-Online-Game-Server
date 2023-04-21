using AhpilyServer;
using Card_Online_Game_Server.Cache;
using Protocol.Code;
using Protocol.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Card_Online_Game_Server.Logic
{
    public class ChatHandler : IHandler
    {
        private UserCache userCache = Caches.UserCache;
        private MatchCache matchCache = Caches.MatchCache;
        private FightCache fightCache = Caches.FightCache;

        public void OnDisconnect(ClientPeer client)
        {
        }

        public void OnReceive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case ChatCode.Send_Quick_Cres:
                    SendQuick(client, (int)value);
                    break;

                case ChatCode.Send_ZiDingYi_Cres:
                    SendZiDingYi(client, (string)value);
                    break;

                case ChatCode.Send_Emoji_Cres:
                    SendEmoji(client, (string)value);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        ///  有人发送快捷消息
        /// </summary>
        private void SendQuick(ClientPeer clientPeer, int index)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (!userCache.IsOnline(clientPeer)) return; // 角色不在线

                var userModel = userCache.GetUserModelByClient(clientPeer);
                ChatDto chatDto = new ChatDto
                {
                    id = userModel.Id,
                    Index = index,
                };

                if (matchCache.IsUserHaveRoom(userModel))
                {
                    var room = matchCache.GetUserRoom(userModel);
                    room.Borcast(OpCode.Chat, ChatCode.Send_Quick_Bro, chatDto);
                }
                else if (fightCache.IsUserHaveFightRoom(userModel.Id))
                {
                    var room = fightCache.GetFightRoomByUid(userModel.Id);
                    Borcast(room, OpCode.Chat, ChatCode.Send_Quick_Bro, chatDto);
                }
            });
        }

        /// <summary>
        /// 有人发送自定义消息
        /// </summary>
        /// <param name="clientPeer"></param>
        /// <param name="text"></param>
        private void SendZiDingYi(ClientPeer clientPeer, string text)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (!userCache.IsOnline(clientPeer)) return; // 角色不在线

                var userModel = userCache.GetUserModelByClient(clientPeer);
                ChatDto chatDto = new ChatDto
                {
                    id = userModel.Id,
                    text = text,
                };

                if (matchCache.IsUserHaveRoom(userModel))
                {
                    var room = matchCache.GetUserRoom(userModel);
                    room.Borcast(OpCode.Chat, ChatCode.Send_ZiDingYi_Bro, chatDto);
                }
                else if (fightCache.IsUserHaveFightRoom(userModel.Id))
                {
                    var room = fightCache.GetFightRoomByUid(userModel.Id);
                    Borcast(room, OpCode.Chat, ChatCode.Send_ZiDingYi_Bro, chatDto);
                }
            });
        }

        /// <summary>
        /// 有人发送表情
        /// </summary>
        /// <param name="clientPeer"></param>
        /// <param name="name"></param>
        private void SendEmoji(ClientPeer clientPeer, string name)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (!userCache.IsOnline(clientPeer)) return; // 角色不在线

                var userModel = userCache.GetUserModelByClient(clientPeer);
                ChatDto chatDto = new ChatDto
                {
                    id = userModel.Id,
                    text = name,
                };

                if (matchCache.IsUserHaveRoom(userModel))
                {
                    var room = matchCache.GetUserRoom(userModel);
                    room.Borcast(OpCode.Chat, ChatCode.Send_Emoji_Bro, chatDto);
                }
                else if (fightCache.IsUserHaveFightRoom(userModel.Id))
                {
                    var room = fightCache.GetFightRoomByUid(userModel.Id);
                    Borcast(room, OpCode.Chat, ChatCode.Send_Emoji_Bro, chatDto);
                }
            });
        }


        public void Borcast(FightRoom room, int opCode, int subCode, object value, ClientPeer currentClient = null)     // 战斗房间广播
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