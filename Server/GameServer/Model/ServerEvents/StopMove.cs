using MayhemCommon;
using MayhemCommon.MessageObjects;

namespace GameServer.Model.ServerEvents
{
    public class StopMove : ServerPacket
    {
        public StopMove(GCharacter character) : base(MessageSubCode.StopMove)
        {
            PositionData data = character.Position;
        }
    }
}