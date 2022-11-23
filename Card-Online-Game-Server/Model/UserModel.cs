using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card_Online_Game_Server.Model
{
    /// <summary>
    /// 账号数据模型
    /// </summary>
    public class UserModel
    {
        public int AccountId; // 关联的账号id
        public int Id; // 标识id

        public string Avatar;
        public string AvatarMask;
        public string Name;
        public string RankLogo;
        public string RankName;
        public string GradeLogo;
        public string GradeName;

        public UserModel(int accountId, int id, string name)
        {
            AccountId = accountId;
            Id = id;

            Name = name;

            // 初始默认值
            Avatar = "https://tse1-mm.cn.bing.net/th/id/OIP-C.QHGqzm6IPJ7QGhjeU31o0wHaNK?w=187&h=333&c=7&r=0&o=5&pid=1.7";
            AvatarMask = "https://tse1-mm.cn.bing.net/th/id/OIP-C.qXQ_cEdU0gbtmHArTfem4QHaFS?w=266&h=190&c=7&r=0&o=5&pid=1.7";
            RankLogo = "https://tse1-mm.cn.bing.net/th/id/OIP-C.qXQ_cEdU0gbtmHArTfem4QHaFS?w=266&h=190&c=7&r=0&o=5&pid=1.7";
            RankName = "最强王者";
            GradeLogo = "https://tse1-mm.cn.bing.net/th/id/OIP-C.qXQ_cEdU0gbtmHArTfem4QHaFS?w=266&h=190&c=7&r=0&o=5&pid=1.7";
            GradeName = "皇帝";
        }
    }
}