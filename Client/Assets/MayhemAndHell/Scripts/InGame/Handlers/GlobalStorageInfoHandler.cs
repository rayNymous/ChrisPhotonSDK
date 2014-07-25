using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using MayhemCommon;
using MayhemCommon.MessageObjects;

namespace Assets.MayhemAndHell.Scripts.InGame.Handlers
{
    public class GlobalStorageInfoHandler : PacketHandler
    {
        public GlobalStorageInfoHandler(InGameController controller) : base(controller)
        {
        }

        public override void OnHandleEvent(EventData eventData)
        {
            var data =
                Deserialize<GlobalStorageData>((byte[])eventData.Parameters[(byte)ClientParameterCode.Object]);

            Controller.InGameView.Gui.Storage.Show(data);
        }

        public override MessageSubCode MessageSubCode
        {
            get { return MessageSubCode.GlobalStorageData; }
        }
    }
}
