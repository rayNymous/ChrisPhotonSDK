using System;
using System.Collections.Generic;
using ProtoBuf;

namespace MayhemCommon.MessageObjects
{
    [ProtoContract]
    public class ItemData
    {
        [ProtoMember(3)] public String Description;
        [ProtoMember(6)] public int ItemLevel;
        [ProtoMember(1)] public String Name;
        [ProtoMember(4)] public ItemQuality Quality;
        [ProtoMember(7)] public EquipmentSlot Slot;
        [ProtoMember(2)] public String SpriteName;
        [ProtoMember(5)] public Dictionary<string, float> Stats;
    }
}