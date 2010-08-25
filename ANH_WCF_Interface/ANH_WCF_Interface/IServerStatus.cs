using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ANH_WCF_Interface
{
    public interface IServerStatus
    {
        ServerType type { get; set; }
        
        String args { get; set; }
        
        Boolean IsRunning { get; set; }
        
        Boolean IsCrashed { get; set; }
        
        Int64 Uptime { get; }
        
        Int64 StartedTime { get; set; }
    }
}
