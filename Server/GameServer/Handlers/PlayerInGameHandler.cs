using System;
using System.Collections.Generic;
using GameServer.Model;
using GameServer.Model.ServerEvents;
using GameServer.Model.Stats;
using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;

namespace GameServer.Handlers
{
    public class PlayerInGameHandler : PhotonServerHandler
    {
        public PlayerInGameHandler(PhotonApplication application) : base(application)
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
            get { return (int) MessageSubCode.PlayerInGame; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var peerId = new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]);
            Dictionary<Guid, SubServerClientPeer> clients =
                Server.ConnectionCollection<SubServerConnectionCollection>().Clients;
            var instance = clients[peerId].ClientData<GPlayerInstance>();

            instance.Spawn();
            Log.Debug("PlayerInGameHandler: remove test health values");
            instance.Health = (int) instance.Stats.GetStat<MaxHealth>();
            instance.Heat = (int) instance.Stats.GetStat<MaxHeat>();

            // Player initialization package 
            instance.SendPacket(new PlayerInit(instance));

            // Display visible objects around
            instance.SendPacket(new ShowObjects(instance.VisibleObjects));

            Log.DebugFormat("Character in game server! Heat: " + instance.Heat + "/" + instance.Stats.GetStat<MaxHeat>());

            // Notify Guild members that someone logged in
            // Notify friend list that someone logged in

            return true;
        }
    }
}