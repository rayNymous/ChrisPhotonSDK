using System;
using ProtoBuf;

namespace MayhemCommon.MessageObjects.Views
{
    [ProtoContract]
    [Serializable]
    public class NpcView : ObjectView
    {
        [ProtoMember(2)] public ObjectHint Hint;
        [ProtoMember(1)] public RelationshipType Type;
    }
}