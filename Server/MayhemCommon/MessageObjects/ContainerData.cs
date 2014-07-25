using System;
using ProtoBuf;

namespace MayhemCommon.MessageObjects
{
    [ProtoContract]
    public class ContainerData
    {
        [ProtoMember(3)] public ItemData[] Items;
        [ProtoMember(1)] public String Name;
        [ProtoMember(2)] public ContainerType Type;
    }
}