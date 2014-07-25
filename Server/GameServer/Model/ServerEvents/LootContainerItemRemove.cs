using MayhemCommon;

namespace GameServer.Model.ServerEvents
{
    public class LootContainerItemRemove : ServerPacket
    {
        public LootContainerItemRemove(int index) : base(MessageSubCode.LootContainerItemRemove, null)
        {
            AddParameter(index, ClientParameterCode.Object);
        }
    }
}