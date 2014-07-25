using System;
using System.Collections.Generic;
using GameServer.Model;
using GameServer.Model.Items;
using GameServer.Model.ServerEvents;
using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;

namespace GameServer.Handlers
{
    public class TakeItemHandler : GameRequestHandler
    {
        public TakeItemHandler(PhotonApplication application) : base(application)
        {
        }

        public override int? SubCode
        {
            get { return (int) MessageSubCode.TakeItem; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var peerId = new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]);
            Dictionary<Guid, SubServerClientPeer> clients =
                Server.ConnectionCollection<SubServerConnectionCollection>().Clients;
            var instance = clients[peerId].ClientData<GPlayerInstance>();

            int index = Convert.ToInt32(message.Parameters[(byte) ClientParameterCode.Object]);
            var character = instance.Target as GCharacter;
            if (character != null)
            {
                GContainer loot = character.Loot;
                if (loot != null)
                {
                    GItem item = loot.RemoveItem(index);
                    Log.Debug("Item removed");
                    if (item != null)
                    {
                        if (instance.Inventory.AddNewItem(item))
                        {
                            Log.Debug("Item added");
                            instance.SendPacket(new LootContainerItemRemove(index));
                            Log.Debug("Packet sent");
                        }
                        else
                        {
                            Log.Debug("Item returned");
                            loot.AddItem(item, index);
                        }
                    }
                }
            }
            return true;
        }
    }
}