using System;
using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Server;
using SubServerCommon;

namespace ChatServer.Handlers
{
    public class DeregisterEventHandler : PhotonServerHandler
    {
        public DeregisterEventHandler(PhotonApplication application) : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Async; }
        }

        public override byte Code
        {
            get { return (byte) ServerEventCode.CharacterDeregister; }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var peerId = new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]);

            // TODO Remove from Groups, guilds etc.

            Server.ConnectionCollection<SubServerConnectionCollection>().Clients.Remove(peerId);
            Log.DebugFormat("Removed peer {0}, now we have {1} clients", peerId,
                Server.ConnectionCollection<SubServerConnectionCollection>().Clients.Count);

            return true;
        }
    }
}