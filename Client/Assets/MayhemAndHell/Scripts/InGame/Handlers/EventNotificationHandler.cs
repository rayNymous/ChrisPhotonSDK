using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using MayhemCommon;

namespace Assets.MayhemAndHell.Scripts.InGame.Handlers
{
    public class EventNotificationHandler : PacketHandler
    {
        public EventNotificationHandler(InGameController controller) : base(controller)
        {
        }

        public override void OnHandleEvent(EventData eventData)
        {
            String message = Convert.ToString(eventData.Parameters[(byte)ClientParameterCode.Object]);
            Controller.InGameView.Gui.EventNotifications.AddNotification(message);
        }

        public override MessageSubCode MessageSubCode
        {
            get { return MessageSubCode.EventNotification; }
        }
    }
}
