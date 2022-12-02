using AhpilyServer;
using AhpilyServer.Concurrent;
using Card_Online_Game_Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card_Online_Game_Server.Cache
{
    /// <summary>
    /// 匹配缓存层
    /// </summary>
    public class MatchCache
    {
        public Dictionary<UserModel, int> userRoomIdDic = new Dictionary<UserModel, int>(); // 用户模型 对应 房间id  注: 记录角色当前房间

        public Dictionary<int, MatchRoom> currentAliveRoom = new Dictionary<int, MatchRoom>(); // 房间id 对应 房间数据模型 注: 当前存在房间

        public Queue<MatchRoom> roomQueue = new Queue<MatchRoom>(); // 房间队列

        public ConcurrentInt id = new ConcurrentInt(-1); // 房间id

        /// <summary>
        /// 进入房间
        /// </summary>
        /// <returns></returns>
        public MatchRoom Enter(UserModel userModel, ClientPeer clientPeer)
        {
            // 遍历房间
            foreach (var room in currentAliveRoom.Values)
            {
                if (room.IsFull()) continue; // 满了继续找下一个

                // 进入房间  存储角色进入的房间
                room.Enter(userModel, clientPeer);
                userRoomIdDic.Add(userModel, room.Id);
                return room;
            }

            // 没有空闲房间 查看队列是否有房间 进行重用

            MatchRoom matchRoom;

            if (roomQueue.Count > 0)
            {
                matchRoom = roomQueue.Dequeue();
            }
            else
            {
                matchRoom = new MatchRoom
                {
                    Id = id.Add_Get(),
                    UserClientDic = new Dictionary<UserModel, ClientPeer>(),
                    UserReadyList = new List<UserModel>()
                };
            }

            // 开启房间后 存储当前房间 和 角色进入的房间

            matchRoom.Enter(userModel, clientPeer);
            currentAliveRoom.Add(matchRoom.Id, matchRoom);
            userRoomIdDic.Add(userModel, matchRoom.Id);

            return matchRoom;
        }

        /// <summary>
        /// 离开房间
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        public MatchRoom Leave(UserModel userModel)
        {
            int roomId = userRoomIdDic[userModel]; // 获取当前用户房间id
            var room = currentAliveRoom[roomId]; // 当前房间

            room.Leave(userModel); // 离开房间
            userRoomIdDic.Remove(userModel); // 从该房间移除玩家

            if (room.IsEmpty()) ClearRoom(room);

            return room;
        }

        public bool IsUserHaveRoom(UserModel userModel) => userRoomIdDic.ContainsKey(userModel); // 当前角色是否有房间

        public MatchRoom GetUserRoom(UserModel userModel) => currentAliveRoom[userRoomIdDic[userModel]]; // 获取角色当前房间

        public void ClearRoom(MatchRoom matchRoom) // 清除房间
        {
            currentAliveRoom.Remove(matchRoom.Id); // 移除当前房间

            foreach (var userModel in matchRoom.UserClientDic.Keys)   // 移除房间中的玩家
            {
                userRoomIdDic.Remove(userModel);
            }

            matchRoom.Clear();
            roomQueue.Enqueue(matchRoom);
        }
    }
}