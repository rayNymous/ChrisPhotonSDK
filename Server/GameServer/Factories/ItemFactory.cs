using System;
using System.Collections.Generic;
using ExitGames.Logging;
using GameServer.Calculators.Functions;
using GameServer.Calculators.Lambdas;
using GameServer.Data;
using GameServer.Model.Interfaces;
using GameServer.Model.Items;
using GameServer.Model.Stats;
using MayhemCommon;

namespace GameServer.Factories
{
    public class ItemFactory : IFactory
    {
        public Dictionary<int, ItemTemplate> Items;

        protected ILogger Log = LogManager.GetCurrentClassLogger();

        public ItemFactory()
        {
            Items = new Dictionary<int, ItemTemplate>();
            Items.Add(0, new ItemTemplate
            {
                TemplateId = 0,
                Description = "Worst coins ever",
                Sprite = "bronze_coins",
                Name = "Bronze Coins",
                StackSize = 1000,
                Type = ItemType.Currency,
                ItemLevel = 1,
                Quality = ItemQuality.Normal,
                Stats = null
            });
            Items.Add(1, new ItemTemplate
            {
                TemplateId = 1,
                Name = "Herbs",
                Description = "A collection of strange smelling herbs",
                Sprite = "herbs",
                StackSize = 20,
                Type = ItemType.Material,
                ItemLevel = 1,
                Quality = ItemQuality.Normal,
                Stats = null
            });
            Items.Add(2, new ItemTemplate
            {
                TemplateId = 2,
                Name = "Shield",
                Description = "Wooden shield of a weak barbarian",
                Sprite = "shield",
                StackSize = 1,
                Type = ItemType.Equipment,
                ItemLevel = 1,
                Quality = ItemQuality.Legendary,
                Slot = EquipmentSlot.LeftHand,
                Stats = new List<IFunction>
                {
                    new FunctionAdd(new Armor(), 0, null, new LambdaConstant(10)),
                    new FunctionAdd(new FrostResistance(), 0, null, new LambdaConstant(10))
                }
            });
            Items.Add(3, new ItemTemplate
            {
                TemplateId = 3,
                Name = "Sword",
                Description = "More like a butter knife",
                Sprite = "sword",
                StackSize = 1,
                Type = ItemType.Equipment,
                ItemLevel = 1,
                Quality = ItemQuality.Enchanted,
                Slot = EquipmentSlot.RightHand,
                Stats = new List<IFunction>
                {
                    new FunctionAdd(new Strength(), 0, null, new LambdaConstant(1))
                }
            });

            Items.Add(4, new ItemTemplate
            {
                TemplateId = 4,
                Name = "Wood",
                Description = "Exactly",
                Sprite = "wood",
                StackSize = 1,
                Type = ItemType.Material,
                ItemLevel = 1,
                Quality = ItemQuality.Normal,
                Slot = EquipmentSlot.RightHand,
                Stats = null
            });
        }

        public GItem GetItemFromTemplate(ItemTemplate template)
        {
            if (template == null)
            {
                return null;
            }
            GItem item = null;
            switch (template.Type)
            {
                case ItemType.Junk:
                    return new GItem(template, Guid.NewGuid());
                    break;
                case ItemType.Usable:
                    return new GItem(template, Guid.NewGuid());
                    break;
                case ItemType.Material:
                    return new GItem(template, Guid.NewGuid());
                    break;
                case ItemType.Equipment:
                    return new GItem(template, Guid.NewGuid());
                    break;
            }
            return item;
        }

        public GItem GetItemFromTemplate(int templadeId)
        {
            ItemTemplate template;
            Items.TryGetValue(templadeId, out template);
            return GetItemFromTemplate(Items[templadeId]);
        }
    }
}