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
        /// <param name="data">需要发送的数据 字节数组</param>
        /// <returns></returns>
        public static byte[] EncodeMessage(byte[] data)
        {
            // using关键字 执行后自动释放资源
            using (MemoryStream ms = new MemoryStream()) // 初始化内存流对象
            {
                using (BinaryWriter bw = new BinaryWriter(ms)) // 二级制写入内存流对象
                {
                    bw.Write(data.Length); // 写入长度
                    bw.Write(data); // 写入数据

                    byte[] byteArray = new byte[(int)ms.Length]; // 字节数组
                    //赋值字节数组 内存复制 数据源 偏移量 目标源 目标偏移 复制数量
                    Buffer.BlockCopy(ms.GetBuffer(), 0, byteArray, 0, (int)ms.Length);

                    return byteArray;
                }
            }
        }

        /// <summary>
        /// 解析消息体 从缓存中取出一个一个完整额数据包
        /// </summary>
        /// <param name="dataCache">数据缓存列表</param>
        /// <returns></returns>
        public static byte[] DecodeMessage(ref List<byte> dataCache) // ref 作用 在方法里修改会同步到外部的值
        {
            // 4字节 构成一个int长度 小于4 不能构成一个完整的消息
            if (dataCache.Count < 4) throw new Exception("数据缓存长度小于4 不能构成一个完整的消息");

            // using关键字 执行后自动释放资源
            using (MemoryStream ms = new MemoryStream(dataCache.ToArray())) // 初始化内存流对象
            {
                using (BinaryReader br = new BinaryReader(ms)) // 二级制读取内存流对象
                {
                    // 获取读取数据的长度 即约定的传输长度
                    var len = br.ReadInt32();
                    // 剩余长度= 总长度 - 当前读取位置
                    var dataRemainLength = (int)(ms.Length - ms.Position);
                    // 判断 数据剩余长度 是否大于 读取数据的长度
                    if (dataRemainLength < len) throw new Exception("数据长度不够包头约定的长度 不能构成一个完整的消息");
                    // 读取剩余的数据
                    byte[] data = br.ReadBytes(len);
                    // 更新数据缓存
                    dataCache.Clear(); // 清空之前数据
                    dataCache.AddRange(br.ReadBytes(dataRemainLength)); //添加剩余数据

                    return data;
                }
            }
        }

        #endregion 黏包拆包问题 封装有规定的数据包
    }
}