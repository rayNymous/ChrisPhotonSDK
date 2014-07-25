using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using MayhemCommon;
using MayhemCommon.MessageObjects;

namespace Assets.MayhemAndHell.Scripts.InGame.Handlers
{
    public class TargetStatusHandler : PacketHandler
    {
        public TargetStatusHandler(InGameController controller) : base(controller)
        {
        }

        public override void OnHandleEvent(EventData eventData)
        {
            var data = Deserialize<TargetStatusData>((byte[]) eventData.Parameters[(byte) ClientParameterCode.Object]);
            // It's the player
            if (Controller.GetPlayerId() == data.InstanceId)
            {
                Controller.InGameView.Gui.PlayerHealth.UpdateHealth(data.CurrentHealth, data.MaxHealth);
            } 
            else if(Controller.InGameView.Gui.TargetBar.InstanceId == data.InstanceId)
            {
                Controller.InGameView.Gui.TargetBar.UpdateStatus(data);
            }
            
        }

        public override MessageSubCode MessageSubCode
        {
            get { return MessageSubCode.TargetStatus; }
        }
    }
}
