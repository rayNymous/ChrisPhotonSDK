using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using MayhemCommon;
using MayhemCommon.MessageObjects;

namespace Assets.MayhemAndHell.Scripts.InGame.Handlers
{
    public class AttackEventHandler : PacketHandler
    {
        public AttackEventHandler(InGameController controller) : base(controller)
        {
        }

        public override void OnHandleEvent(EventData eventData)
        {
            var data = Deserialize<AttackData>((byte[])eventData.Parameters[(byte) ClientParameterCode.Object]);
        }

        public override MessageSubCode MessageSubCode
        {
            get { return MessageSubCode.Attack; }
        }
    }
}
