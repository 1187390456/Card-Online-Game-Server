using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Dto
{
    /// <summary>
    /// 聊天传输模型
    /// </summary>
    [Serializable]
    public class ChatDto
    {
        public int id; // 用户id
        public int Index; // 消息索引
        public string text;  // 内容
    }
}