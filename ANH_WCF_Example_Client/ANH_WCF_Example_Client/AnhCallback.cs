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
        public event EventHandler<ServerStatusEventArgs> StatusReceived;
        public event EventHandler<AvailableServerEventArgs> AvailableServerListReceived;
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

        #region IAnhServiceCallback Members
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

        public void ServerStatus(object[] status)
        {
            //List<IServerStatus> meh = new List<IServerStatus>((IServerStatus[])status);
            List<IServerStatus> meh = new List<IServerStatus>(status.Length);
            foreach (IServerStatus s in status)
            {
                meh.Add(s);
            }

            if (StatusReceived != null)
            {
                ServerStatusEventArgs ssea = new ServerStatusEventArgs();
                ssea.StatusList = meh;
                StatusReceived(this, ssea);
            }
        }

        public void AvailableServers(ServerType[] servers)
        {
            List<ServerType> meh = new List<ServerType>(servers);
            if (AvailableServerListReceived != null)
            {
                AvailableServerEventArgs asea = new AvailableServerEventArgs();
                asea.ServerList = meh;
                AvailableServerListReceived(this, asea);
            }
        }

        #endregion
    }
}
