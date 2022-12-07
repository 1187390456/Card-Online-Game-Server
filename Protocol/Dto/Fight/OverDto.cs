using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Dto
{
    /// <summary>
    /// 游戏结束传输
    /// </summary>
    [Serializable]
    public class OverDto
    {
        public List<int> WinLists = new List<int>();
        public List<int> FailLists = new List<int>();
        public List<int> LeaveLists = new List<int>();
        public int BeanCount;

        public OverDto()
        {
        }

        public OverDto(int beanCount, List<int> winList, List<int> failLists, List<int> leaveLists)
        {
            WinLists = winList;
            FailLists = failLists;
            LeaveLists = leaveLists;
            BeanCount = beanCount;
        }
    }
}