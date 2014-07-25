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
    public class ContainerSetItemHandler : PacketHandler
    {
        public ContainerSetItemHandler(InGameController controller) : base(controller)
        {
        }

        public override void OnHandleEvent(EventData eventData)
        {
            int index = Convert.ToInt32(eventData.Parameters[(byte) ClientParameterCode.Object]);
            var containerType = (ContainerType) Convert.ToInt32(eventData.Parameters[(byte) ClientParameterCode.Object3]);

            ItemData data = null;

            if (eventData.Parameters.ContainsKey((byte) ClientParameterCode.Object2))
            {
                data = Deserialize<ItemData>((byte[]) eventData.Parameters[(byte) ClientParameterCode.Object2]);
            }

            ContainerItem item = null;
            if (data != null) {
                item = new ContainerItem()
                {
                    Description = data.Description,
                    Name = data.Name,
                    Quality = data.Quality,
                    SpriteName = data.SpriteName,
                    Stats = data.Stats,
                    Atlas = Controller.InGameView.Gui.IconAtlas,
                    Slot = data.Slot
                };
            }

            switch (containerType)
            {
                case ContainerType.Inventory:
                    Controller.InGameView.Gui.Inventory.Storage.ForceReplace(index, item);
                    break;
                case ContainerType.GlobalStorage:
                    Controller.InGameView.Gui.Storage.GlobalStorage.ForceReplace(index, item);
                    break;
                case ContainerType.Equipment:
                    Debug.Log("ForceReplace Equipment " + index);
                    Controller.InGameView.Gui.Equipment.ForceEquip(item, (MayhemCommon.EquipmentSlot)index);
                break;
            }
        }

        public override MessageSubCode MessageSubCode
        {
            get { return MessageSubCode.ContainerSetItem; }
        }
    }
}
