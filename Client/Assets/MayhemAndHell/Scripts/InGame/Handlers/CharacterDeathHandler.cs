using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using MayhemCommon;
using UnityEngine;

namespace Assets.MayhemAndHell.Scripts.InGame.Handlers
{
    public class CharacterDeathHandler : PacketHandler
    {
        public CharacterDeathHandler(InGameController controller) : base(controller)
        {
        }

        public override void OnHandleEvent(EventData eventData)
        {
            Guid instanceId = new Guid((Byte[])eventData.Parameters[(byte)ClientParameterCode.InstanceId]);

            GameObject obj = GameObject.Find(instanceId + "");
            if (obj != null)
            {
                GameCharacter character = obj.GetComponent<GameCharacter>();
                if (character != null)
                {
                    character.transform.FindChild("sprite").gameObject.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Sprites/chars")[5];
                    character.Animator.speed = 0f;
                }
            }
        }

        public override MessageSubCode MessageSubCode
        {
            get { return MessageSubCode.CharacterDeath; }
        }
    }
}
