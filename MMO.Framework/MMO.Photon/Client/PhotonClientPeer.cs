using System.Diagnostics;
using ExitGames.Logging;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Server;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMO.Photon.Client
{
    public class PhotonClientPeer : PeerBase
    {
        protected readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly Guid _peerId;
        private readonly Dictionary<Type, IClientData> _clientData = new Dictionary<Type, IClientData>();
        private readonly PhotonApplication _server;
        private readonly PhotonClientHandlerList _handlerList;
        public PhotonServerPeer CurrentServer { get; set; }

        #region Factory Method

        public delegate PhotonClientPeer Factory(IRpcProtocol protocol, IPhotonPeer photonPeer);

        #endregion

        public PhotonClientPeer(IRpcProtocol protocol, IPhotonPeer photonPeer, IEnumerable<IClientData> clientData, PhotonClientHandlerList handlerList, PhotonApplication application)
            : base(protocol, photonPeer)
        {
            _peerId = Guid.NewGuid();
            this._handlerList = handlerList;
            this._server = application;
            foreach (var data in clientData)
            {
                this._clientData.Add(data.GetType(), data);
            }
            this._server.ConnectionCollection<PhotonConnectionCollection>().OnClientConnect(this);
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            _handlerList.HandleMessage(
                new PhotonRequest(
                    operationRequest.OperationCode,
                    operationRequest.Parameters.ContainsKey(_server.SubCodeParameterKey) ? (int?)Convert.ToInt32(operationRequest.Parameters[_server.SubCodeParameterKey]) : null,
                    operationRequest.Parameters), this);
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            _server.ConnectionCollection<PhotonConnectionCollection>().OnClientDisconnect(this);
            Log.DebugFormat("Client {0} disconnected", _peerId);
        }

        public Guid PeerID
        {
            get { return _peerId; }
        }

        public T ClientData<T>() where T : class, IClientData
        {
            IClientData result;
            _clientData.TryGetValue(typeof(T), out result);
            if(result != null) {
                return result as T;
            }
            return null;
        }
    }
}
