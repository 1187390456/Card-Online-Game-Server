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
                    clientPeer.receiveArgs.Completed += Receive_Completed; // 绑定异步接收事件完成回调
                    clientPeer.receiveArgs.UserToken = clientPeer; // 将自身存储在userToken字段
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
        /// 处理连接请求
        /// </summary>
        /// <param name="e">异步回调事件</param>
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            ClientPeer client = clientPeerPool.Dequeue();
            client.clientSocket = e.AcceptSocket;

            // 开始接收数据
            StartReceive(client);

            e.AcceptSocket = null;
            StartAccept(e);
        }

        /// <summary>
        /// 接受请求异步事件完成时触发委托
        /// </summary>
        /// <param name="sender">委托对象</param>
        /// <param name="e">异步回调事件</param>
        private void Accept_Completed(object sender, SocketAsyncEventArgs e) => ProcessAccept(e);

        #endregion 接收客户端的连接

        #region 接收数据

        /// <summary>
        /// 开始接收数据
        /// </summary>
        /// <param name="client"></param>
        private void StartReceive(ClientPeer client)
        {
            try
            {
                var res = client.clientSocket.ReceiveAsync(client.receiveArgs);
                if (!res) ProcessReceive(client.receiveArgs);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 处理接收的请求
        /// </summary>
        /// <param name="e"></param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            ClientPeer client = e.UserToken as ClientPeer; // 通过存储在自身的userToken 拿到自身客户端

            // 判断网络消息是否接收成功 确保成功 且 传输字节数有值(长度大于0)
            if (client.receiveArgs.SocketError == SocketError.Success && client.receiveArgs.BytesTransferred > 0)
            {
                // 拷贝当前客户端字节数据 即数据包
                byte[] packet = new byte[client.receiveArgs.BytesTransferred];
                Buffer.BlockCopy(client.receiveArgs.Buffer, 0, packet, 0, client.receiveArgs.BytesTransferred);

                // 让客户端自身处理数据包 自身解析
                client.StartReceive(packet);

                StartReceive(client); // 尾递归 闭环
            }
            // 没有传输的字节数 代表断开连接了
            else if (client.receiveArgs.BytesTransferred == 0)
            {
                if (client.receiveArgs.SocketError == SocketError.Success)
                {
                    // 客户端主动断开连接
                }
                else
                {
                    // 异常断开
                }
            }
        }

        /// <summary>
        /// 接收完成时 触发的事件回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Receive_Completed(object sender, SocketAsyncEventArgs e) => ProcessReceive(e);

        #endregion 接收数据
    }
}