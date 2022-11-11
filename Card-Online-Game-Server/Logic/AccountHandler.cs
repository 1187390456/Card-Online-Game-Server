using AhpilyServer;
using Card_Online_Game_Server.Cache;
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

        public void OnDisconnect(ClientPeer client)
        {
            if (accountCache.IsOnline(client)) accountCache.Offline(client); // 下线
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

                case AccountCode.LOGIN:
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
                    clientPeer.Send(OpCode.ACCOUNT, AccountCode.Regist_Sres, "账号已存在!");
                    return;
                }
                if (string.IsNullOrEmpty(account) || account.Length < 4 || account.Length > 16)
                {
                    clientPeer.Send(OpCode.ACCOUNT, AccountCode.Regist_Sres, "账号不合法!");
                    return;
                }
                if (string.IsNullOrEmpty(password) || password.Length < 4 || password.Length > 16)
                {
                    clientPeer.Send(OpCode.ACCOUNT, AccountCode.Regist_Sres, "密码不合法!");
                    return;
                }
                accountCache.Create(account, password); // 注册账号
                clientPeer.Send(OpCode.ACCOUNT, AccountCode.Regist_Sres, "注册成功!");
            });
        }

        private void Login(ClientPeer clientPeer, string account, string password)
        {
            // 防止多线程同时访问
            SingleExecute.Instance.Execute(() =>
            {
                if (!accountCache.IsExit(account))
                {
                    clientPeer.Send(OpCode.ACCOUNT, AccountCode.LOGIN, "账号不存在!");
                    return;
                }
                if (accountCache.IsOnline(clientPeer))
                {
                    clientPeer.Send(OpCode.ACCOUNT, AccountCode.LOGIN, "账号已登录!");
                    return;
                }
                if (!accountCache.IsMatch(account, password))
                {
                    clientPeer.Send(OpCode.ACCOUNT, AccountCode.LOGIN, "密码错误!");
                    return;
                }
                accountCache.Online(clientPeer, account); // 上线账号
                clientPeer.Send(OpCode.ACCOUNT, AccountCode.LOGIN, "登录成功!");
            });
        }
    }
}