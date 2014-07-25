using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using MayhemCommon;
using MayhemCommon.MessageObjects;

namespace Assets.MayhemAndHell.Scripts.InGame.Handlers
{
    public class StopMoveHandler : PacketHandler
    {
        public StopMoveHandler(InGameController controller) : base(controller)
        {
        }

        public override void OnHandleEvent(EventData eventData)
        {
            //var instanceId = new Guid((Byte[])eventData.Parameters[(byte)ClientParameterCode.InstanceId]);
            //var data = Deserialize<PositionData>((byte[])eventData.Parameters[(byte)ClientParameterCode.Object]);
        }

        public override MessageSubCode MessageSubCode
        {
            get { return MessageSubCode.StopMove; }
        }
    }
}
