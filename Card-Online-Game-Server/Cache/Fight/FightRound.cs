using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card_Online_Game_Server.Cache.Fight
{
    /// <summary>
    /// 回合管理
    /// </summary>
    public class FightRound
    {
        public int CurrentUid; // 当前出牌者

        public int LastUid; // 上一次出牌者

        public int LastCardLength; // 上次出牌的长度

        public int LastCardWeight; // 上次出牌的权值

        public int LastCardType; // 上次出牌的类型

        //  开始出牌 第一次出牌或主动出牌
        public void Start(int uid)
        {
            CurrentUid = uid;
            LastUid = uid;
        }

        // 改变出牌 管牌
        public void Change(int length, int type, int weight, int uid)
        {
            LastCardLength = length;
            LastCardWeight = weight;
            LastCardType = type;
            LastUid = uid;
        }

        // 轮换 不出
        public void Turn(int uid)
        {
            CurrentUid = uid;
        }

        public FightRound()
        {
            CurrentUid = -1;
            LastUid = -1;

            LastCardLength = -1;
            LastCardWeight = -1;
            LastCardType = -1;
        }

        public void Init()
        {
            CurrentUid = -1;
            LastUid = -1;

            LastCardLength = -1;
            LastCardWeight = -1;
            LastCardType = -1;
        }
    }
}