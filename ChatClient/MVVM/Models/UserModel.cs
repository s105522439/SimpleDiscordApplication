using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ChatClient.MVVM.Models
{
    public class UserModel
    {
        public string Username { get; set; }
        public Brush UserColor {  get; set; }
        public string UID { get; set; }
        public string LastMessage {  get; set; }
    }
}
