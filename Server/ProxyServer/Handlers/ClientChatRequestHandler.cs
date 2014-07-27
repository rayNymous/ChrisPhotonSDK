using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using Photon.SocketServer;
using SubServerCommon;
using SubServerCommon.Data.ClientData;

namespace ProxyServer.Handlers
{
    internal class ClientChatRequestHandler : PhotonClientHandler
    {
        public ClientChatRequestHandler(PhotonApplication application)
            : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Async | MessageType.Request | MessageType.Response; }
        }

        public override byte Code
        {
            get { return (byte) ClientOperationCode.Chat; }
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

            message.Parameters.Add((byte) ClientParameterCode.UserId, peer.ClientData<UserData>().UserId.ToByteArray());

            message.Parameters.Remove((byte) ClientParameterCode.CharacterId);
            message.Parameters.Add((byte) ClientParameterCode.CharacterId, peer.ClientData<UserData>().CharacterId.ToByteArray());

            if (message.Code == (byte) ClientOperationCode.Chat)
            {
                if (Server.ConnectionCollection<PhotonConnectionCollection>() != null)
                {
                    // TODO Send to chat server instead of game server (if necessary?)
                    Server.ConnectionCollection<PhotonConnectionCollection>().GetServerByType((int) ServerType.Game)
                        .SendOperationRequest(operationRequest, new SendParameters());
                }
                else
                {
                    Log.DebugFormat("No server can handle a chat request");
                }
            }
            else
            {
                Log.DebugFormat("Invalid Operation Code - Expecting Chat");
            }

            return true;
        }
    }
}