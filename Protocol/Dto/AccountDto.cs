using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Dto
{
    /// <summary>
    /// 账号传输数据模型 所有传输数模模型必须序列化 使用默认构造赋值
    /// </summary>
    [Serializable]
    public class AccountDto
    {
        public string Account; // 账号
        public string Password; // 密码
    }
}