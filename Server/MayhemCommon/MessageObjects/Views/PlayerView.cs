using System;
using ProtoBuf;

namespace MayhemCommon.MessageObjects.Views
{
    [Serializable]
    [ProtoContract]
    public class PlayerView : ObjectView
    {
        [ProtoMember(1)]
        public int HairStyle { get; set; }

        [ProtoMember(2)]
        public int Eyes { get; set; }

        [ProtoMember(3)]
        public int Sex { get; set; }

        [ProtoMember(4)]
        public int Armor { get; set; }

        [ProtoMember(5)]
        public int Helmet { get; set; }

        [ProtoMember(6)]
        public int LeftHand { get; set; }

        [ProtoMember(7)]
        public int Righthand { get; set; }
    }
}