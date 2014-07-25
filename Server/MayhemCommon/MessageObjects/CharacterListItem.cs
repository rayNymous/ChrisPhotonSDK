using System;
using ProtoBuf;

namespace MayhemCommon.MessageObjects
{
    [Serializable]
    [ProtoContract]
    public class CharacterListItem
    {
        [ProtoMember(5)] public bool Deployed;
        [ProtoMember(4)] public int Fame;
        [ProtoMember(1)] public Guid Id;
        [ProtoMember(2)] public String Name;
        [ProtoMember(3)] public String Sex;
    }
}