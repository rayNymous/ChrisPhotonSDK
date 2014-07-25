using System;
using System.Linq;
using GameServer.Data;
using GameServer.Model.Interfaces;
using MayhemCommon;
using MayhemCommon.MessageObjects;

namespace GameServer.Model.Items
{
    public class GItem : IItem
    {
        public GItem(ItemTemplate template, Guid id)
        {
            Template = template;
            InstanceId = id;
        }

        public ItemTemplate Template { get; set; }
        public Guid InstanceId { get; set; }

        public int StackSize { get; set; }

        public int ItemLevel
        {
            get { return Template.ItemLevel; }
        }

        public ItemQuality Quality
        {
            get { return Template.Quality; }
        }

        public string SpriteName
        {
            get { return Template.Sprite; }
        }

        public EquipmentSlot Slot
        {
            get { return Template.Slot; }
        }

        public ItemType Type
        {
            get { return Template.Type; }
        }

        public bool Equipable { get; set; }

        public string Name
        {
            get { return Template.Name; }
        }

        public string Description
        {
            get { return Template.Description; }
        }

        public static implicit operator ItemData(GItem item)
        {
            if (item == null)
            {
                return null;
            }

            return new ItemData
            {
                Description = item.Description,
                ItemLevel = item.ItemLevel,
                Name = item.Name,
                Quality = item.Quality,
                SpriteName = item.SpriteName,
                Stats = item.Template.Stats != null
                    ? item.Template.Stats.ToDictionary(function => function.StringFormat, function => function.LambdaValue)
                    : null,
                Slot = item.Slot
            };
        }
    }
}