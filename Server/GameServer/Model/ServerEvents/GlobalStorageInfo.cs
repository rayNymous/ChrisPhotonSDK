using MayhemCommon;

namespace GameServer.Model.ServerEvents
{
    public class GlobalStorageInfo : ServerPacket
    {
        public GlobalStorageInfo(GPlayerInstance instance)
            : base(MessageSubCode.GlobalStorageData, null)
        {
            AddSerializedParameter(instance.Storage.Data, ClientParameterCode.Object);
        }
    }
}