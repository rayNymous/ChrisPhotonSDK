using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MMO.Photon.Application;
using Photon.SocketServer.ServerToServer;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;

namespace MMO.Photon.Server
{
    public class PhotonServerPeer : ServerPeerBase
    {
        private readonly PhotonServerHandlerList _handlerList;
        protected readonly PhotonApplication Server;
        public Guid? ServerId { get; set; }
        public string TcpAddress { get; set; }
        public string UdpAddress { get; set; }
        public string ApplicationName { get; set; }
        public int ServerType { get; set; }

        #region Factory Method

        public delegate PhotonServerPeer Factory(IRpcProtocol protocol, IPhotonPeer photonPeer);

        #endregion

        public PhotonServerPeer(IRpcProtocol protocol, IPhotonPeer photonPeer, PhotonServerHandlerList handlerList, PhotonApplication application) 
            : base(protocol, photonPeer)
        {
            this._handlerList = handlerList;
            Server = application;
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            _handlerList.HandleMessage(
                new PhotonRequest(
                    operationRequest.OperationCode, 
                    operationRequest.Parameters.ContainsKey(Server.SubCodeParameterKey) ? (int?) Convert.ToInt32(operationRequest.Parameters[Server.SubCodeParameterKey]) :null , 
                    operationRequest.Parameters), this);
        }

        protected override void OnEvent(IEventData eventData, SendParameters sendParameters)
        {
 	        _handlerList.HandleMessage(
                new PhotonEvent(
                    eventData.Code, 
                    eventData.Parameters.ContainsKey(Server.SubCodeParameterKey) ? (int?) Convert.ToInt32(eventData.Parameters[Server.SubCodeParameterKey]) :null , 
                    eventData.Parameters), this);
        }

        protected override void OnOperationResponse(OperationResponse operationResponse, SendParameters sendParameters)
        {
 	         _handlerList.HandleMessage(
                 new PhotonResponse(
                     operationResponse.OperationCode,
                     operationResponse.Parameters.ContainsKey(Server.SubCodeParameterKey) ? (int?)Convert.ToInt32(operationResponse.Parameters[Server.SubCodeParameterKey]) : null, 
                     operationResponse.Parameters, 
                     operationResponse.DebugMessage, 
                     operationResponse.ReturnCode), this);
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            Server.ConnectionCollection<PhotonConnectionCollection>().OnDisconnect(this);
        }
    }
}
