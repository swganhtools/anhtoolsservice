using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ANH_WCF_Interface;
using ANH_WCF_Example_Client.ANHService;

namespace ANH_WCF_Example_Client
{
    public class AnhCallback : IAnhServiceCallback
    {
        public event EventHandler<MessageEventArgs> MessageReceived;
        public void ServerMessage(ServerType type, String args, MessageType messtype, String Message)
        {
            if (MessageReceived != null)
            {
                MessageEventArgs mea = new MessageEventArgs();
                mea.Message = Message;
                mea.Args = args;
                mea.ServerType = type;
                mea.MessageType = messtype;
                MessageReceived(this, mea);
            }
        }
        public void TestEventSystem()
        {
            if (MessageReceived != null)
            {
                MessageEventArgs mea = new MessageEventArgs();
                mea.Message = "Test Message";
                mea.Args = "";
                mea.ServerType = ServerType.ChatServer;
                MessageReceived(this, mea);
            }
        }
    }
}
