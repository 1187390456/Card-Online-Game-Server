using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Dto
{
    /// <summary>
    /// 房间传输数据模型
    /// </summary>
    [Serializable]
    public class MatchRoomDto
    {
        public Dictionary<int, UserDto> uidUserDic; // 用户id 对应 用户传输数据模型

        public List<int> readyList; // 准备玩家列表
    }
}