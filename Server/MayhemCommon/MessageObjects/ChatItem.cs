using System;
using ProtoBuf;

namespace MayhemCommon.MessageObjects
{
    [ProtoContract]
    public class ChatItem
    {
        [ProtoMember(1)]
        public int InstanceId { get; set; }
        [ProtoMember(2)]
        public ChatType Type { get; set; }
        [ProtoMember(3)]
        public string Text { get; set; }
        [ProtoMember(4)]
        public bool IsNpc { get; set; }
        [ProtoMember(5)]
        public string Name { get; set; }
    }
}