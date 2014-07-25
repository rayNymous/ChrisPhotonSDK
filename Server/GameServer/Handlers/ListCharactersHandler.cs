using GameServer.Model.Interfaces;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Server;
using Photon.SocketServer;

namespace GameServer.Handlers
{
    public class ListCharactersHandler : PhotonServerHandler
    {
        private readonly IWorld _world;
        private PhotonApplication application;

        public ListCharactersHandler(IWorld world, PhotonApplication application) : base(application)
        {
            _world = world;
        }

        public override MessageType Type
        {
            get { return MessageType.Request; }
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
            Log.Debug("Received");

            var charSelectData =
                Helpers.Deserialize<CharacterSelectData>(
                    (byte[]) message.Parameters[(byte) ClientParameterCode.CharacterList]);


            // Zones might be null when first playing
            if (charSelectData.Zones != null)
            {
                foreach (ZoneListItem zoneListItem in charSelectData.Zones)
                {
                    zoneListItem.PlayersOnline = _world.GetZone(zoneListItem.InstanceId).OnlinePlayersCount;
                }
                message.Parameters[(byte) ClientParameterCode.CharacterList] = Helpers.Serialize(charSelectData);
            }

            message.Parameters.Add((byte) ClientParameterCode.ZoneInfo, "");

            serverPeer.SendOperationResponse(new OperationResponse
            {
                OperationCode = message.Code,
                Parameters = message.Parameters
            }, new SendParameters());


            return true;
        }
    }
}