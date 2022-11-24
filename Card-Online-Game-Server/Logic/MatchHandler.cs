using AhpilyServer;
using Card_Online_Game_Server.Cache;
using Card_Online_Game_Server.Cache.Match;
using Card_Online_Game_Server.Model;
using Protocol.Code;
using Protocol.Code.SubCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card_Online_Game_Server.Logic
{
    public class MatchHandler : IHandler
    {
        public MatchCache matchCache = Caches.MatchCache;
        public UserCache userCache = Caches.UserCache;

        public void OnDisconnect(ClientPeer client)
        {
            throw new NotImplementedException();
        }

        public void OnReceive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case MatchCode.Enter_Cres:
                    break;

                case MatchCode.Exit_Cres:
                    break;

                case MatchCode.Ready_Cres:
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 进入房间
        /// </summary>
        /// <param name="clientPeer"></param>
        private void Enter(ClientPeer clientPeer)
        {
            SingleExecute.Instance.Execute(() =>
            {
                UserModel userModel = userCache.GetUserModelByClient(clientPeer); // 获取当前客户端角色id

                if (matchCache.IsUserHaveRoom(userModel)) return; // 角色存在房间

                MatchRoom matchRoom = matchCache.Enter(userModel, clientPeer);  // 进入房间

                // 广播给其他用户
                matchRoom.Borcast(OpCode.Match, MatchCode.Enter_Bro, userModel);

                // TODO 返回给当前客户端
            });
        }
    }
}