using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using MayhemCommon;

namespace Assets.MayhemAndHell.Scripts.InGame.Handlers
{
    public class PlayerStatusHandler : PacketHandler
    {
        public PlayerStatusHandler(InGameController controller) : base(controller)
        {
        }

        public override void OnHandleEvent(EventData eventData)
        {
            var data = (Dictionary<byte, object>)eventData.Parameters[(byte)ClientParameterCode.Object];

            if (data.ContainsKey((byte) PlayerStatusValue.Coins))
            {
                Controller.InGameView.Gui.CoinsBar.UpdateCount(Convert.ToInt32(data[(byte)PlayerStatusValue.Coins]));
            }
            if (data.ContainsKey((byte) PlayerStatusValue.Heat))
            {
                Controller.InGameView.Gui.HeatBar.UpdateCount(Convert.ToInt32(data[(byte)PlayerStatusValue.Heat]));
            }
            if (data.ContainsKey((byte)PlayerStatusValue.GlobalCoins))
            {
                Controller.InGameView.Gui.Storage.SetCoins(Convert.ToInt32(data[(byte)PlayerStatusValue.GlobalCoins]));
            }
        }

        public override MessageSubCode MessageSubCode
        {
            get { return MessageSubCode.PlayerStatus; }
        }
    }
}
