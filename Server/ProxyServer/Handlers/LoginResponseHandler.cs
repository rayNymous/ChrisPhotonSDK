using System;
using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;
using Photon.SocketServer;
using SubServerCommon.Data.ClientData;

namespace ProxyServer.Handlers
{
    public class LoginResponseHandler : PhotonServerHandler
    {
        public LoginResponseHandler(PhotonApplication application) : base(application)
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
            get { return (int) MessageSubCode.Login; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            Log.Debug("LoginResponseHandler.OnHandleMessage");
            if (message.Parameters.ContainsKey((byte) ClientParameterCode.PeerId))
            {
                PhotonClientPeer peer;
                Server.ConnectionCollection<PhotonConnectionCollection>().Clients.TryGetValue(
                    new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]), out peer);

                if (peer != null)
                {
                    Log.Debug("Proxy LoginResponseHandler");
                    if (message.Parameters.ContainsKey((byte) ClientParameterCode.UserId))
                    {
                        // Character is selected and zone is set
                        Log.DebugFormat("Found User {0}",
                            new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.UserId]));
                        peer.ClientData<UserData>().UserId =
                            new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.UserId]);
                    }

                    message.Parameters.Remove((byte) ClientParameterCode.PeerId);
                    message.Parameters.Remove((byte) ClientParameterCode.UserId);
                    message.Parameters.Remove((byte) ClientParameterCode.CharacterId);

                    var response = message as PhotonResponse;
                    if (response != null)
                    {
                        peer.SendOperationResponse(
                            new OperationResponse(response.Code, response.Parameters)
                            {
                                DebugMessage = response.DebugMessage,
                                ReturnCode = response.ReturnCode
                            }, new SendParameters());
                    }
                    else
                    {
                        peer.SendOperationResponse(new OperationResponse(message.Code, message.Parameters),
                            new SendParameters());
                    }
                }
            }
            return true;
        }
    }
}