using System.Collections.Generic;
using System.Linq;
using GameServer.Model.Interfaces;
using MayhemCommon;
using MayhemCommon.MessageObjects.Views;

namespace GameServer.Model.ServerEvents
{
    public class ShowObjects : ServerPacket
    {
        private readonly List<IObject> _objects;

        public ShowObjects(List<IObject> objects)
            : base(MessageSubCode.ShowObjects)
        {
            _objects = objects;
        }

        public override void LateInitialization(GPlayerInstance player)
        {
            if (_objects == null)
            {
                AddParameter(true, ClientParameterCode.Invalid);
                return;
            }

            List<ObjectView> views = _objects.Select(obj => obj.GetObjectView(player)).ToList();

            AddSerializedParameter(new ViewsData {Views = views}, ClientParameterCode.Object);
        }
    }
}