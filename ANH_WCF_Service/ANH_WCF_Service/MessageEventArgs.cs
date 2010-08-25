using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ANH_WCF_Interface;

namespace ANH_WCF_Service
{
    public class MessageEventArgs:EventArgs
    {
        public MessageType messagetype;
        public ServerType type;
        public String args;
        public String Message;
        public MessageEventArgs(){;}
        public MessageEventArgs(MessageType messtype, ServerType servertype, String arguments, String message)
        {
            messagetype = messtype;
            type = servertype;
            args = arguments;
            Message = message;
        }
    }
}
