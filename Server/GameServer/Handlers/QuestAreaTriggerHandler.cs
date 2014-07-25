using System;
using System.Collections.Generic;
using GameServer.Model;
using GameServer.Model.ServerEvents;
using GameServer.Quests;
using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;

namespace GameServer.Handlers
{
    public class QuestAreaTriggerHandler : GameRequestHandler
    {
        public QuestAreaTriggerHandler(PhotonApplication application) : base(application)
        {
        }

        public override int? SubCode
        {
            get { return (int) MessageSubCode.QuestAreaTrigger; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var peerId = new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]);
            Dictionary<Guid, SubServerClientPeer> clients =
                Server.ConnectionCollection<SubServerConnectionCollection>().Clients;
            var instance = clients[peerId].ClientData<GPlayerInstance>();

            int questId = Convert.ToInt32(message.Parameters[(byte) ClientParameterCode.Object]);
            string areaName = Convert.ToString(message.Parameters[(byte) ClientParameterCode.Object2]);

            Log.Debug("Testasss 1 ");

            QuestState questState = instance.GetQuestState(questId);

            if (questState != null)
            {
                Log.Debug("Testasss 2 ");
                string notification = questState.Quest.OnAreaTrigger(instance, areaName);
                if (notification != null)
                {
                    Log.Debug("Testasss 3 ");
                    instance.SendPacket(new EventNotification(notification));
                }
            }

            return true;
        }
    }
}