using System;
using System.Collections.Generic;
using GameServer.Model;
using GameServer.Model.Items;
using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;

namespace GameServer.Handlers
{
    public class ItemTransferHandler : GameRequestHandler
    {
        public ItemTransferHandler(PhotonApplication application) : base(application)
        {
        }

        public override int? SubCode
        {
            get { return (int) MessageSubCode.ItemTransfer; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var peerId = new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]);
            Dictionary<Guid, SubServerClientPeer> clients =
                Server.ConnectionCollection<SubServerConnectionCollection>().Clients;
            var instance = clients[peerId].ClientData<GPlayerInstance>();

            var data = (Dictionary<string, object>) message.Parameters[(byte) ClientParameterCode.Object];

            var fromContainer = (ContainerType) Convert.ToByte(data["fromC"]);
            var toContainer = (ContainerType) Convert.ToByte(data["toC"]);

            if (fromContainer == ContainerType.Inventory)
            {
                int fromSlot = Convert.ToInt32(data["fromS"]);

                if (toContainer == ContainerType.Inventory)
                {
                    // From inventory to inventory
                    int toSlot = Convert.ToInt32(data["toS"]);
                    instance.Inventory.SwitchItems(fromSlot, toSlot);
                    Log.Debug("Switching inventory items");
                }
                else if (toContainer == ContainerType.GlobalStorage)
                {
                    // From inventory to global
                    int toSlot = Convert.ToInt32(data["toS"]);

                    GItem itemA = instance.Inventory.RemoveItem(fromSlot);
                    GItem itemB = instance.Storage.RemoveItem(toSlot);

                    instance.Storage.AddItemSafe(itemA, toSlot);
                    instance.Inventory.AddItemSafe(itemB, fromSlot);
                }
                else if (toContainer == ContainerType.Equipment)
                {
                    // From inventory to equipment
                    GItem item = instance.Inventory.RemoveItem(fromSlot);

                    GItem prevItem = instance.Equipment.EquipItem(item);

                    instance.Inventory.AddItemSafe(prevItem, fromSlot);
                }
            }
            else if (fromContainer == ContainerType.GlobalStorage)
            {
                int fromSlot = Convert.ToInt32(data["fromS"]);

                if (toContainer == ContainerType.Inventory)
                {
                    // From global to inventory
                    int toSlot = Convert.ToInt32(data["toS"]);

                    GItem itemA = instance.Storage.RemoveItem(fromSlot);
                    GItem itemB = instance.Inventory.RemoveItem(toSlot);

                    instance.Inventory.AddItemSafe(itemA, toSlot);
                    instance.Storage.AddItemSafe(itemB, fromSlot);
                }
                else if (toContainer == ContainerType.GlobalStorage)
                {
                    // From global to global
                    int toSlot = Convert.ToInt32(data["toS"]);
                    instance.Storage.SwitchItems(fromSlot, toSlot);
                }
                else if (toContainer == ContainerType.Equipment)
                {
                    // From global to equipment
                    GItem item = instance.Storage.RemoveItem(fromSlot);

                    GItem prevItem = instance.Equipment.EquipItem(item);

                    instance.Storage.AddItemSafe(prevItem, fromSlot);
                }
            }

            return true;
        }
    }
}