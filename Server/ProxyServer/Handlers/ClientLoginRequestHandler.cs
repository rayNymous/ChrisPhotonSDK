using System;
using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using Photon.SocketServer;
using SubServerCommon;
using SubServerCommon.Data.ClientData;

namespace ProxyServer.Handlers
{
    internal class ClientLoginRequestHandler : PhotonClientHandler
    {
        public ClientLoginRequestHandler(PhotonApplication application) : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Async | MessageType.Request | MessageType.Response; }
        }

        public override byte Code
        {
            get { return (byte) ClientOperationCode.Login; }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonClientPeer peer)
        {
            // Cleanup false data in case some users decided to manually add information
            message.Parameters.Remove((byte) ClientParameterCode.PeerId);
            message.Parameters.Remove((byte) ClientParameterCode.UserId);

            message.Parameters.Add((byte) ClientParameterCode.PeerId, peer.PeerID.ToByteArray());

            var operationRequest = new OperationRequest(message.Code, message.Parameters);

            if (peer.ClientData<UserData>().UserId != Guid.Empty)
            {
                message.Parameters.Add((byte) ClientParameterCode.UserId, peer.ClientData<UserData>().UserId.ToByteArray());
            }

            // In case player is sending character select after leaving a zone, characterId is allready set in parameters
            if (peer.ClientData<UserData>().CharacterId != Guid.Empty &&
                !message.Parameters.ContainsKey((byte) ClientParameterCode.CharacterId))
            {
                message.Parameters.Add((byte) ClientParameterCode.CharacterId, peer.ClientData<UserData>().CharacterId.ToByteArray());
            }

            if (message.Code == (byte) ClientOperationCode.Login)
            {
                if (Server.ConnectionCollection<PhotonConnectionCollection>() != null)
                {
                    Log.DebugFormat("Passing login credentials to login server");
                    Server.ConnectionCollection<PhotonConnectionCollection>().GetServerByType((int) ServerType.Login)
                        .SendOperationRequest(operationRequest, new SendParameters());
                }
                else
                {
                    Log.DebugFormat("No server can handle a login request");
                }
            }
            else
            {
                Log.DebugFormat("Invalid Operation Code - Expecting Login");
            }

            return true;
        }
    }
}