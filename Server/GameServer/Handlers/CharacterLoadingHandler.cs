using System;
using System.Collections.Generic;
using GameServer.Model;
using GameServer.Model.ServerEvents;
using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;

namespace GameServer.Handlers
{
    public class CharacterLoadingHandler : PhotonServerHandler
    {
        public CharacterLoadingHandler(PhotonApplication application) : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Request; }
        }

        public override byte Code
        {
            get { return (byte) ClientOperationCode.Game; }
        }

        public override int? SubCode
        {
            get { return (int?) MessageSubCode.CharacterLoading; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var peerId = new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]);

            Dictionary<Guid, SubServerClientPeer> clients =
                Server.ConnectionCollection<SubServerConnectionCollection>().Clients;
            var instance = clients[peerId].ClientData<GPlayerInstance>();

            instance.SendPacket(new LoadZone(instance, instance.Zone));
            return true;
        }
    }
}