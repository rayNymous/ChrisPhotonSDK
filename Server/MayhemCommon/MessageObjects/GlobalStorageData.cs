using System.Collections.Generic;
using ProtoBuf;

namespace MayhemCommon.MessageObjects
{
    [ProtoContract]
    public class GlobalStorageData
    {
        [ProtoMember(1)] public int Coins;
        [ProtoMember(2)] public Dictionary<int, ItemData> Items;
    }
}