using System;
using ProtoBuf;

namespace MayhemCommon.MessageObjects
{
    [Serializable]
    [ProtoContract]
    public class PositionData
    {
        public PositionData() : this(0, 0, 0, 0)
        {
        }

        public PositionData(float x, float y, float z) : this(x, y, z, 0)
        {
        }

        public PositionData(float x, float y, float z, short heading)
        {
            X = x;
            Y = y;
            Z = z;
            Heading = heading;
        }

        [ProtoMember(1)]
        public float X { get; set; }

        [ProtoMember(2)]
        public float Y { get; set; }

        [ProtoMember(3)]
        public float Z { get; set; }

        [ProtoMember(4)]
        public short Heading { get; set; }
    }
}