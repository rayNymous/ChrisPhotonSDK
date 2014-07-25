using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using UnityEngine;

namespace Assets.MayhemAndHell.Scripts.InGame.Handlers
{
    public class TargetHandler : PacketHandler
    {
        public TargetHandler(InGameController controller) : base(controller)
        {
        }

        public override void OnHandleEvent(EventData eventData)
        {
            TargetInfo info = Deserialize<TargetInfo>((byte[]) eventData.Parameters[(byte) ClientParameterCode.Object]);
            if (info != null)
            {
                GameObject targetedObject = GameObject.Find(info.InstanceId + "");

                // Show target on object
                var obj = targetedObject.GetComponent<WorldObject>();
                Controller.InGameView.Target.Enable(obj);

                // Show target buttons
                Actions actions = Controller.InGameView.Gui.Actions;
                actions.PrepareActions(info.Actions);

                // Target bar
                Controller.InGameView.Gui.TargetBar.Show(info);
                
            }
            else
            {
                Debug.Log("TARGET LOST");
                Controller.InGameView.Gui.TargetLost();
            }
            
        }

        public override MessageSubCode MessageSubCode
        {
            get { return MessageSubCode.Target; }
        }
    }
}
