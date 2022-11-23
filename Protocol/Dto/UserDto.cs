using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Dto
{
    /// <summary>
    /// 角色传输数据模型 所有传输数模模型必须序列化
    /// </summary>
    [Serializable]
    public class UserDto
    {
        public string Avatar; // 头像
        public string AvatarMask; // 头像框
        public string Name; // 名称
        public string RankLogo; // 排位logo
        public string RankName; // 排位名称
        public string GradeLogo; // 等级logo
        public string GradeName; // 等级名称

        public UserDto()
        {
        }

        public UserDto(string avatar, string avatarMask, string name, string rankLogo, string rankName, string gradeLogo, string gradeName)
        {
            Avatar = avatar;
            AvatarMask = avatarMask;
            Name = name;
            RankLogo = rankLogo;
            RankName = rankName;
            GradeLogo = gradeLogo;
            GradeName = gradeName;
        }
    }
}