using System;
using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Server;
using SubServerCommon;

namespace LoginServer.Handlers
{
    public class ServerUserLoggedOutHandler : PhotonServerHandler
    {
        public ServerUserLoggedOutHandler(PhotonApplication application)
            : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Async; }
        }

        public override byte Code
        {
            get { return (byte) ServerEventCode.UserLoggedOut; }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            Log.Debug("OnHandle Log out message");
            var server = Server as LoginServer;
            if (server != null)
            {
                var peerId = new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]);
                server.ConnectionCollection<SubServerConnectionCollection>().Clients.Remove(peerId);
            }
            return true;
        }
    }
}