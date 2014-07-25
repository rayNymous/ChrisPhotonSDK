using System;
using ProtoBuf;

namespace MayhemCommon.MessageObjects
{
    [ProtoContract]
    public class ObjectHints
    {
        [ProtoMember(2)] public ObjectHint[] Hints;
        [ProtoMember(1)] public int[] Ids;
    }
}