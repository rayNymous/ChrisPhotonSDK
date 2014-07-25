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
    public class DeleteObjectsHandler : PacketHandler
    {
        public DeleteObjectsHandler(InGameController controller) : base(controller)
        {
        }

        public override void OnHandleEvent(EventData eventData)
        {
            byte[] rawData = (byte[])eventData.Parameters[(byte) ClientParameterCode.Object];
            if (rawData == null)
            {
                return;
            }

            DeleteObjectsData data = Deserialize<DeleteObjectsData>(rawData);

            if (data != null)
            {
                foreach (var instanceId in data.Ids)
                {
                    GameObject obj = GameObject.Find(instanceId + "");

                    if (Controller.InGameView.Target.Target != null 
                        && instanceId == Controller.InGameView.Target.Target.InstanceId)
                    {
                        Controller.InGameView.Gui.TargetLost();
                    }

                    var worldObject = obj.GetComponent<WorldObject>();
                    if (worldObject != null)
                    {
                        worldObject.PrepareForDestruction();
                    }

                    UnityEngine.Object.Destroy(obj);
                }
            }
        }

        public override MessageSubCode MessageSubCode
        {
            get { return MessageSubCode.DeleteObjects; }
        }
    }
}
