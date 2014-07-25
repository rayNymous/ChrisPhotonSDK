using System;
using ProtoBuf;

namespace MayhemCommon.MessageObjects
{
    [ProtoContract]
    public class TargetStatusData
    {
        [ProtoMember(1)]
        public int MaxHealth { get; set; }

        [ProtoMember(2)]
        public int CurrentHealth { get; set; }

        [ProtoMember(3)]
        public int InstanceId { get; set; }
    }
}