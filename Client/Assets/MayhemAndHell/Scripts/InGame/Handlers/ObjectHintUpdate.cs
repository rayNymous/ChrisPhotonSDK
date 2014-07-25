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
    public class ObjectHintUpdate : PacketHandler
    {
        public ObjectHintUpdate(InGameController controller) : base(controller)
        {
        }

        public override void OnHandleEvent(EventData eventData)
        {
            var data = Deserialize<ObjectHints>((byte[])eventData.Parameters[(byte) ClientParameterCode.Object]);
            Controller.InGameView.LogDebug("GOT THIS");
            if (data != null)
            {
                for (int i = 0; i < data.Ids.Length; i++)
                {
                    var obj = GetObjectById(data.Ids[i]);
                    if (obj != null)
                    {
                        if (obj.Hint != null)
                        {
                            obj.Hint.Show(data.Hints[i]);
                        }
                    }
                }
            }
            else
            {
                Debug.Log("ObjectHintUpdate data equals null");
            }
        }

        public override MessageSubCode MessageSubCode
        {
            get { return MessageSubCode.ObjectHint; }
        }
    }
}
