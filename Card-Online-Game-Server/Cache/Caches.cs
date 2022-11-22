using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card_Online_Game_Server.Cache
{
    /// <summary>
    /// 全局缓存空间
    /// </summary>
    public class Caches
    {
        public static AccountCache AccountCache { get; set; }
        public static UserCache UserCache { get; set; }

        static Caches()
        {
            AccountCache = new AccountCache();
            UserCache = new UserCache();
        }
    }
}