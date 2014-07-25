using System;
using ProtoBuf;

namespace MayhemCommon.MessageObjects
{
    [Serializable]
    [ProtoContract]
    public class MoveToData
    {
        [ProtoMember(1)]
        public PositionData Destination { get; set; }

        [ProtoMember(2)]
        public float Speed { get; set; }
    }
}