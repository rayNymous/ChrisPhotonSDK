using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using MayhemCommon;

public class DeathNotificationHandler : PacketHandler
{
    public DeathNotificationHandler(InGameController controller) : base(controller)
    {
    }

    public override void OnHandleEvent(EventData eventData)
    {
        Controller.InGameView.Gui.DeathWindow.Show();
    }

    public override MessageSubCode MessageSubCode
    {
        get { return MessageSubCode.DeathNotification; }
    }
}
