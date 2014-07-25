using System;
using System.Collections.Generic;
using GameServer.Factories;
using GameServer.Model;
using GameServer.Model.Items;
using GameServer.Model.ServerEvents;
using GameServer.Quests;
using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;

namespace GameServer.Handlers
{
    public class ActionRequestHandler : GameRequestHandler
    {
        public ActionRequestHandler(PhotonApplication application) : base(application)
        {
        }

        public override int? SubCode
        {
            get { return (int) MessageSubCode.ActionRequest; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var peerId = new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]);
            Dictionary<Guid, SubServerClientPeer> clients =
                Server.ConnectionCollection<SubServerConnectionCollection>().Clients;
            var instance = clients[peerId].ClientData<GPlayerInstance>();

            int actionIndex = Convert.ToInt32(message.Parameters[(byte) ClientParameterCode.Object]);

            GameAction[] actions = GameActions.GetActions(instance, instance.Target);

            if (actions != null && actionIndex >= 0 && actionIndex < actions.Length)
            {
                Log.Debug("Action found: " + actions[actionIndex]);
                GameAction action = actions[actionIndex];
                switch (action)
                {
                    case GameAction.Attack:
                        instance.StartAutoAttack();
                        break;
                    case GameAction.Trade:
                        break;
                    case GameAction.Whisper:
                        break;
                    case GameAction.Follow:
                        break;
                    case GameAction.Talk:
                        var npc = instance.Target as GNpc;
                        if (npc != null)
                        {
                            int dialogIndex = npc.Template.GetValueInt("dialog", 0);
                            if (dialogIndex < 0)
                            {
                                Log.Debug("Invalid dialog index");
                                return true;
                            }

                            Dialog dialog;

                            instance.Zone.World.GetFactory<DialogFactory>().Dialogs.TryGetValue(dialogIndex, out dialog);

                            if (dialog != null)
                            {
                                DialogPage page = dialog.Pages[0];

                                if (page == null)
                                {
                                    return true;
                                }

                                instance.Dialog = dialog;

                                instance.MoveTo(npc, () => instance.SendPacket(new DialogPagePacket(instance, page, dialog.Title)));
                            }
                            else
                            {
                                return true;
                            }
                        }
                        break;
                    case GameAction.PickUp:
                        break;
                    case GameAction.Gather:
                        break;
                    case GameAction.Mine:
                        break;
                    case GameAction.Inspect:
                        break;
                    case GameAction.Storage:
                        var storage = instance.Target as GNpc;

                        if (storage != null)
                        {
                            instance.MoveTo(storage, () => instance.SendPacket(new GlobalStorageInfo(instance)));
                        }

                        break;
                    case GameAction.Loot:
                        var character = instance.Target as GCharacter;
                        if (character != null)
                        {
                            GContainer loot = character.Loot;
                            if (loot != null)
                            {
                                Log.Debug("Sent Loot container");
                                instance.MoveTo(character, () => instance.SendPacket(new LootContainer(loot)));
                            }
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            Log.Debug("Action request successful");

            return true;
        }
    }
}