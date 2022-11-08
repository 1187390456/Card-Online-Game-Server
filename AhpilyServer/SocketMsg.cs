using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhpilyServer
{
    /// <summary>
    /// 网络消息
    /// </summary>
    public class SocketMsg
    {
        public SocketMsg()
        {
        }

        public SocketMsg(int opCode, int subCode, object value)
        {
            OpCode = opCode;
            SubCode = subCode;
            Value = value;
        }

        public int OpCode { get; set; }
        public int SubCode { get; set; }
        public object Value { get; set; }
    }
}