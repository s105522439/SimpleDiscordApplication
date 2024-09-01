using ChatClient.MVVM.Models;
using ChatClient.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ChatClient.MVVM.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private Server server;
        private string username;
        private Brush userColor;
        private string uid;
        private string message;
        public string Username { get { return username; } set { username = value;OnPropertyChanged(); } }
        public Brush UserColor { get { return userColor; } set {  userColor = value;OnPropertyChanged(); } }   
        public string UID { get { return uid; } set { uid = value; OnPropertyChanged(); } }
        public string Message { get { return message; }set { message = value; OnPropertyChanged(); } }
        public ObservableCollection<UserModel> OtherOnlineUsers { get; set; }
        public ObservableCollection<MessageModel> Messages { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        public MainViewModel(Server server)
        {
            this.server = server;
            Messages = new ObservableCollection<MessageModel>();
            OtherOnlineUsers = new ObservableCollection<UserModel>();
            server.IninitializeUserEvent += IninitializeUser;
            server.IninitializeOtherOnlineUsersEvent += InitializeOtherOnlineUsers;
            server.MessageReceivedEvent += MessageReceived;
            server.UserDisconnectedEvent += UserDisconnected;
            server.Ininitialize();
            SendMessageCommand = new RelayCommand(() => { server.SendMessageToServer(Message); Message = ""; }, () => !string.IsNullOrWhiteSpace(Message));
        }

        private void InitializeOtherOnlineUsers()
        {
            //涉及UI操作的都在UI线程中进行，不然容易报错，主要是这里是被监听的分支线程调用的。
            Application.Current.Dispatcher.Invoke(() =>
            {
                UserModel otherUser = new UserModel();
                otherUser.Username = server.reader.ReadMessage();
                string ColorCode = server.reader.ReadMessage();
                otherUser.UserColor = (Brush)new BrushConverter().ConvertFromString(ColorCode);
                otherUser.UID = server.reader.ReadMessage();
                OtherOnlineUsers.Add(otherUser);
            });
        }

        private void IninitializeUser()
        {
            //同理，Wpf，UI操作不在Ui线程容易出现异常
            Application.Current.Dispatcher.Invoke(() =>
            {
                Username = server.reader.ReadMessage();
                string ColorCode = server.reader.ReadMessage();
                UserColor = (Brush)new BrushConverter().ConvertFromString(ColorCode);
                UID = server.reader.ReadMessage();
            });
        }
        private void MessageReceived()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageModel message = new MessageModel();
                message.UID = server.reader.ReadMessage();
                message.Time= server.reader.ReadMessage();
                message.Message = server.reader.ReadMessage();
                if (OtherOnlineUsers.Any(o => o.UID == message.UID))
                {
                    var user = OtherOnlineUsers.Where(o => o.UID == message.UID).FirstOrDefault();
                    message.Username = user.Username;
                    message.UserColor = user.UserColor;
                }
                else
                {
                    message.Username = Username;
                    message.UserColor = UserColor;
                }
                Messages.Add(message);
            });
        }
        private void UserDisconnected()
        {
            Application.Current.Dispatcher.Invoke(() => 
            {
                var UID=server.reader.ReadMessage();
                var User=OtherOnlineUsers.Where(o => o.UID==UID).FirstOrDefault();
                OtherOnlineUsers.Remove(User);
            });
        }
    }
}
