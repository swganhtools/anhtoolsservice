using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using ANH_WCF_Interface;
using System.Timers;
using ProcessCallerLibrary;
using System.IO;
using System.Web;

namespace ANH_WCF_Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class AnhService : IAnhService
    {
        CentralServerMonitor monitor = null;
        
        public AnhService()
        {
            monitor = CentralServerMonitor.GetCentralMonitor();
            LoadConfig();
            monitor.GetListOfServersAvailable();
        }

        private void SendClientMessage(ServerType type, String args, MessageType messagetype, String message)
        {
            if (callback == null)
            {
                try
                {
                    callback = OperationContext.Current.GetCallbackChannel<IAnhCallback>();
                }
                catch { }
            }

            //if we have a callback send a message
            if(callback!=null)
            {
                try
                {
                    callback.ServerMessage(type, args, messagetype, message);
                }
                catch (Exception) { callback = null; }
            }
        }
        private void monitor_MessageReceived(object sender, MessageEventArgs e)
        {
            SendClientMessage(e.type, e.args, e.messagetype, e.Message);
        }

        #region Config and Tools
        private String AdminPassword = "";
        private Boolean AllowObservation = false;
        private Boolean Authenticated = false;
        
        private static bool PredicateComment(String s)
        {
            if (s.Length == 0) return false;
            return (s[0] == '#');
        }

        private bool GetServerExists(ServerType type, string args)
        {
            Server s = monitor.FindServer(type, args);
            return s != null && s.IsRunning;
        }

        private void LoadConfig()
        {
            try
            {               
                    //Get the file variables into the class members
                    try
                    {
                        AdminPassword = System.Configuration.ConfigurationManager.AppSettings["AdminPassword"];
                    }
                    catch { }
                    try
                    {
                        AllowObservation = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["AllowObservation"]);
                    }
                    catch { }
                    try
                    {
                        monitor.SetWorkingDirectory(System.Configuration.ConfigurationManager.AppSettings["WorkingDirectory"]);
                    }
                    catch { }
                }            
            catch { }
        }
        #endregion Config and Tools

        #region Service Implementation
        private IAnhCallback callback = null;

        public Boolean AuthenticateSession(String hash)
        {
            if (String.Compare(HashCalc.GetSHA(AdminPassword), hash) == 0)
            {
                Authenticated = true;
                return true;
            }
            else
            {
                return false; //not authed
            }
        }

        public String StartServer(ServerType type, String args)
        {
            if (!Authenticated)
                return "You must be Authenticated to perform this task";

            if (GetServerExists(type, args))
                return "Server: " + type + " args:" + args + " process is already running. Please kill it before trying to run a new instance";

            ProcessCaller pc = new ProcessCaller(monitor);
            pc.Arguments = args;
            pc.WorkingDirectory = monitor.GetWorkingDirectory() + "\\";

            switch (type)
            {
                case ServerType.None:
                    return "Server Type: None is not a valid type to start";
                case ServerType.ConnectionServer:
                    pc.FileName = monitor.GetWorkingDirectory() + "connectionserver.exe";
                    break;
                case ServerType.ChatServer:
                    pc.FileName = monitor.GetWorkingDirectory() + "chatserver.exe";
                    break;
                case ServerType.LoginServer:
                    pc.FileName = monitor.GetWorkingDirectory() + "loginserver.exe";
                    break;
                case ServerType.ZoneServer:
                    pc.FileName = monitor.GetWorkingDirectory() + "zoneserver.exe";
                    break;
                case ServerType.PingServer:
                    pc.FileName = monitor.GetWorkingDirectory() + "pingserver.exe";
                    break;
                default:
                    return "Hey man some weird ass shit just happened. An enum wasn't handled properly";
            }


            Server s = new Server(pc);
            s.args = args;
            s.type = type;

            return monitor.AddServer(s);

        }

        public String StopServer(ServerType type, String args)
        {
            if (!Authenticated)
                return "You must be Authenticated to perform this task";
            Server s = monitor.FindServer(type, args);
            if (s != null)
                return monitor.RemoveServer(s);
            else
                return "Unable to find a suitable server to stop. Aborting.";
        }

        public void GetServerStatuses()
        {
            if (!AllowObservation && !Authenticated)
            {
                SendClientMessage(ServerType.None, "", MessageType.Failed, "You must be Authenticated to perform this task");
                return;
            }
            if (callback != null)
            {
                try
                {
                    callback.ServerStatus(new List<IServerStatus>(monitor.ServerList));
                    return;
                }
                catch { }
            }
            SendClientMessage(ServerType.None, "", MessageType.Failed, "There was a problem with the Callback");
        }

        public String SubscribeToStatusUpdates()
        {
            if (!AllowObservation && !Authenticated)
                return "You must be Authenticated to perform this task";

            if(callback==null)
                callback = OperationContext.Current.GetCallbackChannel<IAnhCallback>();
            if (callback != null)
            {
                monitor.MessageReceived += new EventHandler<MessageEventArgs>(monitor_MessageReceived);
                return "Subscribe to Service Notices was successful";
            }
            else
            {
                return "Failed to subscribe";
            }
        }

        public String UnsubscribeFromStatusUpdates()
        {
            if (callback != null)
            {
                callback = null;
                monitor.MessageReceived -= new EventHandler<MessageEventArgs>(monitor_MessageReceived);
            }
            return "Unsubscribe from Service Notices was successful";
        }

        public void GetAvailableServers()
        {
            if (!Authenticated)
            {
                SendClientMessage(ServerType.None, "", MessageType.Failed, "You must be Authenticated to perform this task");
                return;
            }

            if (callback != null)
            {
                try
                {
                    callback.AvailableServers(monitor.AvailableServers);
                    return;
                }
                catch {  }
            }

            SendClientMessage(ServerType.None, "", MessageType.Failed, "There was a problem with the Callback");
        }
        #endregion Service Implementation
       
        
    }
}
