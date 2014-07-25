using ProtoBuf;

namespace MayhemCommon.MessageObjects
{
    [ProtoContract]
    public class DialogAction
    {
        [ProtoMember(1)]
        public int CurrentPage { get; set; }

        [ProtoMember(2)]
        public int Index { get; set; }
    }
}