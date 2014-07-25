using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Model.Interfaces;
using MayhemCommon;
using MayhemCommon.MessageObjects;

namespace GameServer.Model.ServerEvents
{
    public class DeleteObjects : ServerPacket
    {
        public DeleteObjects(IEnumerable<IObject> objects)
            : base(MessageSubCode.DeleteObjects)
        {
            if (objects == null)
            {
                AddParameter(true, ClientParameterCode.Invalid);
                return;
            }

            var ids = objects.Select(obj => obj.InstanceId).ToArray();

            AddSerializedParameter(new DeleteObjectsData {Ids = ids}, ClientParameterCode.Object);
        }
    }
}