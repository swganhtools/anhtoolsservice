using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ANH_WCF_Interface;
using ProcessCallerLibrary;

namespace ANH_WCF_Service
{
    public class Server : IServerStatus
    {
        public Server(ProcessCaller pc)
        {
            // TODO: Complete member initialization
            Process = pc;
        }
        public ServerType type { get; set; }

        public String args { get; set; }

        public Boolean IsRunning { get; set; }

        public Boolean IsCrashed { get; set; }

        public Int64 Uptime { 
            get{
                if(IsRunning)
                {
                    return DateTime.Now.Ticks - StartedTime;
                } else {
                    return DateTime.Now.Ticks - FinishedTime;
                }
            }  
        }

        public Int64 StartedTime { get; set; }
        public Int64 FinishedTime { get; set; }

        internal ProcessCaller Process { get; set; }
    }
}