using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Dto
{
    /// <summary>
    /// 轮换传输模型
    /// </summary>
    [Serializable]
    public class TurnDto
    {
        public int currentId;
        public int nextId;
        public bool isFirst;
        public TurnDto()
        {

        }
        public TurnDto(int currentId, int nextId, bool isFirst)
        {
            this.currentId = currentId;
            this.nextId = nextId;
            this.isFirst = isFirst;
        }
    }
}
