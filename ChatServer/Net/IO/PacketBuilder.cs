using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Net.IO
{
    //每次创建一个ms的实例，再打包。
    public class PacketBuilder
    {
        MemoryStream _ms;
        public PacketBuilder()
        {
            _ms = new MemoryStream();
        }
        //以下三个方法将信息转成二进制的形式打包成数组，用于TCP的数据传输
        public void WriteOpCode(byte opcode)
        {
            _ms.WriteByte(opcode);
        }
        public void WriteMessage(string msg)
        {
            // 获取UTF-8编码的字节数组
            byte[] msgBytes = Encoding.UTF8.GetBytes(msg);
            // 获取字节数组的长度
            int msgLength = msgBytes.Length;
            // 将长度这个整形数字转为Byte，并将其写入内存流中
            _ms.Write(BitConverter.GetBytes(msgLength), 0, sizeof(int));
            // 将UTF-8编码的字节数组写入流中
            _ms.Write(msgBytes, 0, msgBytes.Length);
        }
        //返回打包好的数据流
        public byte[] GetPacketBytes() { return _ms.ToArray(); }
    }
}
