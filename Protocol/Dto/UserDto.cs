using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Dto
{
    /// <summary>
    /// 角色传输数据模型 所有传输数模模型必须序列化 使用默认构造赋值
    /// </summary>
    [Serializable]
    public class UserDto
    {
        public int Id; // ID
        public string Avatar; // 头像
        public string AvatarMask; // 头像框
        public string Name; // 名称
        public string RankLogo; // 排位logo
        public string RankName; // 排位名称
        public string GradeLogo; // 等级logo
        public string GradeName; // 等级名称
        public string BeanCount; // 豆子数量
        public string DiamondCount; // 钻石数量
    }
}