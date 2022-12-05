using AhpilyServer;
using Card_Online_Game_Server.Cache;
using Card_Online_Game_Server.Model;
using Protocol.Code;
using Protocol.Dto;
using System;

namespace Card_Online_Game_Server.Logic
{
    /// <summary>
    ///  账号处理逻辑层
    /// </summary>
    public class AccountHandler : IHandler
    {
        private AccountCache accountCache = Caches.AccountCache;
        private UserCache userCache = Caches.UserCache;

        public void OnDisconnect(ClientPeer client)
        {
            if (accountCache.IsOnline(client)) accountCache.Offline(client);
        }

        /// <summary>
        /// 接收客户端消息
        /// </summary>
        /// <param name="client"></param>
        /// <param name="subCode"></param>
        /// <param name="value"></param>
        public void OnReceive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case AccountCode.Regist_Cres:
                    {
                        AccountDto accountDto = value as AccountDto;
                        Regist(client, accountDto.Account, accountDto.Password);
                    }
                    break;

                case AccountCode.Login:
                    {
                        AccountDto accountDto = value as AccountDto;
                        Login(client, accountDto.Account, accountDto.Password);
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="clientPeer"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        private void Regist(ClientPeer clientPeer, string account, string password)
        {
            // 防止多线程同时访问
            SingleExecute.Instance.Execute(() =>
            {
                if (accountCache.IsExit(account))
                {
                    clientPeer.Send(OpCode.Account, AccountCode.Regist_Sres, -1);
                    return;
                }
                if (string.IsNullOrEmpty(account) || account.Length < 4 || account.Length > 16)
                {
                    clientPeer.Send(OpCode.Account, AccountCode.Regist_Sres, -2);
                    return;
                }
                if (string.IsNullOrEmpty(password) || password.Length < 4 || password.Length > 16)
                {
                    clientPeer.Send(OpCode.Account, AccountCode.Regist_Sres, -3);
                    return;
                }
                accountCache.Create(account, password); // 注册账号

                Console.Clear();
                LogAllAccount();
                clientPeer.Send(OpCode.Account, AccountCode.Regist_Sres, 0);
            });
        }

        private void Login(ClientPeer clientPeer, string account, string password)
        {
            // 防止多线程同时访问
            SingleExecute.Instance.Execute(() =>
            {
                if (!accountCache.IsExit(account))
                {
                    clientPeer.Send(OpCode.Account, AccountCode.Login, -1);
                    return;
                }
                if (accountCache.IsOnline(account))
                {
                    clientPeer.Send(OpCode.Account, AccountCode.Login, -2);
                    return;
                }
                if (!accountCache.IsMatch(account, password))
                {
                    clientPeer.Send(OpCode.Account, AccountCode.Login, -3);
                    return;
                }
                accountCache.Online(clientPeer, account); // 上线账号

                Console.Clear();
                LogAllAccount();

                if (userCache.IsExit(accountCache.GetId(clientPeer)))
                {
                    clientPeer.Send(OpCode.Account, AccountCode.Login, 1);   // 存在角色
                }
                else
                {
                    clientPeer.Send(OpCode.Account, AccountCode.Login, 0);   // 不存在角色
                }
            });
        }

        /// <summary>
        /// 打印账号信息
        /// </summary>
        public void LogAllAccount()
        {
            foreach (var item in accountCache.accModelDic)
            {
                Console.WriteLine("账号 : {0}  密码 : {1}", item.Key, item.Value.Password);
            }
        }
    }
}