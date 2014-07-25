using System;
using System.Collections.Generic;
using ExitGames.Logging;
using GameServer.Data;
using GameServer.Data.Templates;
using GameServer.Model;
using GameServer.Model.ServerEvents;
using SubServerCommon;

namespace GameServer.Quests
{
    public abstract class Quest
    {
        protected ILogger Log = LogManager.GetCurrentClassLogger();

        public List<NpcTemplate> RelatedNpcs = new List<NpcTemplate>();

        protected Quest(NpcFactory npcFactory)
        {
        }

        public abstract String Name { get; }
        public abstract int QuestId { get; }

        public Dialog StartDialog { get; set; }
        public Dialog CompleteDialog { get; set; }

        public DialogLink QuestStartLink { get; set; }
        public DialogLink QuestCompleteLink { get; set; }

        public void AddEventToNpc(NpcFactory npcFactory, QuestEventType eventType, int npcId)
        {
            NpcTemplate template = npcFactory.GetTemplate(npcId);
            if (template != null)
            {
                template.AddQuestEvent(eventType, this);
                if (!RelatedNpcs.Contains(template))
                {
                    RelatedNpcs.Add(template);
                }
            }
        }

        public virtual string OnKill(GPlayerInstance killer, GNpc prey)
        {
            return null;
        }

        public virtual string OnQuestStarted(GPlayerInstance player, QuestState state)
        {
            state.SetProgress(QuestProgressState.Started);
            return null;
        }

        public virtual void OnQuestCompleted(GPlayerInstance player)
        {
        }

        public virtual bool CanStart(GPlayerInstance player)
        {
            QuestState state = player.GetQuestState(this);
            if (state == null)
            {
                return true;
            }
            return false;
        }

        public virtual string OnAreaTrigger(GPlayerInstance player, string areaName)
        {
            return null;
        }

        public void StartQuest(GPlayerInstance player)
        {
            QuestState state = player.GetQuestState(this);
            if (state == null)
            {
                state = new QuestState(this, player);
                player.AddQuestState(state);

                String notification = OnQuestStarted(player, state);
                notification = notification ?? "'" + Name + "' started";
                player.SendPacket(new EventNotification(notification));
            }
            Log.DebugFormat("Player started a quest '{0}'", Name);
        }
    }
}