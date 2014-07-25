using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using MayhemCommon;
using MayhemCommon.MessageObjects;

namespace Assets.MayhemAndHell.Scripts.InGame.Handlers
{
    public class LootContainerHandler : PacketHandler
    {
        public LootContainerHandler(InGameController controller) : base(controller)
        {
        }

        public override void OnHandleEvent(EventData eventData)
        {
            ContainerData data =
                Deserialize<ContainerData>((byte[]) eventData.Parameters[(byte) ClientParameterCode.Object]);
        
            var items = new List<ContainerItem>();

            foreach (var item in data.Items)
            {
                if (item == null)
                {
                    items.Add(null);
                }
                else
                {
                    items.Add(new ContainerItem()
                    {
                        Description = item.Description,
                        Name = item.Name,
                        Quality = item.Quality,
                        SpriteName = item.SpriteName,
                        Stats = item.Stats
                    });
                }
            }

            Controller.InGameView.Gui.LootContainer.Show(items);
        }

        public override MessageSubCode MessageSubCode
        {
            get { return MessageSubCode.LootContainer; }
        }
    }
}
