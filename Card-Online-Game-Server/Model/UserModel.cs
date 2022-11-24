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
        public int AccountId; // 账号id
        public int Id; // 角色id
        public string Avatar; // 头像地址
        public string AvatarMask; // 头像框地址
        public string Name; // 名称
        public string RankLogo; // 排位图标地址
        public string RankName; // 排位等级名称
        public string GradeLogo;// 等级图标地址
        public string GradeName; // 等级名称

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