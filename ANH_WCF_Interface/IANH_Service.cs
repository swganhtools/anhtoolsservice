using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ANHServiceLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract(Namespace="ANH",SessionMode = SessionMode.Required, CallbackContract = typeof(IANHClientService))]
    //[ServiceContract(Namespace = "ANH", CallbackContract = typeof(IANHClientService))]
    public interface IANHService
    {
        //[OperationContract]
        //string GetData(int value);

        //[OperationContract]
        //CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: Add your service operations here

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void SubscribeToEvents(String ServerName);
        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void UnsubscribeEvents(String ServerName);
        [OperationContract]
        List<String> GetAvailableServerTypes();
        [OperationContract]
        Boolean StartServer(String Type, String Arguments);
        [OperationContract]
        Boolean StopServer(String Type, String Arguments);
        [OperationContract]
        List<String> GetServerStatuses();

    }

    
    public interface IANHClientService
    {
        [OperationContract(IsOneWay = true)]
        void SendServiceMessage(String Message);
    }
    // Use a data contract as illustrated in the sample below to add composite types to service operations
    //[DataContract]
    //public class CompositeType
    //{
    //    bool boolValue = true;
    //    string stringValue = "Hello ";

    //    [DataMember]
    //    public bool BoolValue
    //    {
    //        get { return boolValue; }
    //        set { boolValue = value; }
    //    }

    //    [DataMember]
    //    public string StringValue
    //    {
    //        get { return stringValue; }
    //        set { stringValue = value; }
    //    }
    //}
}
