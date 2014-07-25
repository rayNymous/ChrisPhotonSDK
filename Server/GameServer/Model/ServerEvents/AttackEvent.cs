using MayhemCommon;
using MayhemCommon.MessageObjects;

namespace GameServer.Model.ServerEvents
{
    public class AttackEvent : ServerPacket
    {
        public AttackEvent(AttackData data) : base(MessageSubCode.Attack)
        {
            AddSerializedParameter(data, ClientParameterCode.Object);
        }
    }
}