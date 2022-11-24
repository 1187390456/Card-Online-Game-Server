using AhpilyServer.Concurrent;
using Card_Online_Game_Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card_Online_Game_Server.Cache.Match
{
    /// <summary>
    /// 匹配缓存层
    /// </summary>
    public class MatchCache
    {
        public Dictionary<UserModel, int> userRoomIdDic = new Dictionary<UserModel, int>(); // 用户模型 对应 房间id  注: 当前用户在哪个房间

        public Dictionary<int, MatchRoom> roomIdModelDic = new Dictionary<int, MatchRoom>(); // 房间id 对应 房间数据模型 注: 当前房间数量

        public Queue<MatchRoom> roomQueue = new Queue<MatchRoom>(); // 房间队列

        public ConcurrentInt id = new ConcurrentInt(-1); // 房间id

        /// <summary>
        /// 进入匹配队列
        /// </summary>
        /// <returns></returns>
        public MatchRoom Enter(UserModel userModel)
        {
            // 遍历房间
            foreach (var room in roomIdModelDic.Values)
            {
                if (room.IsFull()) continue; // 满了继续找下一个

                // 进入
                room.Enter(userModel);
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
                    UserList = new List<UserModel>(),
                    UserReadyList = new List<UserModel>()
                };
            }

            // 开启房间后 存储当前房间 和 角色对应的房间

            matchRoom.Enter(userModel);
            roomIdModelDic.Add(matchRoom.Id, matchRoom);
            userRoomIdDic.Add(userModel, matchRoom.Id);

            return matchRoom;
        }
    }
}