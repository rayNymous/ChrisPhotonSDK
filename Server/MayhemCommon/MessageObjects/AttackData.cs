using System;
using ProtoBuf;

namespace MayhemCommon.MessageObjects
{
    [ProtoContract]
    public class AttackData
    {
        [ProtoMember(1)]
        public int AttackerId { get; set; }

        [ProtoMember(2)]
        public int TargetId { get; set; }

        [ProtoMember(3)]
        public int Damage { get; set; }

        [ProtoMember(4)]
        public bool IsCritical { get; set; }
    }
}