using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AhpilyServer
{
    /// <summary>
    /// 客户端
    /// </summary>
    public class ClientPeer
    {
        public Socket clientSocket { get; set; }

        #region 接收数据

        private List<byte> dataCache = new List<byte>(); // 一旦接收到数据 存储到缓存区里面

        public SocketAsyncEventArgs receiveArgs;

        /// <summary>
        /// 自身处理数据包
        /// </summary>
        /// <param name="packet"></param>
        public void StartReceive(byte[] packet)
        {
        }

        #endregion 接收数据
    }
}