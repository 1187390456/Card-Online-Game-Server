using AhpilyServer.Concurrent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card_Online_Game_Server.Cache
{
    /// <summary>
    /// 战斗缓存层
    /// </summary>
    public class FightCache
    {
        private Dictionary<int, FightRoom> currentFightRoom = new Dictionary<int, FightRoom>(); // 房间id 对应 房间数据 注: 当前存在的战斗房间

        private Dictionary<int, int> uidRoomIdDic = new Dictionary<int, int>(); // 用户id 对应 战斗房间id 注: 代表该玩家在哪个战斗房间

        private Queue<FightRoom> roomQueue = new Queue<FightRoom>(); // 战斗房间队列

        private ConcurrentInt id = new ConcurrentInt(-1); // 房间id

        public FightRoom CrateRoom(List<int> uidList) // 创建房间
        {
            FightRoom room = null;

            // 查找是否有可利用的房间
            if (roomQueue.Count > 0)
            {
                room = roomQueue.Dequeue();
                room.Init(uidList);
            }
            else
            {
                room = new FightRoom(id.Add_Get(), uidList);
            }
            // 绑定映射 存储数据

            foreach (int uid in uidList) // 遍历获取玩家id
            {
                uidRoomIdDic.Add(uid, room.Id); // 玩家与战斗房间绑定
            }
            currentFightRoom.Add(room.Id, room); // 绑定战斗房间

            return room;
        }

        public void ClearRoom(FightRoom room) // 清除房间
        {
            // 解除映射 清除数据
            currentFightRoom.Remove(room.Id); // 清除该战斗房间

            foreach (var player in room.PlayerList) // 遍历获取玩家id
            {
                uidRoomIdDic.Remove(player.Id); // 玩家与战斗房间绑定
            }

            // 初始化房间数据 入队
            room.PlayerList.Clear();
            room.LeavePlayerLists.Clear();
            room.TableCardList.Clear();
            room.CardLibrary.Init();
            room.FightRound.Init();
            room.Multiple = 15;
            room.GrabTurnCount = 0;

            roomQueue.Enqueue(room);
        }

        public bool RemoveUidRoom(int userId) => uidRoomIdDic.Remove(userId); // 移除玩家房间映射 离开就调用

        public bool IsFightRoomAlive(int roomId) => currentFightRoom.ContainsKey(roomId); // 是否存在该战斗房间

        public bool IsUserHaveFightRoom(int uid) => uidRoomIdDic.ContainsKey(uid); // 用户是否拥有战斗房间

        public FightRoom GetFightRoomById(int roomId) => currentFightRoom[roomId]; // 通过房间id 获取战斗房间信息

        public FightRoom GetFightRoomByUid(int uid) => GetFightRoomById(uidRoomIdDic[uid]); // 通过用户id 获取战斗房间信息
    }
}