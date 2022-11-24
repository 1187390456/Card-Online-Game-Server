using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card_Online_Game_Server.Model
{
    /// <summary>
    /// 账号的数据模型
    /// </summary>
    public class AccountModel
    {
        public int Id; // 账号id
        public string Account; // 账号
        public string Password; // 密码

        public AccountModel(int id, string account, string password)
        {
            Id = id;
            Account = account;
            Password = password;
        }
    }
}