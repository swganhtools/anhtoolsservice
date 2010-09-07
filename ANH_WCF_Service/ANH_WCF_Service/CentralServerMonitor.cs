using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProcessCallerLibrary;
using ANH_WCF_Interface;
using System.IO;
using System.ComponentModel;

namespace ANH_WCF_Service
{
    public class CentralServerMonitor : ISynchronizeInvoke
    {
        ISynchronizeInvoke m_Synchronizer = new Synchronizer();
        public event EventHandler<MessageEventArgs> MessageReceived;

        private static CentralServerMonitor csm = null;
        private CentralServerMonitor()
        {
        }
        public static CentralServerMonitor GetCentralMonitor()
        {
            if (csm == null) csm = new CentralServerMonitor();
            return csm;
        }
        internal List<Server> ServerList = new List<Server>();
        private String WorkingDirectory = "";
        internal List<ServerType> AvailableServers = new List<ServerType>();
        internal void SetWorkingDirectory(String dir)
        {
            WorkingDirectory = dir;
        }
        

        public String AddServer(Server s)
        {
            if (s.type == ServerType.None)
                return "StartServer Called however Server Type is None. Aborting";

            //see if we dont already have an existing server
            Server e = FindServer(s.type, s.args);
            if (e == null) //if we don't set e as the newly created server (otherwise we use the old one and discard the new one)
            {
                e = s;
                ServerList.Add(e);
            }

            //if our server is running
            if (e.IsRunning)
            {
                return "Server: "+e.type + " args: "+e.args+" is already running";
            }
            
            //setup the events
            e.Process.Cancelled += new EventHandler(pc_Cancelled);
            e.Process.Completed += new EventHandler(pc_Completed);
            e.Process.Failed += new System.Threading.ThreadExceptionEventHandler(pc_Failed);
            e.Process.StdErrReceived += new DataReceivedHandler(pc_StdErrReceived);
            e.Process.StdOutReceived += new DataReceivedHandler(pc_StdOutReceived);

            //set the timers/flags and run
            e.StartedTime = DateTime.Now.Ticks;
            e.IsRunning = true;
            e.IsCrashed = false;
            e.Process.Start();
            
            //send the started message
            return "Started Server. Type: " + e.type.ToString() + " Args: " + e.args;
        }
        public String RemoveServer(Server s)
        {
            Server e = FindServer(s.Process);
            if (e == null) return "Unable to find Server type: " + s.type + " args:" + s.args;

            e.Process.Cancelled -= new EventHandler(pc_Cancelled);
            e.Process.Completed -= new EventHandler(pc_Completed);
            e.Process.Failed -= new System.Threading.ThreadExceptionEventHandler(pc_Failed);
            e.Process.StdErrReceived -= new DataReceivedHandler(pc_StdErrReceived);
            e.Process.StdOutReceived -= new DataReceivedHandler(pc_StdOutReceived);
            e.Process.Cancel(); //TODO: investigate whether cancelandwait or addoutput("q") is more appropriate
            
            ServerList.Remove(e);

            return "Stopped Server type: " + s.type + " args:" + s.args;
        }
        internal void GetListOfServersAvailable()
        {
            if (AvailableServers.Count > 0) AvailableServers.Clear();

            if (String.Compare(WorkingDirectory, "") == 0) return;

            List<String> temp = new List<string>();
            temp.AddRange(Directory.EnumerateFiles(WorkingDirectory, "*.exe", SearchOption.TopDirectoryOnly));

            foreach (String s in temp)
            {
                String t = s.Remove(0,WorkingDirectory.Length);
                switch (t.ToLower())
                {
                    case "connectionserver.exe":
                        AvailableServers.Add(ServerType.ConnectionServer);
                        break;
                    case "chatserver.exe":
                        AvailableServers.Add(ServerType.ChatServer);
                        break;
                    case "loginserver.exe":
                        AvailableServers.Add(ServerType.LoginServer);
                        break;
                    case "pingserver.exe":
                        AvailableServers.Add(ServerType.PingServer);
                        break;
                    case "zoneserver.exe":
                        AvailableServers.Add(ServerType.ZoneServer);
                        break;
                    default:
                        break;
                }
            }
        }
        internal Server FindServer(ProcessCaller pc)
        {
            foreach (Server s in ServerList)
            {
                if (s.Process == pc)
                    return s;
            }
            return null;
        }
        internal Server FindServer(ServerType type, String args)
        {
            foreach (Server s in ServerList)
            {
                if (s.type == type)
                {
                    if(String.Compare(s.args.ToLower(), args.ToLower())==0)
                        return s;
                }
            }
            return null;
        }
        private ServerType GetServerType(string p)
        {
            String[] temp = p.Split(new char[]{'\\','.'}, StringSplitOptions.RemoveEmptyEntries);
            String Filename = "";
            if (temp.Length >= 2)
            {
                Filename = temp[temp.Length - 2];
            }
            switch (Filename.ToLower())
            {
                case "connectionserver":
                    return ServerType.ConnectionServer;
                case "chatserver":
                    return ServerType.ChatServer;
                case "loginserver":
                    return ServerType.LoginServer;
                case "pingserver":
                    return ServerType.PingServer;
                case "zoneserver":
                    return ServerType.ZoneServer;
                default:
                    return ServerType.None;
            }
        }
        void pc_StdOutReceived(object sender, DataReceivedEventArgs e)
        {
            if(MessageReceived != null)
            {
                ProcessCaller pc = (ProcessCaller)sender;
                ServerType st = GetServerType(pc.FileName);
                String args = pc.Arguments;
                MessageReceived(this, new MessageEventArgs(MessageType.Message, st, args, e.Text));
            }
        }

        void pc_StdErrReceived(object sender, DataReceivedEventArgs e)
        {
            ProcessCaller pc = (ProcessCaller)sender;

            Server s = FindServer(pc);
            if (s != null)
            {
                s.FinishedTime = DateTime.Now.Ticks;
                s.IsRunning = false;
                s.IsCrashed = true;

                if (MessageReceived != null)
                {
                    MessageReceived(this, new MessageEventArgs(MessageType.STDErr, s.type, s.args, e.Text));
                }
            }
            else
            {
                if (MessageReceived != null)
                    MessageReceived(this, new MessageEventArgs(MessageType.STDErr, ServerType.None, pc.FileName, e.Text));
            }
        }

        void pc_Failed(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            ProcessCaller pc = (ProcessCaller)sender;

            Server s = FindServer(pc);
            s.FinishedTime = DateTime.Now.Ticks;
            s.IsRunning = false;
            s.IsCrashed = true;

            if (s == null) 
            {
                if (MessageReceived != null)
                    MessageReceived(this, new MessageEventArgs(MessageType.Failed, ServerType.None, pc.FileName, e.Exception.Message));
                return;
            }

            if (MessageReceived != null)
            {
                MessageReceived(this, new MessageEventArgs(MessageType.Failed, s.type, s.args, e.Exception.Message));
            }
        }

        void pc_Completed(object sender, EventArgs e)
        {
            ProcessCaller pc = (ProcessCaller)sender;

            Server s = FindServer(pc);
            if (s == null)
            {
                if (MessageReceived != null)
                    MessageReceived(this, new MessageEventArgs(MessageType.Completed, ServerType.None, pc.FileName, "Server Completed"));
                return;
            }

            s.FinishedTime = DateTime.Now.Ticks;
            s.IsRunning = false;

            if(MessageReceived != null)
            {
                MessageReceived(this, new MessageEventArgs(MessageType.Completed, s.type, s.args, "Server Completed"));
            }
        }

        void pc_Cancelled(object sender, EventArgs e)
        {
            ProcessCaller pc = (ProcessCaller)sender;
            
            Server s = FindServer(pc);
            s.FinishedTime = DateTime.Now.Ticks;
            s.IsRunning = false;

            if (MessageReceived != null)
            {
                
                ServerType st = GetServerType(pc.FileName);
                String args = pc.Arguments;
                MessageReceived(this, new MessageEventArgs(MessageType.Cancelled, st, args, "No Message"));
            }
        }

        internal string GetWorkingDirectory()
        {
            
            return WorkingDirectory;
        }
    
        #region ISynchronizeInvoke Members

        IAsyncResult ISynchronizeInvoke.BeginInvoke(Delegate method, object[] args)
        {
            return m_Synchronizer.BeginInvoke(method, args);
        //    throw new NotImplementedException();
        }

        object ISynchronizeInvoke.EndInvoke(IAsyncResult result)
        {
        //    throw new NotImplementedException();
            return m_Synchronizer.EndInvoke(result);
        }

        object ISynchronizeInvoke.Invoke(Delegate method, object[] args)
        {
        //    throw new NotImplementedException();
            return m_Synchronizer.Invoke(method, args);
        }

        bool ISynchronizeInvoke.InvokeRequired
        {
        //    get { return false; }
            get { return m_Synchronizer.InvokeRequired; }
        }

        #endregion
    }
}