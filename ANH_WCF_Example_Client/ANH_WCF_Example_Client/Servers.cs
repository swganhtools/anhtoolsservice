using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ANH_WCF_Interface;

namespace ANH_WCF_Example_Client
{
    class Servers
    {
        public Servers() { ;}
        public Servers(ServerType t, String a) { type = t; args = a; }
        public ServerType type { get; set; }
        public String args { get; set; }
        public override string ToString()
        {
            return type.ToString() + " args= " + args;
        }
    }
}
