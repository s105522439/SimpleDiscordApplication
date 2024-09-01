using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Messages
{
    public class JoinSuccessMessage
    {
        public string content { get; }
        public JoinSuccessMessage(string content)
        {
            this.content = content;
        }
    }
}
