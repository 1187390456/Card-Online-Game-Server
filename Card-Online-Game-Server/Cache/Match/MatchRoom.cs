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
        public int Id { get; private set; } // 房间id

        public List<UserModel> UserList { get; private set; }// 当前房间人员列表

        public List<UserModel> UserReadyList { get; private set; }// 当前房间已准备人员列表

        public MatchRoom(int id)
        {
            Id = id;
            UserList = new List<UserModel>();
            UserReadyList = new List<UserModel>();
        }

        public bool IsFull() => UserList.Count >= 3; // 房间是否满了

        public bool IsEmpty() => UserList.Count == 0; //房间是否为空

        public bool IsAllReady() => UserReadyList.Count == 3; // 是否房间所有人准备

        public void Enter(UserModel userModel) => UserList.Add(userModel); // 进入房间

        public void Leave(UserModel userModel) => UserList.Remove(userModel); // 离开房间

        public void Ready(UserModel userModel) => UserReadyList.Add(userModel); // 玩家准备
    }
}