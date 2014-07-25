using System;
using System.Collections.Generic;
using GameServer.Model;
using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;
using SubServerCommon;

namespace GameServer.Handlers
{
    public class GameDeregisterEventHandler : PhotonServerHandler
    {
        public GameDeregisterEventHandler(PhotonApplication application) : base(application)
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
            Dictionary<Guid, SubServerClientPeer> clients =
                Server.ConnectionCollection<SubServerConnectionCollection>().Clients;
            var instance = clients[peerId].ClientData<GPlayerInstance>();
            instance.Deregister();

            Server.ConnectionCollection<SubServerConnectionCollection>().Clients.Remove(peerId);
            Log.DebugFormat("Removed peer {0}, now we have {1} clients", peerId,
                Server.ConnectionCollection<SubServerConnectionCollection>().Clients.Count);

            return true;
        }
    }
}