using System;
using System.Collections.Generic;
using GameServer.Data;
using GameServer.Model.Interfaces;
using GameServer.Model.Items;
using MayhemCommon;

namespace GameServer.Factories
{
    public class ContainerFactory : IFactory
    {
        private readonly ItemFactory _itemFactory;
        private readonly Random _random;

        public ContainerFactory(ItemFactory itemFactory)
        {
            _itemFactory = itemFactory;
            _random = new Random();
        }

        public GContainer GetLootContainer(IEnumerable<ItemDrop> dropList)
        {
            var items = new List<GItem>();

            lock (_random)
            {
                float dropChance;
                foreach (ItemDrop drop in dropList)
                {
                    dropChance = (float) _random.NextDouble();
                    GItem item = _itemFactory.GetItemFromTemplate(drop.TemplateId);

                    if (item != null && dropChance < drop.DropChance)
                    {
                        item.StackSize = _random.Next(drop.MinStackSize, drop.MaxStackSize);
                        if (item.StackSize > 0)
                        {
                            items.Add(item);
                        }
                    }
                }
            }

            if (items.Count > 0)
            {
                var container = new GContainer(ContainerType.Loot, items.Count);
                for (int i = 0; i < items.Count; i++)
                {
                    container.AddItem(items[i], i);
                }
                return container;
            }

            return null;
        }
    }
}