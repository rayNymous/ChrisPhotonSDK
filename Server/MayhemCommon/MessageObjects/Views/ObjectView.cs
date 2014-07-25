using System;
using ProtoBuf;

namespace MayhemCommon.MessageObjects.Views
{
    [Serializable]
    [ProtoContract]
    [ProtoInclude(500, typeof (NpcView))]
    [ProtoInclude(501, typeof (PlayerView))]
    [ProtoInclude(502, typeof (ResourceView))]
    public class ObjectView
    {
        [ProtoMember(1)]
        public int InstanceId { get; set; }

        [ProtoMember(2)]
        public string Name { get; set; }

        [ProtoMember(3)]
        public ObjectType ObjectType { get; set; }

        [ProtoMember(4)]
        public string Prefab { get; set; }

        [ProtoMember(5)]
        public string DataType { get; set; }

        [ProtoMember(6)]
        public PositionData Position { get; set; }
    }
}