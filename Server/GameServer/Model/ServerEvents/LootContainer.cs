using MayhemCommon;
using MayhemCommon.MessageObjects;

namespace GameServer.Model.ServerEvents
{
    public class LootContainer : ServerPacket
    {
        public LootContainer(ContainerData container) : base(MessageSubCode.LootContainer, null)
        {
            AddSerializedParameter(container, ClientParameterCode.Object);
        }
    }
}