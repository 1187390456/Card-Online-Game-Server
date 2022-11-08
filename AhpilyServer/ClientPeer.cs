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

        public ClientPeer()
        {
            receiveArgs = new SocketAsyncEventArgs
            {
                UserToken = this // 将自身存储在userToken字段
            }; // 接收异步套接字
            sendArgs = new SocketAsyncEventArgs(); // 发送异步套接字
            sendArgs.Completed += SendArgs_Completed;
            sendQueue = new Queue<byte[]>(); // 消息队列
        }

        #region 接收数据

        public delegate void ReceiveCompleted(ClientPeer client, SocketMsg msg); // 处理包完成事件委托

        public ReceiveCompleted receiveCompleted { get; set; } //消息解析完成回调

        private List<byte> dataCache = new List<byte>(); // 一旦接收到数据 存储到缓存区里面

        public SocketAsyncEventArgs receiveArgs { get; set; } // 异步接收套接字请求

        private bool isReceiveProcess = false; //是否正在处理接收数据

        /// <summary>
        /// 自身处理数据包 长度加自身数据
        /// </summary>
        /// <param name="packet"></param>
        public void StartReceive(byte[] packet)
        {
            dataCache.AddRange(packet);
            if (!isReceiveProcess) ProcessReceive();
        }

        /// <summary>
        /// 处理接收数据
        /// </summary>
        private void ProcessReceive()
        {
            isReceiveProcess = true;

            byte[] data = EncodeTool.DecodePacket(ref dataCache); // 解析数据包
            // 未解析成功
            if (data == null)
            {
                isReceiveProcess = false;
                return;
            }

            SocketMsg msg = EncodeTool.DecodeMsg(data); // 将收到的数据包转成 所需的消息类

            // 回调给上层
            receiveCompleted?.Invoke(this, msg);

            // 尾递归
            ProcessReceive();
        }

        #endregion 接收数据

        #region 断开连接

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            // 清空数据
            dataCache.Clear();
            isReceiveProcess = false;
            //TODO 发送数据预留

            clientSocket.Shutdown(SocketShutdown.Both);  // 断开 发送/接受 消息 both都断开
            clientSocket.Close(); // 释放资源
            clientSocket = null;

            clientSocket.Disconnect(true);
        }

        #endregion 断开连接

        #region 发送数据

        private Queue<byte[]> sendQueue; // 消息队列
        private bool isSendProcess = false; // 是否正在发送包
        private SocketAsyncEventArgs sendArgs; // 发送异步套接字操作

        public delegate void SendDisconnect(ClientPeer client, string reason); // 声明

        public SendDisconnect sendDisconnect;  // 发送时发现断开连接的回调

        /// <summary>
        /// 发送网络消息
        /// </summary>
        /// <param name="opCode">操作码</param>
        /// <param name="subCode">子操作</param>
        /// <param name="value">参数</param>
        public void Send(int opCode, int subCode, object value)
        {
            SocketMsg msg = new SocketMsg(opCode, subCode, value); // 构造发送消息类
            byte[] data = EncodeTool.EncodeMsg(msg); // 将消息类转成 字节数组
            byte[] packet = EncodeTool.EncodePacket(data); // 将字节数组 转成指定的 数据包字节数组

            // 存入消息队列
            sendQueue.Enqueue(packet);

            if (!isSendProcess) StartSend(); // 不在发送处理中 执行发送处理
        }

        /// <summary>
        /// 处理发送消息
        /// </summary>
        private void StartSend()
        {
            isSendProcess = true;

            // 数据条数为0 停止发送
            if (sendQueue.Count == 0)
            {
                isSendProcess = false;
                return;
            }
            byte[] packet = sendQueue.Dequeue(); // 取出一条数据

            // 设置 异步套接字 数据缓冲区 即与异步套接字一起使用
            sendArgs.SetBuffer(packet, 0, packet.Length);

            var res = clientSocket.SendAsync(sendArgs); // false 完成
            if (!res) ProcessSend();
        }

        /// <summary>
        /// / 发送完成处理
        /// </summary>
        /// <param name="e"></param>
        private void ProcessSend()
        {
            // 发送出错
            if (sendArgs.SocketError != SocketError.Success)
            {
                // 执行委托 客户端断开连接了
                sendDisconnect?.Invoke(this, sendArgs.SocketError.ToString());
            }
            else
            {
                StartSend(); // 尾递归发送消息
            }
        }

        /// <summary>
        /// 异步发送完成回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendArgs_Completed(object sender, SocketAsyncEventArgs e) => ProcessSend();

        #endregion 发送数据
    }
}