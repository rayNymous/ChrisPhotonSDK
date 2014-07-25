using MayhemCommon;
using MayhemCommon.MessageObjects;

namespace GameServer.Model.ServerEvents
{
    public class MoveTo : ServerPacket
    {
        public MoveTo(GCharacter character, MoveToData data, GPlayerInstance player = null)
            : base(MessageSubCode.MoveTo, player)
        {
            AddParameter(character.InstanceId, ClientParameterCode.InstanceId);
            AddSerializedParameter(data, ClientParameterCode.Object);
        }
    }
}