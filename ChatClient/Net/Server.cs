using ChatClient.Generators;
using ChatClient.Messages;
using ChatClient.MVVM.Models;
using ChatClient.Net.IO;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Net
{
    public class Server
    {
        public TcpClient client;
        public PacketReader reader;
        public event Action IninitializeUserEvent;
        public event Action IninitializeOtherOnlineUsersEvent;
        public event Action MessageReceivedEvent;
        public event Action UserDisconnectedEvent;
        public Server()
        {
            client = new TcpClient();
        }
        public void ConnectToServer(String username)
        {
            try
            {
                //Connect如果连接不到会直接扔Exception，所以这里只需要try，catch就能判断是否连接成功。
                client.Connect("127.0.0.1", 7891);
                PacketBuilder builder = new PacketBuilder();
                builder.WriteOpCode(0);
                builder.WriteMessage(username);
                RandomColorGenerator generator = new RandomColorGenerator();
                string UserColorCode = generator.GenerateColorCode();
                builder.WriteMessage(UserColorCode);
                client.Client.Send(builder.GetPacketBytes());
                var message = new JoinSuccessMessage("成功加入");
                WeakReferenceMessenger.Default.Send(message);
                reader = new PacketReader(client.GetStream());
                ReadFromChatServer();
            }
            catch 
            {
                var message = new ServerErrorMessage("找不到服务器");
                WeakReferenceMessenger.Default.Send(message);
            }
        }
        public void Ininitialize()
        {
            PacketBuilder builder = new PacketBuilder();
            builder.WriteOpCode(1);
            builder.WriteMessage("获取初始信息，包括此客户端的用户名和UID，以及当前服务器在线用户列表");
            client.Client.Send(builder.GetPacketBytes());
        }
        public void ReadFromChatServer()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var opcode = reader.ReadByte();
                    switch (opcode)
                    {
                        case 1: IninitializeUserEvent.Invoke(); break;
                        case 2: IninitializeOtherOnlineUsersEvent.Invoke(); break;
                        case 3: MessageReceivedEvent.Invoke(); break;
                        case 4: UserDisconnectedEvent.Invoke(); break; 
                        default:; break;
                    }
                }
            });
        }
        public void SendMessageToServer(string message)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(3);
            messagePacket.WriteMessage(message);
            client.Client.Send(messagePacket.GetPacketBytes());
        }
    }
}
