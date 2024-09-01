using ChatServer.Net.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public class Client
    {
        //用来存放listener.AcceptTcpClient返回的TCPClient，以及Username并分配UID
        public TcpClient ClientSocket;
        public string username;
        public string userColorCode;
        public Guid UID;
        public PacketReader packetReader;
        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();
            packetReader = new PacketReader(ClientSocket.GetStream());
            //这里的opcode的操作码是0，接收注册的客户端发来的信息。
            var opcode = packetReader.ReadByte();
            username = packetReader.ReadMessage();
            userColorCode = packetReader.ReadMessage();
            Console.WriteLine($"[{DateTime.Now}]:Client has connected with Username:{username}");
            Task.Run(() => { ProcessMessageFromChatClient(); });
        }
        private void ProcessMessageFromChatClient()
        {
            while (true)
            {
                try
                {
                    var opcode = packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 1:Program.UnicastBroadcastStateMessage(this); break;
                        case 3:
                            var msg = packetReader.ReadMessage();
                            Program.BroadcastMessage(msg,UID.ToString());
                            break;
                        default: break;
                    }
                }
                catch
                {
                    Console.WriteLine($"[{UID.ToString()}]:{username} Disconnected!");
                    Program.BroadcastDisconnectedMessage(UID.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}
