using System;
using ProtoBuf;

namespace MayhemCommon.MessageObjects.Views
{
    [Serializable]
    [ProtoContract]
    public class ResourceView : ObjectView
    {
        [ProtoMember(1)]
        public int RequiredLevel { get; set; }

        [ProtoMember(2)]
        public ResourceType ResourceType { get; set; }
    }
}