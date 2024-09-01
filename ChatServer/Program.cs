using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using ChatServer.Net.IO;

namespace ChatServer
{
    public class Program
    {
        static TcpListener listener;
        static List<Client> users;
        static void Main(string[] args)
        {
            //将控制台命令窗口转为PowerShell以支持UTF-8的表情
            Process process = new Process();
            process.StartInfo.FileName = "powershell.exe";
            process.StartInfo.Arguments = "-noexit"; // 保持PowerShell窗口打开
            process.Start();
            //—————————————————一以下是挂起服务器监听———————————————————————————//
            users = new List<Client>();
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7891);
            listener.Start();
            //保持监听，一有监听就创建Client对象
            while (true)
            {
                Client client = new Client(listener.AcceptTcpClient());
                users.Add(client);
            }
        }
        public static void UnicastBroadcastStateMessage(Client client)
        {
            //单播告诉当前连接的客户端在服务器的个人信息以及其它在线用户
            PacketBuilder builder = new PacketBuilder();
            builder.WriteOpCode(1);
            builder.WriteMessage(client.username);
            builder.WriteMessage(client.userColorCode);
            builder.WriteMessage(client.UID.ToString());
            client.ClientSocket.Client.Send(builder.GetPacketBytes());
            foreach (var other in users.Where(x => x.UID.ToString() != client.UID.ToString()))
            {
                PacketBuilder builderElse = new PacketBuilder();
                builderElse.WriteOpCode(2);
                builderElse.WriteMessage(other.username);
                builderElse.WriteMessage(other.userColorCode);
                builderElse.WriteMessage(other.UID.ToString());
                client.ClientSocket.Client.Send(builderElse.GetPacketBytes());
            }
            //向除当前新连接的用户以外的其它用户更新新连接的用户
            foreach (var user in users.Where(x => x.UID.ToString() != client.UID.ToString())) 
            {
                PacketBuilder builderElse = new PacketBuilder();
                builderElse.WriteOpCode(2);
                builderElse.WriteMessage(client.username);
                builderElse.WriteMessage(client.userColorCode);
                builderElse.WriteMessage(client.UID.ToString());
                user.ClientSocket.Client.Send(builderElse.GetPacketBytes());
            }
        }
        public static void BroadcastMessage(string message,string UID)
        {
            foreach (var user in users)
            {
                var msgpacket = new PacketBuilder();
                msgpacket.WriteOpCode(3);
                msgpacket.WriteMessage(UID);
                msgpacket.WriteMessage(DateTime.Now.ToString());    
                msgpacket.WriteMessage(message);
                user.ClientSocket.Client.Send(msgpacket.GetPacketBytes());
            }
        }
        public static void BroadcastDisconnectedMessage(string UID)
        {
            var disconnectedUser = users.Where(x => x.UID.ToString() == UID).FirstOrDefault();
            users.Remove(disconnectedUser);
            foreach (var user in users)
            {
                var brocastBuilder = new PacketBuilder();
                brocastBuilder.WriteOpCode(4);
                brocastBuilder.WriteMessage(UID);
                user.ClientSocket.Client.Send(brocastBuilder.GetPacketBytes());
            }
        }
    }
}
