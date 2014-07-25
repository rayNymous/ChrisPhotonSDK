using System;
using System.Collections.Generic;
using ProtoBuf;

namespace MayhemCommon.MessageObjects
{
    [ProtoContract]
    public class DialogPageData
    {
        [ProtoMember(1)]
        public String NpcName { get; set; }

        [ProtoMember(2)]
        public String Text { get; set; }

        [ProtoMember(3)]
        public int PageId { get; set; }

        [ProtoMember(4)]
        public bool LeftButtonEnabled { get; set; }

        [ProtoMember(5)]
        public string LeftButtonText { get; set; }

        [ProtoMember(6)]
        public IEnumerable<DialogLinkData> Links { get; set; }
    }
}