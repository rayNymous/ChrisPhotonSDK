using System;
using MayhemCommon;

namespace GameServer.Model.ServerEvents
{
    public class EventNotification : ServerPacket
    {
        public EventNotification(String message) : base(MessageSubCode.EventNotification)
        {
            AddParameter(message, ClientParameterCode.Object);
        }
    }
}