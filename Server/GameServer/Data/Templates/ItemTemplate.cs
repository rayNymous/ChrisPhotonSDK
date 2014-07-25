using System.Collections.Generic;
using GameServer.Model.Interfaces;
using MayhemCommon;

namespace GameServer.Data
{
    public class ItemTemplate
    {
        public List<IFunction> Stats;
        public int TemplateId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ItemType Type { get; set; }
        public int StackSize { get; set; }

        public bool IsStackable
        {
            get { return StackSize > 1; }
        }

        public string Sprite { get; set; }
        public ItemQuality Quality { get; set; }
        public int ItemLevel { get; set; }
        public EquipmentSlot Slot { get; set; }
    }
}