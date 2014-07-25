using System;
using ProtoBuf;

namespace MayhemCommon.MessageObjects
{
    [Serializable]
    [ProtoContract]
    public class PlayerInitData
    {
        [ProtoMember(10)] public ContainerData Equipment;
        [ProtoMember(9)] public ContainerData Inventory;

        [ProtoMember(1)]
        public string CharacterName { get; set; }

        [ProtoMember(2)]
        public int InstanceId { get; set; }

        [ProtoMember(3)]
        public PositionData Position { get; set; }

        [ProtoMember(4)]
        public int CurrentHealth { get; set; }

        [ProtoMember(5)]
        public int MaxHealth { get; set; }

        [ProtoMember(6)]
        public int MaxHeat { get; set; }

        [ProtoMember(7)]
        public int CurrentHeat { get; set; }

        [ProtoMember(8)]
        public int Coins { get; set; }
    }
}