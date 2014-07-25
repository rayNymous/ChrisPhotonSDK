using System;
using System.Collections.Generic;
using GameServer.Model;
using GameServer.Model.Interfaces;
using GameServer.Model.ServerEvents;
using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;

namespace GameServer.Handlers
{
    public class TargetRequestHandler : PhotonServerHandler
    {
        public TargetRequestHandler(PhotonApplication application) : base(application)
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
            get { return (int) MessageSubCode.TargetRequest; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var peerId = new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]);
            Dictionary<Guid, SubServerClientPeer> clients =
                Server.ConnectionCollection<SubServerConnectionCollection>().Clients;
            var instance = clients[peerId].ClientData<GPlayerInstance>();

            if (!message.Parameters.ContainsKey((byte) ClientParameterCode.InstanceId))
            {
                instance.SetTarget(null);
                return true;
            }
            var targetId = Convert.ToInt32(message.Parameters[(byte) ClientParameterCode.InstanceId]);

            IObject target = instance.ZoneBlock.GetNearbyObject(targetId);
            if (target != null)
            {
                instance.SetTarget(target);
                var character = target as GCharacter;
                if (character != null)
                {
                    var t = new Target(character,
                        GameActions.ActionsToStringArray(GameActions.GetActions(instance, character)));
                    instance.SendPacket(t);
                }
                else
                {
                    instance.SendPacket(new Target(target,
                        GameActions.ActionsToStringArray(GameActions.GetActions(instance, target))));
                }
            }
            return true;
        }
    }
}