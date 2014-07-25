using MayhemCommon;

namespace GameServer.Model.ServerEvents
{
    public class FinishLoading : ServerPacket
    {
        public FinishLoading() : base(MessageSubCode.FinishLoading)
        {
        }
    }
}