using GameServer.Model.Interfaces;
using MayhemCommon;
using MayhemCommon.MessageObjects;

namespace GameServer.Model.ServerEvents
{
    internal class LoadZone : ServerPacket
    {
        public LoadZone(GPlayerInstance instance, IZone zone)
            : base(MessageSubCode.LoadZone)
        {
            var gZone = zone as GZone;
            if (gZone != null)
            {
                AddParameter(gZone.Template.SceneName, ClientParameterCode.ZoneId);
                AddParameter(instance.InstanceId, ClientParameterCode.InstanceId);
                PositionData pos = instance.Position;
                AddSerializedParameter(pos, ClientParameterCode.Object);
            }
        }
    }
}