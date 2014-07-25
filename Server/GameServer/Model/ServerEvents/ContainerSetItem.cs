using MayhemCommon;
using MayhemCommon.MessageObjects;

namespace GameServer.Model.ServerEvents
{
    public class ContainerSetItem : ServerPacket
    {
        public ContainerSetItem(int index, ItemData item, ContainerType type)
            : base(MessageSubCode.ContainerSetItem, null)
        {
            AddParameter(index, ClientParameterCode.Object);
            if (item != null)
            {
                AddSerializedParameter(item, ClientParameterCode.Object2);
            }

            AddParameter((byte) type, ClientParameterCode.Object3);
        }
    }
}