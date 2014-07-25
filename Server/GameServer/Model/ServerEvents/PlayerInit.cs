using GameServer.Model.Stats;
using MayhemCommon;
using MayhemCommon.MessageObjects;

namespace GameServer.Model.ServerEvents
{
    internal class PlayerInit : ServerPacket
    {
        public PlayerInit(GPlayerInstance player) : base(MessageSubCode.PlayerInit)
        {
            var data = new PlayerInitData
            {
                CurrentHealth = player.Health,
                MaxHealth = (int) player.Stats.GetStat<MaxHealth>(),
                CharacterName = player.Name,
                InstanceId = player.InstanceId,
                Position = player.Position,
                Coins = player.Coins,
                CurrentHeat = player.Heat,
                MaxHeat = (int) player.Stats.GetStat<MaxHeat>(),
                Inventory = player.Inventory,
                Equipment = player.Equipment
            };
            AddSerializedParameter(data, ClientParameterCode.Object);
        }
    }
}