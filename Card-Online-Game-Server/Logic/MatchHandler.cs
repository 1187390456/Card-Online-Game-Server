using AhpilyServer;
using Card_Online_Game_Server.Cache;
using Card_Online_Game_Server.Cache.Match;
using Card_Online_Game_Server.Model;
using Protocol.Code;
using Protocol.Code.SubCode;
using Protocol.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card_Online_Game_Server.Logic
{
    public class MatchHandler : IHandler
    {
        public MatchCache matchCache = Caches.MatchCache;
        public UserCache userCache = Caches.UserCache;

        public void OnDisconnect(ClientPeer client)
        {
            if (!userCache.IsOnline(client)) return;
            UserModel userModel = userCache.GetUserModelByClient(client); // 获取当前客户端角色
            if (matchCache.IsUserHaveRoom(userModel)) Leave(client); // 角色在房间则调用离开房间
        }

        public void OnReceive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case MatchCode.Enter_Cres:
                    Enter(client);
                    break;

                case MatchCode.Leave_Cres:
                    Leave(client);
                    break;

                case MatchCode.Ready_Cres:
                    Ready(client);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 进入房间
        /// </summary>
        /// <param name="clientPeer"></param>
        private void Enter(ClientPeer clientPeer)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (!userCache.IsOnline(clientPeer)) return; // 角色不在线

                UserModel userModel = userCache.GetUserModelByClient(clientPeer); // 获取当前客户端角色

                if (matchCache.IsUserHaveRoom(userModel)) return; // 角色存在房间

                MatchRoom matchRoom = matchCache.Enter(userModel, clientPeer);      // 进入房间

                matchRoom.Borcast(OpCode.Match, MatchCode.Enter_Bro, userModel.Id, clientPeer);     // 进入 广播给其他用户

                MatchRoomDto matchRoomDto = CreateMatchRoomDto(matchRoom); // 重新获取当前房间状态
                clientPeer.Send(OpCode.Match, MatchCode.Enter_Sres, matchRoomDto); // 发送房间状态
            });
        }

        /// <summary>
        /// 离开房间
        /// </summary>
        /// <param name="clientPeer"></param>
        private void Leave(ClientPeer clientPeer)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (!userCache.IsOnline(clientPeer)) return; // 角色不在线

                UserModel userModel = userCache.GetUserModelByClient(clientPeer);

                if (!matchCache.IsUserHaveRoom(userModel)) return; // 当前用户不在房间

                MatchRoom matchRoom = matchCache.GetUserRoom(userModel);

                // 离开 广播给所有人 要先广播给所有人再离开 要不然自己收不到  这里先做测试
                matchRoom.Borcast(OpCode.Match, MatchCode.Leave_Bro, userModel.Id);

                matchCache.Leave(userModel); // 离开房间
            });
        }

        /// <summary>
        ///  房间准备
        /// </summary>
        /// <param name="clientPeer"></param>
        private void Ready(ClientPeer clientPeer)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (!userCache.IsOnline(clientPeer)) return; // 角色不在线

                UserModel userModel = userCache.GetUserModelByClient(clientPeer);

                if (!matchCache.IsUserHaveRoom(userModel)) return; // 当前用户不在房间

                MatchRoom matchRoom = matchCache.GetUserRoom(userModel); // 获取房间
                matchRoom.Ready(userModel); // 准备

                matchRoom.Borcast(OpCode.Match, MatchCode.Ready_Bro, userModel.Id, clientPeer); // 广播给其他用户

                if (matchRoom.IsAllReady()) // 所有玩家准备
                {
                    matchRoom.Borcast(OpCode.Match, MatchCode.Start_Bro, null); //通知房间内其他玩家开始战斗了
                    matchCache.ClearRoom(matchRoom); // 清除房间
                }
            });
        }

        private MatchRoomDto CreateMatchRoomDto(MatchRoom matchRoom) // 创建传输模型 传输当前房间状态
        {
            MatchRoomDto matchRoomDto = new MatchRoomDto // 初始化房间传输模型
            {
                uidUserDic = new Dictionary<int, UserDto>(),
                readyList = new List<int>()
            };

            foreach (var user in matchRoom.UserClientDic.Keys) // 遍历存储玩家信息字典
            {
                UserDto userDto = new UserDto
                {
                    Avatar = user.Avatar,
                    AvatarMask = user.AvatarMask,
                    RankLogo = user.RankLogo,
                    RankName = user.RankName,
                    GradeLogo = user.GradeLogo,
                    GradeName = user.GradeName,
                    Name = user.Name,
                }; // 当前用户传输模型

                matchRoomDto.uidUserDic.Add(user.Id, userDto);
            }
            foreach (var user in matchRoom.UserReadyList)// 遍历存储准备玩家列表
            {
                matchRoomDto.readyList.Add(user.Id);
            }
            return matchRoomDto;
        }
    }
}