using Protocol.Code.SubCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Dto
{
    /// <summary>
    /// 房间传输数据模型 存储房间信息
    /// </summary>
    [Serializable]
    public class MatchRoomDto
    {
        public Dictionary<int, UserDto> uidUserDic; // 用户id 对应 用户传输数据模型

        public List<int> readyList = new List<int>(); // 准备玩家列表

        public UserDto leftUserDto; // 左侧玩家

        public UserDto rightUserDto; // 右侧玩家

        public List<UserDto> orderList = new List<UserDto>(); // 玩家房间顺序列表

        public void Add(UserDto userDto)  // 添加用户
        {
            uidUserDic.Add(userDto.Id, userDto);
            orderList.Add(userDto);
        }

        public void Remove(int userId)// 移除用户
        {
            orderList.Remove(uidUserDic[userId]);
            uidUserDic.Remove(userId);
        }

        public void RefreshOrderList(int myUserid) //刷新顺序列表 只能用id判断
        {
            leftUserDto = null;
            rightUserDto = null;
            if (orderList.Count == 1) return; // 房间一个人
            if (orderList.Count == 2) // 两个人
            {
                // 自己在第一位
                if (orderList[0].Id == myUserid)
                {
                    rightUserDto = orderList[1];
                }
                // 自己在第二位
                if (orderList[1].Id == myUserid)
                {
                    leftUserDto = orderList[0];
                }
            }
            if (orderList.Count == 3) //三个人
            {
                // 自己第一位
                if (orderList[0].Id == myUserid)
                {
                    rightUserDto = orderList[1];
                    leftUserDto = orderList[2];
                }
                // 自己第二位 
                if (orderList[1].Id == myUserid)
                {
                    leftUserDto = orderList[0];
                    rightUserDto = orderList[2];
                }
                // 自己第三位
                if (orderList[2].Id == myUserid)
                {
                    rightUserDto = orderList[0];
                    leftUserDto = orderList[1];
                }
            }
        }
    }
}