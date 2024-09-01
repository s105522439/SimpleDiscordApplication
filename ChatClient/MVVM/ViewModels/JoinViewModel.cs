using ChatClient.Messages;
using ChatClient.Net;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatClient.MVVM.ViewModels
{
    public class JoinViewModel
    {
        public Server Server { get; }
        private string username;
        public string Username { get { return username; } set { username = value; JoinCommand.NotifyCanExecuteChanged(); } }
        public RelayCommand JoinCommand { get; set; }
        public JoinViewModel() 
        {
            Server = new Server();
            JoinCommand = new RelayCommand(() => Server.ConnectToServer(username), () => !string.IsNullOrWhiteSpace(username));
        }
    }
}
