using System;
using System.Collections.Generic;
using ProtoBuf;

namespace MayhemCommon.MessageObjects
{
    [Serializable]
    [ProtoContract]
    public class DeleteObjectsData
    {
        [ProtoMember(1)] public int[] Ids;
    }
}