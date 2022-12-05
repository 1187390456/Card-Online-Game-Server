using AhpilyServer;
using Card_Online_Game_Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card_Online_Game_Server.Logic
{
    public class FightHandler : IHandler
    {
        public void OnDisconnect(ClientPeer client)
        {
        }

        public void OnReceive(ClientPeer client, int subCode, object value)
        {
        }

        // 开始战斗
        public void StartFight(List<UserModel> userModels)
        {
        }
    }
}