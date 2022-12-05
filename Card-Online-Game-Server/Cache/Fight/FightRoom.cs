using Protocol.Dto.Fight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card_Online_Game_Server.Cache.Fight
{
    public class FightRoom
    {
        public int Id; // 标识id

        public List<PlayerDto> playerList; // 玩家列表

        public List<int> runAwayLists; // 逃跑玩家列表
    }
}