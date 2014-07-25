using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using MayhemCommon;

namespace Assets.MayhemAndHell.Scripts.CharacterSelect.Handlers
{
    public class AlertHandler : PhotonOperationHandler 
    {
        public AlertHandler(ViewController controller) : base(controller)
        {
        }

        public override byte Code
        {
            get { return (byte) MessageSubCode.Alert; }
        }

        public override void OnHandleResponse(OperationResponse response)
        {
            var controller = this.controller as CharacterSelectController;
            var alert = response.DebugMessage;

            if (controller != null && alert != null)
            {
                controller.View.Gui.Alert.DisplayWarning(alert);
            }
        }
    }
}
