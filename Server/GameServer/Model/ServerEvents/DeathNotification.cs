using MayhemCommon;

namespace GameServer.Model.ServerEvents
{
    public class DeathNotification : ServerPacket
    {
        public DeathNotification() : base(MessageSubCode.DeathNotification, null)
        {
        }
    }
}