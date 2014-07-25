using System.Collections.Generic;
using MayhemCommon;

namespace GameServer.Model.ServerEvents
{
    public class PlayerStatusUpdate : ServerPacket
    {
        public PlayerStatusUpdate(Dictionary<byte, object> data) : base(MessageSubCode.PlayerStatus, null)
        {
            AddParameter(data, ClientParameterCode.Object);
        }
    }
}