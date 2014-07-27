using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using MayhemCommon;
using MayhemCommon.MessageObjects.Views;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.MayhemAndHell.Scripts.InGame.Handlers
{
    public class ShowObjectsHandler : PacketHandler
    {
        private EntityManager _entityManager;

        public ShowObjectsHandler(InGameController controller) : base(controller)
        {
            _entityManager = Object.FindObjectOfType<EntityManager>();
        }

        public override void OnHandleEvent(EventData eventData)
        {
            byte[] rawData = (byte[])eventData.Parameters[(byte) ClientParameterCode.Object];
            if (rawData == null)
            {
                Controller.ControllerView.LogDebug("Received empty ShowObjects object");
                return;
            }
            var data = Deserialize<ViewsData>(rawData);

            foreach (var view in data.Views)    
            {
                
                if (view.InstanceId != Controller.GetPlayerId())
                {
                    Debug.Log(view.Name);
                    var obj = _entityManager.InstantiateObject(view);
                    var npcView = view as NpcView;
                    if (npcView != null && npcView.Hint != ObjectHint.None)
                    {
                        var character = obj.GetComponent<GameCharacter>();
                        if (character != null)
                        {
                            character.Hint.Show(npcView.Hint);
                        }
                    }
                }
            }

        }

        public override MessageSubCode MessageSubCode
        {
            get { return MessageSubCode.ShowObjects; }
        }
    }
}
