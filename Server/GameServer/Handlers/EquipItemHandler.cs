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
    public class EquipItemHandler : GameRequestHandler
    {
        public EquipItemHandler(PhotonApplication application) : base(application)
        {
        }

        public override int? SubCode
        {
            get { return (int) MessageSubCode.EquipItem; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var peerId = new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]);
            Dictionary<Guid, SubServerClientPeer> clients =
                Server.ConnectionCollection<SubServerConnectionCollection>().Clients;
            var instance = clients[peerId].ClientData<GPlayerInstance>();

            bool isEquipping = Convert.ToBoolean(message.Parameters[(byte) ClientParameterCode.Object]);
            var slot = (EquipmentSlot) message.Parameters[(byte) ClientParameterCode.Object2];

            if (isEquipping)
            {
                int storageIndex = Convert.ToInt32(message.Parameters[(byte) ClientParameterCode.Object3]);

                // Equiping item from storage
                GItem item = instance.Inventory.RemoveItem(storageIndex);
                GItem prevItem = instance.Equipment.EquipItem(item);

                if (storageIndex != -1)
                {
                    instance.Inventory.AddItem(prevItem, storageIndex);
                }
                else
                {
                    instance.Inventory.AddNewItem(prevItem);
                }
            }
            else
            {
                // Unequiping item to storage
                GItem item = instance.Equipment.UnequipItem(slot);
                if (instance.Inventory.AddItem(item) < 0)
                {
                    // If failed to add to inventory, equip back
                    instance.Equipment.EquipItem(item);
                }
            }

            Log.Debug("Item equipped successfully");

            return true;
        }
    }
}