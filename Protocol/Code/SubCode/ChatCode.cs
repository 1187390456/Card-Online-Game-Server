using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Code
{
    public class ChatCode
    {
        //  发送快捷消息

        public const int Send_Quick_Cres = 0;
        public const int Send_Quick_Bro = 1;

        // 发送自定义消息

        public const int Send_ZiDingYi_Cres = 2;
        public const int Send_ZiDingYi_Bro = 3;

        // 发送表情

        public const int Send_Emoji_Cres = 4;
        public const int Send_Emoji_Bro = 5;
    }
}