using System;
using System.Collections.Generic;
using GameServer.Model;
using GameServer.Model.Interfaces;
using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;
using SubServerCommon;

namespace GameServer.Handlers
{
    public class GameRegisterEventHandler : PhotonServerHandler
    {
        private readonly IWorld _world;
        private readonly SubServerClientPeer.Factory clientFactory;

        public GameRegisterEventHandler(IWorld world, PhotonApplication application,
            SubServerClientPeer.Factory clientFactory) : base(application)
        {
            _world = world;
            this.clientFactory = clientFactory;
        }

        public override MessageType Type
        {
            get { return MessageType.Async; }
        }

        public override byte Code
        {
            get { return (byte) ServerEventCode.CharacterRegister; }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var characterId = new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.CharacterId]);
            var peerId = new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]);

            try
            {
                Dictionary<Guid, SubServerClientPeer> clients =
                    Server.ConnectionCollection<SubServerConnectionCollection>().Clients;

                // If player has left a zone but not deregistered from the gameserver
                // TODO: Make sure when players leave a zone, they are deregistered from a game server 
                // (right now they are not. Only removed from the world)
                if (!clients.ContainsKey(peerId))
                {
                    clients.Add(peerId, clientFactory(peerId));
                }

                var instance = clients[peerId].ClientData<GPlayerInstance>();
                instance.UserId = new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.UserId]);
                instance.ServerPeer = serverPeer;
                instance.Client = clients[peerId];
                instance.Zone = _world.GetZone(new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.ZoneId]));

                if (instance.Restore(characterId))
                {
                    _world.AddPlayer(instance);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
            return true;
        }
    }
}