using System;
using ProtoBuf;

namespace MayhemCommon.MessageObjects
{
    [ProtoContract]
    public class ZoneListItem
    {
        [ProtoMember(7)] public String CharacterName;
        [ProtoMember(9)] public Guid InstanceId;
        [ProtoMember(3)] public int MaxPlayers;
        [ProtoMember(1)] public String Name;
        [ProtoMember(2)] public int PlayersOnline;
        [ProtoMember(6)] public int Price;
        [ProtoMember(4)] public int Stars;
        [ProtoMember(8)] public String Thumbnail;
        [ProtoMember(5)] public bool Unlocked;
    }
}