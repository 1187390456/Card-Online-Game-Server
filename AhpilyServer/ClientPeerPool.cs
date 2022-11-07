using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhpilyServer
{
    /// <summary>
    /// 客户端连接池
    ///       作用 : 重用客户端连接对象 类似对象池
    /// </summary>
    public class ClientPeerPool
    {
        private Queue<ClientPeer> clientPeerQueue; // 客户端连接池队列

        public ClientPeerPool(int capacity) => clientPeerQueue = new Queue<ClientPeer>(capacity); // 构造连接池

        public void Enqueue(ClientPeer client) => clientPeerQueue.Enqueue(client); // 入队

        public ClientPeer Dequeue() => clientPeerQueue.Dequeue(); // 出队
    }
}