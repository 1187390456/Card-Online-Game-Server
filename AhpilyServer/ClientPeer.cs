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
        private Socket clientSocket;

        /// <summary>
        /// 设置连接对象
        /// </summary>
        /// <param name="socket"></param>
        public void SetSocket(Socket clientSocket)
        {
            this.clientSocket = clientSocket;
        }

        #region 接收数据

        /// <summary>
        /// 一旦接收到数据 存储到缓存区里面
        /// </summary>
        private List<byte> dataCache = new List<byte>();

        #endregion 接收数据
    }
}