using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AhpilyServer
{
    /// <summary>
    /// 服务端
    /// </summary>
    public class ServerPeer
    {
        /// <summary>
        /// 服务端 Scoket对象
        /// </summary>
        private Socket serverSocket;

        /// <summary>
        /// 客户端连接池对象
        /// </summary>
        private ClientPeerPool clientPeerPool;

        /// <summary>
        /// 限制客户端连接数量的信号量
        /// </summary>
        private Semaphore acceptSemaphore;

        /// <summary>
        /// 开启服务器
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="maxCount">最大连接数量</param>
        public void Start(int port, int maxCount)
        {
            try
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                acceptSemaphore = new Semaphore(maxCount, maxCount); // 限制最大连接数量

                clientPeerPool = new ClientPeerPool(maxCount);
                ClientPeer clientPeer = null;
                for (int i = 0; i < maxCount; i++)
                {
                    clientPeer = new ClientPeer();
                    clientPeerPool.Enqueue(clientPeer);
                }

                serverSocket.Bind(new IPEndPoint(IPAddress.Any, port)); // 绑定 ip地址 端口号
                serverSocket.Listen(10); // 设置挂起最大数量
                Console.WriteLine("服务器启动...");
                StartAccept(null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #region 接收客户端的连接

        /// <summary>
        /// 开始等待客户端的连接
        /// </summary>
        /// <param name="e">异步回调事件</param>
        private void StartAccept(SocketAsyncEventArgs e)
        {
            if (e == null)
            {
                e = new SocketAsyncEventArgs();
                e.Completed += Accept_Completed; // 异步事件 执行完毕 会执行当前委托
            }
            // 限制线程的访问
            acceptSemaphore.WaitOne();// 每连接一个 进行计数
            var res = serverSocket.AcceptAsync(e); // 判断是否执行完毕 true 正在执行 false 执行完毕
            if (!res) ProcessAccept(e); // 执行完毕则直接调用
        }

        /// <summary>
        /// 接受请求异步事件完成时触发委托
        /// </summary>
        /// <param name="sender">委托对象</param>
        /// <param name="e">异步回调事件</param>
        private void Accept_Completed(object sender, SocketAsyncEventArgs e)
        {
            // 未执行完毕 等待委托 进行完毕进行执行
            ProcessAccept(e);
        }

        /// <summary>
        /// 处理连接请求
        /// </summary>
        /// <param name="e">异步回调事件</param>
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            ClientPeer client = clientPeerPool.Dequeue();
            client.SetSocket(e.AcceptSocket);

            e.AcceptSocket = null;
            StartAccept(e);
        }

        #endregion 接收客户端的连接
    }
}