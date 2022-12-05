using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Dto.Fight
{
    /// <summary>
    /// 卡牌传输模型
    /// </summary>
    [Serializable]
    public class CardDto
    {
        public string Name; // 名称
        public int Color; // 颜色
        public int Weight; // 权重
    }
}