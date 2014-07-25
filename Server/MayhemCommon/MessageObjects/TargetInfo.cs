using System;
using ProtoBuf;

namespace MayhemCommon.MessageObjects
{
    [Serializable]
    [ProtoContract]
    public class TargetInfo
    {
        [ProtoMember(1)]
        public int InstanceId { get; set; }

        [ProtoMember(2)]
        public String Name { get; set; }

        [ProtoMember(3)]
        public int MaxHealth { get; set; }

        [ProtoMember(4)]
        public int CurrentHealth { get; set; }

        [ProtoMember(5)]
        public int Level { get; set; }

        [ProtoMember(6)]
        public string[] Actions { get; set; }
    }
}