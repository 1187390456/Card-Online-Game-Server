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
                else
                {
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
                else
                {
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
                else
                {
                }
            });
        }
    }
}