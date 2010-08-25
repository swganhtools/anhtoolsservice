using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace ANH_WCF_Interface
{
    [ServiceContract]
    public interface IAnhCallback
    {
        [OperationContract(IsOneWay = true)]
        void ServerMessage(ServerType type, String args, MessageType messtype, String Message);
    }
}
