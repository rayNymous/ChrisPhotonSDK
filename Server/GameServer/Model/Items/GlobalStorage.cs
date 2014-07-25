using System;
using System.Collections.Generic;
using ExitGames.Logging;
using GameServer.Factories;
using GameServer.Model.ServerEvents;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using NHibernate;
using SubServerCommon;
using SubServerCommon.Data.NHibernate;

namespace GameServer.Model.Items
{
    public class GlobalStorage : GContainer
    {
        public int Coins;

        protected ILogger Log = LogManager.GetCurrentClassLogger();

        public GlobalStorage(GPlayerInstance owner)
            : base(ContainerType.Inventory, 8)
        {
            Owner = owner;
        }

        public GPlayerInstance Owner { get; protected set; }

        public GlobalStorageData Data
        {
            get
            {
                var items = new Dictionary<int, ItemData>();

                for (int i = 0; i < Items.Length; i++)
                {
                    items.Add(i, Items[i]);
                }

                return new GlobalStorageData
                {
                    Items = items,
                    Coins = Coins
                };
            }
        }

        public bool AddNewItem(GItem item)
        {
            int itemIndex = AddItem(item);
            if (itemIndex < 0)
            {
                Owner.SendPacket(new EventNotification("Storage is full"));
            }
            else
            {
                Owner.SendPacket(new EventNotification("Item \"" + item.Name + "\" added to globalStorage"));
                return true;
            }
            return false;
        }

        public override int AddItem(GItem item, int index = -1)
        {
            int slot = base.AddItem(item, index);
            if (slot >= 0)
            {
                SaveItemAtSlot(slot);
                Owner.SendPacket(new ContainerSetItem(slot, item, ContainerType.GlobalStorage));
            }
            return slot;
        }

        private void SaveItemAtSlot(int slot)
        {
            GItem itemInstance = Items[slot];
            try
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        User user = session.QueryOver<User>().Where(u => u.Id == Owner.UserId).SingleOrDefault();

                        var currentItem = session.Get<MayhemItem>(itemInstance.InstanceId);

                        MayhemItem itemAtPosition = session.QueryOver<MayhemItem>()
                            .Where(i => i.Container == user.Inventory && i.SlotIndex == slot).SingleOrDefault();

                        // If there's another item at the position, remove it
                        if (itemAtPosition != null && currentItem.Id != itemAtPosition.Id)
                        {
                            session.Delete(itemAtPosition);
                        }

                        if (currentItem != null)
                        {
                            currentItem.Container = user.Inventory;
                            currentItem.SlotIndex = slot;
                        }
                        else
                        {
                            currentItem = new MayhemItem
                            {
                                Container = user.Inventory,
                                Id = itemInstance.InstanceId,
                                SlotIndex = slot,
                                TemplateId = itemInstance.Template.TemplateId
                            };
                        }
                        Log.Debug("GLOBAL STORAGE ITEM SAVED");
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

        /// <summary>
        ///     Like a regular add item, except if destination slot is already taken,
        ///     ties to add item anywhere else.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        public void AddItemSafe(GItem item, int index = -1)
        {
            if (AddItem(item, index) < 0)
            {
                if (AddItem(item) < 0)
                {
                    Owner.SendPacket(new EventNotification("Storage is full. Your item has been lost. Crap :(."));
                }
            }
        }

        public void SwitchItems(int indexA, int indexB)
        {
            GItem itemA = RemoveItem(indexA);
            GItem itemB = RemoveItem(indexB);

            AddItem(itemA, indexB);
            AddItem(itemB, indexA);
        }

        public void RestoreFromDatabase()
        {
            try
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        User user = session.QueryOver<User>().Where(u => u.Id == Owner.UserId).SingleOrDefault();

                        Coins = user.Coins;

                        IList<MayhemItem> currentItems = user.Inventory.Items;

                        var itemFactory = Owner.Zone.World.GetFactory<ItemFactory>();

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