using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using MayhemCommon;

namespace Assets.MayhemAndHell.Scripts.InGame.Handlers
{
    public class LootContainerItemRemoveHandler : PacketHandler
    {
        public LootContainerItemRemoveHandler(InGameController controller)
            : base(controller)
        {
        }

        public override void OnHandleEvent(EventData eventData)
        {
            int index = Convert.ToInt32(eventData.Parameters[(byte) ClientParameterCode.Object]);
            Controller.InGameView.Gui.LootContainer.RemoveItemAt(index);
        }

        public override MessageSubCode MessageSubCode
        {
            get { return MessageSubCode.LootContainerItemRemove; }
        }
    }
}
