using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Dto
{
    /// <summary>
    /// 账号传输数据模型
    /// </summary>
    [Serializable]
    public class AccountDto
    {
        public string Account;
        public string Password;

        public AccountDto()
        {
        }

        public AccountDto(string account, string password)
        {
            Account = account;
            Password = password;
        }
    }
}