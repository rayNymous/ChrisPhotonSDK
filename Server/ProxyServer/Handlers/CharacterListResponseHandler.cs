using System;
using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;
using Photon.SocketServer;
using SubServerCommon;

namespace ProxyServer.Handlers
{
    public class CharacterListResponseHandler : PhotonServerHandler
    {
        public CharacterListResponseHandler(PhotonApplication application) : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Response; }
        }

        public override byte Code
        {
            get { return (byte) ClientOperationCode.Login; }
        }

        public override int? SubCode
        {
            get { return (int) MessageSubCode.ListCharacters; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            if (message.Parameters.ContainsKey((byte) ClientParameterCode.PeerId))
            {
                PhotonClientPeer peer;
                Server.ConnectionCollection<ProxyConnectionCollection>()
                    .Clients.TryGetValue(new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]),
                        out peer);

                if (peer != null)
                {
                    if (!message.Parameters.ContainsKey((byte) ClientParameterCode.ZoneInfo))
                    {
                        Log.Debug("Zone info not attached yet");
                        // If we have no zone info attached
                        PhotonServerPeer gameServer =
                            Server.ConnectionCollection<ProxyConnectionCollection>()
                                .OnGetServerByType((int) ServerType.Game);
                        if (gameServer != null)
                        {
                            peer.CurrentServer = gameServer;
                            gameServer.SendOperationRequest(new OperationRequest
                            {
                                OperationCode = (byte) ClientOperationCode.Login,
                                Parameters = message.Parameters
                            }, new SendParameters());
                        }
                    }
                    else
                    {
                        // If Zone info is allready attached
                        Log.Debug("Zone info already attached, safe to send back to player");
                        peer.SendOperationResponse(new OperationResponse
                        {
                            OperationCode = (byte) ClientOperationCode.Game,
                            Parameters = message.Parameters
                        }, new SendParameters());
                    }
                }
            }
            return true;
        }
    }
}