using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using MayhemCommon;
using UnityEngine;

namespace Assets.MayhemAndHell.Scripts.InGame.Handlers
{
    public class ObjectNotificationHandler : PacketHandler
    {
        public ObjectNotificationHandler(InGameController controller) : base(controller)
        {
        }

        public override void OnHandleEvent(EventData eventData)
        {
            var instanceId = new Guid((Byte[])eventData.Parameters[(byte)ClientParameterCode.InstanceId]);
            var notification = Convert.ToString(eventData.Parameters[(byte) ClientParameterCode.Object]);

            GameObject obj = GameObject.Find(instanceId + "");
            if (obj != null)
            {
                GameCharacter character = obj.GetComponent<GameCharacter>();
                if (character != null)
                {
                    character.ScrollingText.Add(notification, Color.yellow, 0.4f);
                }
            }
        }

        public override MessageSubCode MessageSubCode
        {
            get { return MessageSubCode.ObjectNotification; }
        }
    }
}
