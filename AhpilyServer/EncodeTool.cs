using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhpilyServer
{
    /// <summary>
    /// 关于编码工具类
    /// </summary>
    public static class EncodeTool
    {
        #region 黏包拆包问题 封装有规定的数据包

        /// <summary>
        /// 构造消息体 : 消息头 + 消息尾
        /// </summary>
        /// <param name="value">需要发送的数据 字节数组</param>
        /// <returns></returns>
        public static byte[] EncodeMessage(byte[] data)
        {
            // using关键字 执行后自动释放资源
            using (MemoryStream ms = new MemoryStream()) // 内存流对象
            {
                using (BinaryWriter bw = new BinaryWriter(ms)) // 初始化二级制写入对象
                {
                    bw.Write(data.Length); // 写入长度
                    bw.Write(data); // 写入数据

                    byte[] byteArray = new byte[(int)ms.Length];
                }
            }
        }

        #endregion 黏包拆包问题 封装有规定的数据包
    }
}