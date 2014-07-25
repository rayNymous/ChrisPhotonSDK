using System;
using System.Collections.Generic;
using ProtoBuf;

namespace MayhemCommon.MessageObjects.Views
{
    [Serializable]
    [ProtoContract]
    public class ViewsData
    {
        [ProtoMember(1)] public List<ObjectView> Views;
    }
}