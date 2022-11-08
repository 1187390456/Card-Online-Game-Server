using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
            if (dataCache.Count < 4) return null;

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
                    if (dataRemainLength < len) return null;
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

        #region 构造发送的SocketMsg类

        /// <summary>
        /// 把Socket类转成字节数组 发送出去
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] EncodeMsg(SocketMsg msg)
        {
            MemoryStream ms = new MemoryStream(); // 二进制流对象
            BinaryWriter bw = new BinaryWriter(ms); // 二进制写入对象
            bw.Write(msg.OpCode);
            bw.Write(msg.SubCode);

            // 参数不为空时 才可以将Object转字节数据 存起来
            if (msg.Value != null)
            {
                byte[] valueBytes = EncodeObj(msg.Value); // 序列化 对象转字节数组
                bw.Write(valueBytes);
            }
            // 克隆字节数组
            byte[] data = new byte[ms.Length];
            Buffer.BlockCopy(ms.GetBuffer(), 0, data, 0, (int)ms.Length);
            // 关闭流对象
            bw.Close();
            ms.Close();
            return data;
        }

        /// <summary>
        /// 将收到的字节数据 转成 socketMsg对象 供我们使用
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static SocketMsg DecodeMsg(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryReader br = new BinaryReader(ms);
            SocketMsg msg = new SocketMsg();
            msg.OpCode = br.ReadByte();
            msg.SubCode = br.ReadByte();

            // 还有剩余字节未读取 代表value有值
            if (ms.Length > ms.Position)
            {
                byte[] valueBytes = br.ReadBytes((int)(ms.Length - ms.Position));
                object value = DecodeObj(valueBytes); // 序列化 字节数组转对象
                msg.Value = value;
            }
            br.Close();
            ms.Close();
            return msg;
        }

        #endregion 构造发送的SocketMsg类

        #region 把一个object类型转换成byte[]

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] EncodeObj(object value)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter(); // 二进制格式化对象

                bf.Serialize(ms, value); // 序列化

                // 克隆 序列化后的字节数组
                byte[] valueBytes = new byte[ms.Length];
                Buffer.BlockCopy(ms.GetBuffer(), 0, valueBytes, 0, (int)ms.Length);

                return valueBytes;
            }
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="valueBytes"></param>
        /// <returns></returns>
        public static Object DecodeObj(byte[] valueBytes)
        {
            using (MemoryStream ms = new MemoryStream(valueBytes))
            {
                BinaryFormatter bf = new BinaryFormatter();
                var value = bf.Deserialize(ms); // 反序列化
                return value;
            }
        }

        #endregion 把一个object类型转换成byte[]
    }
}