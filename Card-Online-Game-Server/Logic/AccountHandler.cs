﻿using AhpilyServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card_Online_Game_Server.Logic
{
    /// <summary>
    ///  账号处理逻辑层
    /// </summary>
    public class AccountHandler : IHandler
    {
        public void OnDisconnect(ClientPeer client)
        {
        }

        public void OnReceive(ClientPeer client, int subCode, object value)
        {
        }
    }
}