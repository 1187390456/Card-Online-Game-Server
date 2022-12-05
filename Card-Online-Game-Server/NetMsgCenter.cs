using AhpilyServer;
using Card_Online_Game_Server.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol.Code;

namespace Card_Online_Game_Server
{
    /// <summary>
    /// 网络消息中心
    /// </summary>
    public class NetMsgCenter : IApplication
    {
        private IHandler account = new AccountHandler();
        private IHandler user = new UserHandler();
        private MatchHandler match = new MatchHandler();
        private IHandler chat = new ChatHandler();
        private FightHandler fight = new FightHandler();

        public NetMsgCenter()
        {
            match.StartFightAction += fight.StartFight; // 注册战斗委托
        }

        public void OnDisconnect(ClientPeer client)
        {
            // 注意顺序 先退聊天 再退房间 角色 账号
            chat.OnDisconnect(client);
            match.OnDisconnect(client);
            user.OnDisconnect(client);
            account.OnDisconnect(client);
            fight.OnDisconnect(client);
        }

        public void OnReceive(ClientPeer client, SocketMsg msg)
        {
            switch (msg.OpCode)
            {
                case OpCode.Account:
                    account.OnReceive(client, msg.SubCode, msg.Value);
                    break;

                case OpCode.User:
                    user.OnReceive(client, msg.SubCode, msg.Value);
                    break;

                case OpCode.Match:
                    match.OnReceive(client, msg.SubCode, msg.Value);
                    break;

                case OpCode.Chat:
                    chat.OnReceive(client, msg.SubCode, msg.Value);
                    break;

                default:
                    break;
            }
        }
    }
}