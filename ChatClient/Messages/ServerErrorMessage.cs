using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Messages
{
    public class ServerErrorMessage
    {
        public string content {  get;  }
        public ServerErrorMessage(string content)
        {
            this.content = content;
        }

    }
}
