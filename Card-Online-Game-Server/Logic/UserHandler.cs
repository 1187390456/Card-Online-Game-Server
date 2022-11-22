using AhpilyServer;
using Card_Online_Game_Server.Cache;
using Protocol.Code;
using Protocol.Code.SubCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card_Online_Game_Server.Logic
{
    /// <summary>
    /// 角色逻辑处理层
    /// </summary>
    public class UserHandler : IHandler

    {
        private UserCache userCache = Caches.UserCache; // 用户缓存
        private AccountCache accountCache = Caches.AccountCache; // 账号缓存

        public void OnDisconnect(ClientPeer client)
        {
            if (userCache.IsOnline(client)) userCache.Offline(client);
        }

        public void OnReceive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case UserCode.Create_Cres:
                    Create(client, value.ToString());
                    break;

                case UserCode.Onine_Cres:
                    Online(client);
                    break;

                case UserCode.Get_Cres:
                    GetUserInfo(client);
                    break;
            }
        }

        /// <summary>
        ///  创建角色
        /// </summary>
        /// <param name="clientPeer"></param>
        /// <param name="name"></param>
        private void Create(ClientPeer clientPeer, string name)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (!accountCache.IsOnline(clientPeer)) // 玩家是否在线
                {
                    clientPeer.Send(OpCode.User, UserCode.Create_Sres, -1); // 非法登录
                    return;
                }

                var accountId = accountCache.GetId(clientPeer); // 获取账号id

                if (userCache.IsExit(accountId)) //是否存在角色
                {
                    clientPeer.Send(OpCode.User, UserCode.Create_Sres, -2); //重复创建
                    return;
                }

                userCache.Create(name, accountId); // 创建角色

                clientPeer.Send(OpCode.User, UserCode.Create_Sres, 0); // 创建成功
            });
        }

        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <param name="clientPeer"></param>
        private void GetUserInfo(ClientPeer clientPeer)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (!accountCache.IsOnline(clientPeer))
                {
                    clientPeer.Send(OpCode.User, UserCode.Get_Sres, -1); // 非法登录
                    return;
                }
                var accountId = accountCache.GetId(clientPeer);
                if (!userCache.IsExit(accountId))
                {
                    clientPeer.Send(OpCode.User, UserCode.Get_Sres, -2); // 不存在角色
                    return;
                }
                // TODO 有问题
                var userModel = userCache.GetUserModelByAccountId(accountId);
                clientPeer.Send(OpCode.User, UserCode.Get_Sres, userModel); // 获取角色成功
            });
        }

        /// <summary>
        /// 角色上线
        /// </summary>
        /// <param name="clientPeer"></param>
        private void Online(ClientPeer clientPeer)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (!accountCache.IsOnline(clientPeer))
                {
                    clientPeer.Send(OpCode.User, UserCode.Onine_Sres, -1); // 非法登录
                    return;
                }
                var accountId = accountCache.GetId(clientPeer);

                if (!userCache.IsExit(accountId))
                {
                    clientPeer.Send(OpCode.User, UserCode.Onine_Sres, -2); // 不存在角色
                    return;
                }
                var userId = userCache.GetIdByAccountId(accountId);
                userCache.Online(clientPeer, userId);
                clientPeer.Send(OpCode.User, UserCode.Onine_Sres, 0); // 上线成功
            });
        }
    }
}