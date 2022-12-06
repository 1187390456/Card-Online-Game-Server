using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Dto
{
    /// <summary>
    /// 抢地主传输模型
    /// </summary>
    [Serializable]
    public class GrabDto
    {
        public int Uid;
        public List<CardDto> TableCardList = new List<CardDto>();

        public GrabDto()
        {
        }

        public GrabDto(int uid, List<CardDto> tableCardList)
        {
            Uid = uid;
            TableCardList = tableCardList;
        }
    }
}