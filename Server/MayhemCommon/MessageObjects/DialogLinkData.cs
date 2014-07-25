using System;
using ProtoBuf;

namespace MayhemCommon.MessageObjects
{
    [ProtoContract]
    public class DialogLinkData
    {
        [ProtoMember(1)]
        public DialogLinkType Type { get; set; }

        [ProtoMember(2)]
        public String Text { get; set; }
    }
}