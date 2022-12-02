using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Constant
{
    public class ChatConstant
    {
        public static Dictionary<int, string> chatTypeTextDic = new Dictionary<int, string>(); // 消息索引 文本

        static ChatConstant()
        {
            chatTypeTextDic.Add(0, "大家好! 很高兴见到各位!");
            chatTypeTextDic.Add(1, "和你合作真是太愉快了!");
            chatTypeTextDic.Add(2, "快点吧! 我等的花儿都谢了!");
            chatTypeTextDic.Add(3, "你的牌打得太好啦!");
            chatTypeTextDic.Add(4, "不要吵了! 有什么好吵的! 专心玩游戏吧!");
            chatTypeTextDic.Add(5, "断线啦! 网络怎么这么差!");
            chatTypeTextDic.Add(6, "各位真不好意思我要离开一会!");
            chatTypeTextDic.Add(7, "不要走! 决战到天亮!");
            chatTypeTextDic.Add(8, "你是MM! 还是GG");
            chatTypeTextDic.Add(9, "我们交个朋友吧! 能不能告诉我你的联系方法呀!");
            chatTypeTextDic.Add(10, "再见了! 我会想念大家的!");
        }

        public static string GetChatText(int index) => chatTypeTextDic[index];
    }
}