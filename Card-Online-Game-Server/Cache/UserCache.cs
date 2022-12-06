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
    public class UserCache
    {
        public Dictionary<int, int> accUidDic = new Dictionary<int, int>(); // 账号id 对应 角色id

        public Dictionary<int, UserModel> userModelDic = new Dictionary<int, UserModel>(); // 角色id 对应 角色模型

        public ConcurrentInt id = new ConcurrentInt(-1); //角色id

        public void Create(string name, int accountId)// 创建角色
        {
            UserModel userModel = new UserModel(accountId, id.Add_Get(), name);
            userModelDic.Add(userModel.Id, userModel);
            accUidDic.Add(accountId, userModel.Id);
        }

        public bool IsExit(int accountId) => accUidDic.ContainsKey(accountId); // 当前账号是否有角色

        public UserModel GetUserModelByAccountId(int accountId) => userModelDic[accUidDic[accountId]]; // 根据账号id获取角色模型

        public UserModel GetUserModelByUid(int uid) => userModelDic[uid]; //根据角色id获取角色模型

        public int GetIdByAccountId(int accountId) => accUidDic[accountId]; // 根据账号id获取角色id

        #region 上线相关

        private Dictionary<int, ClientPeer> idClientDic = new Dictionary<int, ClientPeer>(); // 在线玩家 角色id 对应 客户端

        private Dictionary<ClientPeer, int> clientIdDic = new Dictionary<ClientPeer, int>(); // 在线玩家 客户端 对应 角色id

        public bool IsOnline(ClientPeer clientPeer) => clientIdDic.ContainsKey(clientPeer); // 是否在线

        public bool IsOnline(int id) => idClientDic.ContainsKey(id); // 是否在线

        public void Online(ClientPeer clientPeer, int id) // 上线
        {
            idClientDic.Add(id, clientPeer);
            clientIdDic.Add(clientPeer, id);
        }

        public void Offline(ClientPeer clientPeer) // 下线
        {
            idClientDic.Remove(clientIdDic[clientPeer]);
            clientIdDic.Remove(clientPeer);
        }

        public UserModel GetUserModelByClient(ClientPeer clientPeer) => userModelDic[clientIdDic[clientPeer]];// 根据连接对象获取角色模型

        public ClientPeer GetClientById(int id) => idClientDic[id]; // 根据角色id获取连接对象

        public int GetIdByClient(ClientPeer clientPeer) => clientIdDic[clientPeer]; // 根据在线连接对象获取角色id

        #endregion 上线相关
    }
}