using System;
using System.Collections.Generic;
using System.IO;
using GameServer.Model;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;
using ProtoBuf;

namespace GameServer.Handlers
{
    public class UpdatePositionHandler : GameRequestHandler
    {
        public UpdatePositionHandler(PhotonApplication application) : base(application)
        {
        }

        public override int? SubCode
        {
            get { return (int) MessageSubCode.PositionUpdate; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var peerId = new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]);
            Dictionary<Guid, SubServerClientPeer> clients =
                Server.ConnectionCollection<SubServerConnectionCollection>().Clients;
            var instance = clients[peerId].ClientData<GPlayerInstance>();

            var rawPosition = (byte[]) message.Parameters[(byte) ClientParameterCode.Object];
            PositionData positionData = null;

            using (var ms = new MemoryStream(rawPosition))
            {
                positionData = Serializer.Deserialize<PositionData>(ms);
            }

            instance.UpdatePosition(positionData);

            return true;
        }
    }
}