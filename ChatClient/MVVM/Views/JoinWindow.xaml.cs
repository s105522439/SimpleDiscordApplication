using ChatClient.Messages;
using ChatClient.MVVM.ViewModels;
using ChatClient.Net;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChatClient.MVVM.Views
{
    /// <summary>
    /// Interaction logic for JoinWindow.xaml
    /// </summary>
    public partial class JoinWindow : Window
    {
        public JoinViewModel joinViewModel;
        public JoinWindow()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.Register<ServerErrorMessage>(this,(recipient, message) => { MessageBox.Show(message.content); });
            joinViewModel = this.DataContext as JoinViewModel;
            WeakReferenceMessenger.Default.Register<JoinSuccessMessage>(this, JoinSuccess);
        }
        public void JoinSuccess(object sender, JoinSuccessMessage message)
        {
            MainViewModel mainViewModel = new MainViewModel(joinViewModel.Server);
            MainWindow mainWindow = new MainWindow();
            mainWindow.DataContext = mainViewModel;
            mainWindow.Show();
            this.Close();
        }
        public void MainWindow_OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
