using System;
using System.Collections.Generic;
using System.IO;
using GameServer.Factories;
using GameServer.Model;
using GameServer.Model.ServerEvents;
using GameServer.Quests;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;
using ProtoBuf;
using SubServerCommon;

namespace GameServer.Handlers
{
    public class DialogActionHandler : GameRequestHandler
    {
        public DialogActionHandler(PhotonApplication application) : base(application)
        {
        }

        public override int? SubCode
        {
            get { return (int) MessageSubCode.DialogAction; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            Log.Debug("DialogActionHandler found an action");
            var peerId = new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]);
            Dictionary<Guid, SubServerClientPeer> clients =
                Server.ConnectionCollection<SubServerConnectionCollection>().Clients;
            var instance = clients[peerId].ClientData<GPlayerInstance>();

            var rawAction = (byte[]) message.Parameters[(byte) ClientParameterCode.Object];

            DialogAction action;

            using (var ms = new MemoryStream(rawAction))
            {
                action = Serializer.Deserialize<DialogAction>(ms);
            }

            Dialog dialog = instance.Dialog;

            if (dialog == null)
            {
                return true;
            }

            // Checking if any of the bottom buttons were clicked
            if (action.Index < 0)
            {
                if (action.Index == -1)
                {
                    // Right button (Goodbye)
                    instance.Dialog = null;
                }
                else if (action.Index == -2)
                {
                    // Left button
                    Quest quest = dialog.Quest;
                    if (quest != null)
                    {
                        QuestState questState = instance.GetQuestState(quest);
                        if (questState == null)
                        {
                            quest.StartQuest(instance);
                        }
                        else if (questState.Progress == QuestProgressState.TurnIn)
                        {
                            questState.SetProgress(QuestProgressState.Completed);
                            quest.OnQuestCompleted(instance);
                            instance.SendPacket(new EventNotification("'" + quest.Name + "' completed"));
                        }
                    }
                }
                return true;
            }

            if (action.CurrentPage < dialog.Pages.Length)
            {
                DialogPage page = dialog.Pages[action.CurrentPage];

                List<DialogLink> links = page.GetLinks(instance, true);
                DialogLink link = action.Index < links.Count ? links[action.Index] : null;

                if (link != null)
                {
                    DialogPage newPage = null;
                    switch (link.Type)
                    {
                        case DialogLinkType.Unknown:
                            break;
                        case DialogLinkType.Page:
                            newPage = dialog.Pages[link.Target];
                            break;
                        case DialogLinkType.QuestStart:
                            Quest questStart;
                            instance.Zone.World.GetFactory<QuestFactory>()
                                .Quests.TryGetValue(link.Target, out questStart);
                            if (questStart != null)
                            {
                                dialog = questStart.StartDialog;
                                newPage = dialog.Pages[0];
                            }
                            break;
                        case DialogLinkType.QuestComplete:
                            Quest questComplete;
                            instance.Zone.World.GetFactory<QuestFactory>()
                                .Quests.TryGetValue(link.Target, out questComplete);
                            if (questComplete != null)
                            {
                                dialog = questComplete.CompleteDialog;
                                newPage = dialog.Pages[0];
                            }
                            break;
                        case DialogLinkType.Shop:
                            break;
                    }

                    // Dialog might have changed
                    instance.Dialog = dialog;

                    if (newPage != null)
                    {
                        instance.SendPacket(new DialogPagePacket(instance, newPage, dialog.Title));
                    }
                }
            }

            return true;
        }
    }
}