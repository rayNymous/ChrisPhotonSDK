using GameServer.Model.Interfaces;
using MayhemCommon;

namespace GameServer.Model.ServerEvents
{
    public class CharacterDeathEvent : ServerPacket
    {
        public CharacterDeathEvent(ICharacter character)
            : base(MessageSubCode.CharacterDeath)
        {
            AddParameter(character.InstanceId, ClientParameterCode.InstanceId);
        }
    }
}