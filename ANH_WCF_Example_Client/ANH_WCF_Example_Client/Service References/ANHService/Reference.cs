﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ANH_WCF_Example_Client.ANHService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ANHService.IAnhService", CallbackContract=typeof(ANH_WCF_Example_Client.ANHService.IAnhServiceCallback), SessionMode=System.ServiceModel.SessionMode.Required)]
    public interface IAnhService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAnhService/AuthenticateSession", ReplyAction="http://tempuri.org/IAnhService/AuthenticateSessionResponse")]
        bool AuthenticateSession(string hash);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAnhService/StartServer", ReplyAction="http://tempuri.org/IAnhService/StartServerResponse")]
        string StartServer(ANH_WCF_Interface.ServerType type, string args);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAnhService/StopServer", ReplyAction="http://tempuri.org/IAnhService/StopServerResponse")]
        string StopServer(ANH_WCF_Interface.ServerType type, string args);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IAnhService/GetServerStatuses")]
        void GetServerStatuses();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IAnhService/GetAvailableServers")]
        void GetAvailableServers();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAnhService/SubscribeToStatusUpdates", ReplyAction="http://tempuri.org/IAnhService/SubscribeToStatusUpdatesResponse")]
        string SubscribeToStatusUpdates();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAnhService/UnsubscribeFromStatusUpdates", ReplyAction="http://tempuri.org/IAnhService/UnsubscribeFromStatusUpdatesResponse")]
        string UnsubscribeFromStatusUpdates();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IAnhServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IAnhService/ServerMessage")]
        void ServerMessage(ANH_WCF_Interface.ServerType ServType, string Args, ANH_WCF_Interface.MessageType MessType, string Message);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IAnhService/ServerStatus")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(object[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(ANH_WCF_Interface.ServerType))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(ANH_WCF_Interface.MessageType))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(ANH_WCF_Interface.ServerType[]))]
        void ServerStatus(object[] status);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IAnhService/AvailableServers")]
        void AvailableServers(ANH_WCF_Interface.ServerType[] servers);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IAnhServiceChannel : ANH_WCF_Example_Client.ANHService.IAnhService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AnhServiceClient : System.ServiceModel.DuplexClientBase<ANH_WCF_Example_Client.ANHService.IAnhService>, ANH_WCF_Example_Client.ANHService.IAnhService {
        
        public AnhServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public AnhServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public AnhServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public AnhServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public AnhServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public bool AuthenticateSession(string hash) {
            return base.Channel.AuthenticateSession(hash);
        }
        
        public string StartServer(ANH_WCF_Interface.ServerType type, string args) {
            return base.Channel.StartServer(type, args);
        }
        
        public string StopServer(ANH_WCF_Interface.ServerType type, string args) {
            return base.Channel.StopServer(type, args);
        }
        
        public void GetServerStatuses() {
            base.Channel.GetServerStatuses();
        }
        
        public void GetAvailableServers() {
            base.Channel.GetAvailableServers();
        }
        
        public string SubscribeToStatusUpdates() {
            return base.Channel.SubscribeToStatusUpdates();
        }
        
        public string UnsubscribeFromStatusUpdates() {
            return base.Channel.UnsubscribeFromStatusUpdates();
        }
    }
}
