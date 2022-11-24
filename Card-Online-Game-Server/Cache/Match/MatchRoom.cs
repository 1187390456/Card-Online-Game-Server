using AhpilyServer;
using Card_Online_Game_Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card_Online_Game_Server.Cache.Match
{
    /// <summary>
    /// 匹配房间 数据模型 不存数据库
    /// </summary>
    public class MatchRoom
    {
        public int Id;// 房间id

        public Dictionary<UserModel, ClientPeer> UserClientDic;//  用户 对应 客户端  当前房间人员列表

        public List<UserModel> UserReadyList;// 当前房间已准备人员列表

        public bool IsFull() => UserClientDic.Count >= 3; // 房间是否满了

        public bool IsEmpty() => UserClientDic.Count == 0; //房间是否为空

        public bool IsAllReady() => UserReadyList.Count == 3; // 是否房间所有人准备

        public void Enter(UserModel userModel, ClientPeer clientPeer) => UserClientDic.Add(userModel, clientPeer); // 进入房间

        public void Leave(UserModel userModel) => UserClientDic.Remove(userModel); // 离开房间

        public void Ready(UserModel userModel) => UserReadyList.Add(userModel); // 玩家准备

        public void Clear() // 清空列表
        {
            UserClientDic.Clear();
            UserReadyList.Clear();
        }

        public void Borcast(int opCode, int subCode, object value, ClientPeer currentClient = null) // 广播当前房间其他玩家 默认自身客户端不发
        {
            SocketMsg msg = new SocketMsg(opCode, subCode, value); // 构造发送消息类
            byte[] data = EncodeTool.EncodeMsg(msg); // 将消息类转成 字节数组
            byte[] packet = EncodeTool.EncodePacket(data); // 将字节数组 转成指定的 数据包字节数组

            Console.WriteLine("当前房间人数" + UserClientDic.Count);

            foreach (var client in UserClientDic.Values)
            {
                if (currentClient == client) continue;
                client.Send(packet);
            }
        }
    }
}