using GameServer.Model.Stats;
using MayhemCommon;
using MayhemCommon.MessageObjects;

namespace GameServer.Model.ServerEvents
{
    public class TargetStatus : ServerPacket
    {
        public TargetStatus(GCharacter character) : base(MessageSubCode.TargetStatus, null)
        {
            AddSerializedParameter(new TargetStatusData
            {
                InstanceId = character.InstanceId,
                CurrentHealth = character.Health,
                MaxHealth = (int) character.Stats.GetStat<MaxHealth>()
            }, ClientParameterCode.Object);
        }
    }
}