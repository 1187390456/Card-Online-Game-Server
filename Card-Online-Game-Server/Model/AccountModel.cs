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
        public int Id;
        public string Account;
        public string Password;

        public AccountModel(int id, string account, string password)
        {
            Id = id;
            Account = account;
            Password = password;
        }
    }
}