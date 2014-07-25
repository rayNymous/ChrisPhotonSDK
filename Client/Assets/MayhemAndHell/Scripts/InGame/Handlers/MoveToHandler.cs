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
    public class MoveToHandler : PacketHandler
    {
        public MoveToHandler(InGameController controller) : base(controller)
        {
        }

        public override void OnHandleEvent(EventData eventData)
        {
            var instanceId = Convert.ToInt32(eventData.Parameters[(byte) ClientParameterCode.InstanceId]);
            var position = Deserialize<MoveToData>((byte[])eventData.Parameters[(byte)ClientParameterCode.Object]);

            var obj = GetObjectById(instanceId);
            if (obj != null)
            {
                var character = obj.GetComponent<GameCharacter>();
                if (character != null)
                {
                    float duration = Vector2.Distance(character.transform.position,
                        new Vector2(position.Destination.X, position.Destination.Y))/position.Speed;
                    character.MoveTo(position.Destination.X, position.Destination.Y, duration);
                }
            }
            else
            {
                Debug.Log("FAILAS");
            }
        }

        public override MessageSubCode MessageSubCode
        {
            get { return MessageSubCode.MoveTo; }
        }
    }
}
