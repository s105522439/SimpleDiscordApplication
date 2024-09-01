using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Net.IO
{
    public class PacketReader : BinaryReader
    {
        private NetworkStream _stream;
        public PacketReader(NetworkStream stream) : base(stream)
        {
            _stream = stream;
        }
        public string ReadMessage()
        {
            //之所以是ReadInt32因为执行ReadMessage之前我们肯定从Client里先读取了一个字节的操作码
            //从当前流的位置开始读取 4 个字节，并将它们解释为一个 Int32 类型的整数，其实就是字符串长度用来解码
            //读取完的4个字节会从流中消失，剩下的全是string转成的二进制。
            var length = ReadInt32();
            //创建字节流用来存放字符所在的字节流数据
            byte[] msgBuffer = new byte[length];
            _stream.Read(msgBuffer, 0, length);
            //将字节数组中的每个字节按照 UTF8 编码转换为对应的字符，并将这些字符组合成一个字符串。
            var msg = Encoding.UTF8.GetString(msgBuffer);
            return msg;
        }
    }
}
