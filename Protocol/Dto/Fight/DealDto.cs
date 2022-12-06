using Protocol.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Dto.Fight
{
    /// <summary>
    /// 出牌传输模型
    /// </summary>
    [Serializable]
    public class DealDto
    {
        public List<CardDto> SelectCardList; // 选中要出的牌

        public int Length; // 长度

        public int Weight; // 权值

        public int Type; // 类型

        public int Uid; // 出牌角色

        public bool IsRegular; // 是否合法

        public DealDto()
        {
        }

        public DealDto(List<CardDto> cardList, int uid)
        {
            SelectCardList = cardList;
            Uid = uid;
            Length = cardList.Count;

            Weight = CardWeight.GetWeight(cardList, Type);
        }
    }
}