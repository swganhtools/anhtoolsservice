using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ANH_WCF_Interface;

namespace ANH_WCF_Example_Client
{
    public class MessageEventArgs : EventArgs
    {
        public ServerType ServerType { get; set; }
        public String Args { get; set; }
        public String Message { get; set; }
        public MessageType MessageType { get; set; }
    }
    public class ServerStatusEventArgs : EventArgs
    {
        public List<IServerStatus> StatusList { get; set; }
    }
    public class AvailableServerEventArgs : EventArgs
    {
        public List<ServerType> ServerList { get; set; }
    }
}
