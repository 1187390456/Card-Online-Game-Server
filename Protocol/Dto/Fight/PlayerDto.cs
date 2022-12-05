using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Dto.Fight
{
    /// <summary>
    /// 玩家传输模型
    /// </summary>
    [Serializable]
    public class PlayerDto
    {
        public int Id; // 标识
        public int Identify; // 身份
        public List<CardDto> Cards; // 自己拥有的手牌

        public bool HasCard() => Cards.Count != 0; // 是否有手牌

        public void AddCard(CardDto cardDto) => Cards.Add(cardDto); // 添加手牌

        public void RemoveCard(CardDto cardDto) => Cards.Remove(cardDto); // 移除手牌

        public int GetCardCount() => Cards.Count; // 当前卡片数量
    }
}