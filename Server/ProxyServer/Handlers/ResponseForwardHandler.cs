using System;
using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;
using Photon.SocketServer;

namespace ProxyServer.Handlers
{
    public class ResponseForwardHandler : DefaultResponseHandler
    {
        public ResponseForwardHandler(PhotonApplication application) : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Response; }
        }

        public override byte Code
        {
            get { return (byte) (ClientOperationCode.Chat | ClientOperationCode.Login | ClientOperationCode.Game); }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            Log.Debug("GOT TO RESPONSE FORWARD HANDLER");
            if (message.Parameters.ContainsKey((byte) ClientParameterCode.PeerId))
            {
                Log.DebugFormat("Looking for Peer Id {0}",
                    new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]));
                PhotonClientPeer peer;
                Server.ConnectionCollection<PhotonConnectionCollection>().Clients.TryGetValue(
                    new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]), out peer);
                if (peer != null)
                {
                    Log.DebugFormat("Found Peer");
                    message.Parameters.Remove((byte) ClientParameterCode.PeerId);
                    message.Parameters.Remove((byte) ClientParameterCode.UserId);
                    message.Parameters.Remove((byte) ClientParameterCode.CharacterId);

                    var response = message as PhotonResponse;
                    if (response != null)
                    {
                        Log.Debug("Sent this " + response.Code + " " + response.SubCode + " " + response.DebugMessage);
                        peer.SendOperationResponse(
                            new OperationResponse(response.Code, response.Parameters)
                            {
                                DebugMessage = response.DebugMessage,
                                ReturnCode = response.ReturnCode
                            }, new SendParameters());
                    }
                    else
                    {
                        Log.Debug("Sent this " + message.Code);
                        peer.SendOperationResponse(new OperationResponse(message.Code, message.Parameters),
                            new SendParameters());
                    }
                }
            }
            return true;
        }
    }
}