using System.Collections.Generic;
using GameServer.Data.Templates;
using GameServer.Model;
using SubServerCommon;

namespace GameServer.Quests
{
    public class DialogPage
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DialogLink[] Links { get; set; }
        public Dialog Dialog { get; set; }

        public bool IsLeftButtonEnabled { get; set; }
        public string LeftButtonText { get; set; }

        public List<DialogLink> GetLinks(GPlayerInstance player, bool includeQuests)
        {
            var links = new List<DialogLink>();
            DialogLink[] generalLinks = Links;

            // Adding general links
            if (Links != null)
            {
                links.AddRange(Links);
            }

            if (!includeQuests)
            {
                return links;
            }

            NpcTemplate template = null;
            var npc = player.Target as GNpc;

            if (npc != null)
            {
                template = npc.Template;
            }

            if (template == null || (Dialog != null && Dialog.IsQuestDialog))
            {
                return links;
            }

            // If it's main page, add quest related links
            // TODO find a way to not do this when current dialog is quest dialog
            if (Id == 0)
            {
                List<Quest> startedQuests = template.GetQuests(QuestEventType.Start);
                List<Quest> endedQuests = template.GetQuests(QuestEventType.Start);

                // Add quests to start
                foreach (Quest quest in startedQuests)
                {
                    if (quest.CanStart(player))
                    {
                        links.Add(quest.QuestStartLink);
                    }
                }

                // Add quests to end
                foreach (Quest quest in endedQuests)
                {
                    QuestState state = player.GetQuestState(quest);
                    if (state != null && state.Progress == QuestProgressState.TurnIn)
                    {
                        links.Add(quest.QuestCompleteLink);
                    }
                }
            }

            return links;
        }
    }
}