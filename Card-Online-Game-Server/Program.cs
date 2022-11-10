using AhpilyServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card_Online_Game_Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ServerPeer serverPeer = new ServerPeer();
            serverPeer.SetApplication(new NetMsgCenter());
            serverPeer.Start(6666, 10);
            Console.Read();
        }
    }
}