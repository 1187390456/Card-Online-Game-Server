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
    /// 账号缓存
    /// </summary>
    public class AccountCache
    {
        #region 账号相关

        private Dictionary<string, AccountModel> accModelDic = new Dictionary<string, AccountModel>();  // 账号 账号模型 字典

        private ConcurrentInt id = new ConcurrentInt(-1); // 线程安全的id 后期数据库处理

        public bool IsExit(string account) => accModelDic.ContainsKey(account); //  是否存在账号

        public bool IsMatch(string account, string password) => GetAccountModel(account).Password == password; // 判断账号密码是否与存储密码一致

        public AccountModel GetAccountModel(string account) => accModelDic[account]; // 获取 当前账户数据模型

        /// <summary>
        /// 创建账号
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        public void Create(string account, string password)
        {
            AccountModel accountModel = new AccountModel(id.Add_Get(), account, password);
            accModelDic.Add(account, accountModel); // 存储账号
        }

        #endregion 账号相关

        #region 上线相关

        private Dictionary<string, ClientPeer> accClientDic = new Dictionary<string, ClientPeer>(); // 账号 账号关联的客户端 字典
        private Dictionary<ClientPeer, string> clientAccDic = new Dictionary<ClientPeer, string>(); // 账号关联的客户端 账号 字典

        public bool IsOnline(string account) => accClientDic.ContainsKey(account); // 根据账号判断是否在线

        public bool IsOnline(ClientPeer clientPeer) => clientAccDic.ContainsKey(clientPeer);  // 根据客户端判断是否在线

        /// <summary>
        ///  获取当前连接客户端id 即上线玩家id
        /// </summary>
        /// <param name="clientPeer"></param>
        /// <returns></returns>
        public int GetId(ClientPeer clientPeer)
        {
            string account = clientAccDic[clientPeer];
            return accModelDic[account].Id; //  返回账号模型中的id
        }

        /// <summary>
        /// 用户上线
        /// </summary>
        /// <param name="clientPeer"></param>
        /// <param name="account"></param>
        public void Online(ClientPeer clientPeer, string account)
        {
            accClientDic.Add(account, clientPeer);
            clientAccDic.Add(clientPeer, account);
        }

        /// <summary>
        /// 根据客户端进行下线
        /// </summary>
        /// <param name="clientPeer"></param>

        public void Offline(ClientPeer clientPeer)
        {
            string account = clientAccDic[clientPeer]; // 获取账号
            // 移除
            accClientDic.Remove(account);
            clientAccDic.Remove(clientPeer);
        }

        /// <summary>
        /// 根据账号进行下线
        /// </summary>
        /// <param name="account"></param>
        public void Offline(string account)
        {
            ClientPeer clientPeer = accClientDic[account];
            accClientDic.Remove(account);
            clientAccDic.Remove(clientPeer);
        }

        #endregion 上线相关
    }
}