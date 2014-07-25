using MayhemCommon;

namespace GameServer.Model.ServerEvents
{
    public class LeaveZone : ServerPacket
    {
        public LeaveZone() : base(MessageSubCode.LeaveZone, null)
        {
        }
    }
}