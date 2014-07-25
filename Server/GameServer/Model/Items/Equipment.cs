using System;
using System.Collections.Generic;
using ExitGames.Logging;
using GameServer.Factories;
using GameServer.Model.Interfaces;
using GameServer.Model.ServerEvents;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using NHibernate;
using SubServerCommon;
using SubServerCommon.Data.NHibernate;

namespace GameServer.Model.Items
{
    public class Equipment : GContainer
    {
        private readonly GPlayerInstance _player;
        protected ILogger Log = LogManager.GetCurrentClassLogger();

        public Equipment(GPlayerInstance player)
            : base(ContainerType.Equipment, Enum.GetNames(typeof (EquipmentSlot)).Length)
        {
            _player = player;
        }

        public Dictionary<int, ItemData> Data
        {
            get
            {
                var items = new Dictionary<int, ItemData>();

                for (int i = 0; i < Items.Length; i++)
                {
                    items.Add(i, Items[i]);
                }

                return items;
            }
        }

        public GItem UnequipItem(EquipmentSlot slot)
        {
            GItem item = RemoveItem((byte) slot);

            // Removing stat modifiers from previous items
            if (item != null && item.Template.Stats != null)
            {
                foreach (IFunction function in item.Template.Stats)
                {
                    _player.Stats.RemoveModifier(function);
                }
            }

            _player.SendPacket(new ContainerSetItem((byte) slot, null, ContainerType.Equipment));

            return item;
        }

        public GItem EquipItem(GItem item)
        {
            if (item.Type != ItemType.Equipment)
            {
                return item;
            }

            GItem prevItem = UnequipItem(item.Slot);

            AddItem(item, (byte) item.Slot);

            // Adding stat modifiers from current item
            if (item.Template.Stats != null)
            {
                foreach (IFunction function in item.Template.Stats)
                {
                    _player.Stats.AddModifier(function);
                }
            }

            if (item != prevItem)
            {
                Log.Debug("Save item");
                SaveItemAtSlot((byte) item.Slot);
                _player.SendPacket(new ContainerSetItem((byte) item.Slot, item, ContainerType.Equipment));
            }

            return prevItem;
        }

        private void SaveItemAtSlot(int slot)
        {
            GItem itemInstance = Items[slot];
            if (itemInstance == null)
            {
                return;
            }
            try
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        var character = session.Get<MayhemCharacter>(_player.GlobalId);

                        var currentItem = session.Get<MayhemItem>(itemInstance.InstanceId);

                        MayhemItem itemAtPosition = session.QueryOver<MayhemItem>()
                            .Where(i => i.Container == character.Equipment && i.SlotIndex == slot).SingleOrDefault();

                        // If there's another item at the position, remove it
                        if (itemAtPosition != null && currentItem.Id != itemAtPosition.Id)
                        {
                            Log.Debug("TESTAS3");
                            session.Delete(itemAtPosition);
                        }

                        if (currentItem != null)
                        {
                            Log.Debug("TESTAS1");
                            currentItem.Container = character.Equipment;
                            currentItem.SlotIndex = slot;
                        }
                        else
                        {
                            Log.Debug("TESTAS2");
                            currentItem = new MayhemItem
                            {
                                Container = character.Equipment,
                                Id = itemInstance.InstanceId,
                                SlotIndex = slot,
                                TemplateId = itemInstance.Template.TemplateId
                            };
                        }
                        Log.Debug("INVENTORY ITEM SAVED");
                        session.SaveOrUpdate(currentItem);

                        transaction.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void RestoreFromDatabase()
        {
            try
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        MayhemCharacter character =
                            session.QueryOver<MayhemCharacter>()
                                .Where(c => c.Id == _player.GlobalId)
                                .SingleOrDefault();

                        //var currentItems =
                        //    session.QueryOver<MayhemItem>().Where(i => i.Container == character.Inventory).List();
                        IList<MayhemItem> currentItems = character.Equipment.Items;

                        if (currentItems == null)
                        {
                            Log.Debug("NULLLLLLLLLLLLLLLLLLLLLLLLLLLL");
                        }

                        if (currentItems != null)
                        {
                            Log.Debug("VA SITIEK: " + currentItems.Count);
                        }

                        var itemFactory = _player.Zone.World.GetFactory<ItemFactory>();

                        transaction.Commit();

                        // Wipe out unnecessary data
                        Clear();

                        foreach (MayhemItem mayhemItem in currentItems)
                        {
                            ForceAddItem(mayhemItem.SlotIndex,
                                new GItem(itemFactory.Items[mayhemItem.TemplateId], mayhemItem.Id));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}