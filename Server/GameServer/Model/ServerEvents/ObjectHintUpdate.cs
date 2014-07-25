using System;
using System.Collections.Generic;
using GameServer.Model.Interfaces;
using MayhemCommon;
using MayhemCommon.MessageObjects;

namespace GameServer.Model.ServerEvents
{
    public class ObjectHintUpdate : ServerPacket
    {
        private readonly List<IObject> _objects;

        public ObjectHintUpdate(List<IObject> objects)
            : base(MessageSubCode.ObjectHint, null)
        {
            _objects = objects;
        }

        public override void LateInitialization(GPlayerInstance player)
        {
            var ids = new int[_objects.Count];
            var hints = new ObjectHint[_objects.Count];

            int index = 0;
            foreach (IObject character in _objects)
            {
                ids[index] = character.InstanceId;
                hints[index] = character.GetHint(player);
                index++;
            }

            AddSerializedParameter(new ObjectHints
            {
                Hints = hints,
                Ids = ids
            }, ClientParameterCode.Object);
        }
    }
}