using ExitGames.Logging;
using MMO.Photon.Client;
using MMO.Photon.Server;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMO.Photon.Application
{
    public class PhotonPeerFactory
    {
        private readonly PhotonServerPeer.Factory _serverPeerFactory;
        private readonly PhotonClientPeer.Factory _clientPeerFactory;
        private readonly PhotonConnectionCollection _subServerCollection;
        private readonly PhotonApplication _application;

        protected readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public PhotonPeerFactory(PhotonServerPeer.Factory serverPeerFactory, PhotonClientPeer.Factory clientPeerFactory, PhotonConnectionCollection subServerCollection, PhotonApplication application)
        {
            this._serverPeerFactory = serverPeerFactory;
            this._clientPeerFactory = clientPeerFactory;
            this._subServerCollection = subServerCollection;
            this._application = application;
        }

        public PeerBase CreatePeer(InitRequest initRequest)
        {
            if (IsServerPeer(initRequest))
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat(("Received init request from sub server"));
                }
                return _serverPeerFactory(initRequest.Protocol, initRequest.PhotonPeer);
            }
            Log.DebugFormat(("Received init request from client"));
            return _clientPeerFactory(initRequest.Protocol, initRequest.PhotonPeer);
        }

        public PhotonServerPeer CreatePeer(InitResponse initResponse)
        {
            var subServerPeer = _serverPeerFactory(initResponse.Protocol, initResponse.PhotonPeer);
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat(("Received init request from sub server"));
            }
            if (initResponse.RemotePort == _application.MasterEndPoint.Port)
            {
                _application.Register(subServerPeer);
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat(("Registering sub server"));
                }
            }
            return subServerPeer;
        }

        protected bool IsServerPeer(InitRequest initRequest)
        {
            return _subServerCollection.IsServerPeer(initRequest);
        }
    }

}
