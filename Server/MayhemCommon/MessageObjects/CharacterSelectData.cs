using ProtoBuf;

namespace MayhemCommon.MessageObjects
{
    [ProtoContract]
    public class CharacterSelectData
    {
        [ProtoMember(2)] public CharacterListItem[] Characters;
        [ProtoMember(1)] public int Coins;
        [ProtoMember(3)] public ZoneListItem[] Zones;
    }
}