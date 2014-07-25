using System;
using MayhemCommon;

namespace GameServer.Model.ServerEvents
{
    public class ObjectNotification : ServerPacket
    {
        public ObjectNotification(int instanceId, ObjectNotificationType type, string notification)
            : base(MessageSubCode.ObjectNotification, null)
        {
            AddParameter(instanceId, ClientParameterCode.InstanceId);
            AddParameter((byte) type, ClientParameterCode.Object2);
            AddParameter(notification, ClientParameterCode.Object);
        }
    }
}