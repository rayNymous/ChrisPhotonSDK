using GameServer.Data;
using GameServer.Model;
using MayhemCommon;
using SubServerCommon;

namespace GameServer.Quests.Implementations
{
    public class FeelTheBreeze : Quest
    {
        private const int _questId = 1;
        private const string _questName = "Feel the Breeze";

        protected const int SkeletonNpc = 0;

        protected const string AreaName = "shore";
        protected const string VisitedValue = "v";

        public FeelTheBreeze(NpcFactory npcFactory)
            : base(npcFactory)
        {
            StartDialog = new Dialog {Title = "What smell?", Id = 0, Quest = this};
            StartDialog.Pages = new[]
            {
                new DialogPage
                {
                    Id = 0,
                    IsLeftButtonEnabled = true,
                    LeftButtonText = "Accept",
                    Text =
                        "What do you think? The smell of stinky, rotten fish, of course. The worst thing is, I don't even know where it's coming from. Could you walk around and check it out?",
                    Dialog = StartDialog
                }
            };

            CompleteDialog = new Dialog {Title = "Brezze felt", Id = 1, Quest = this};
            CompleteDialog.Pages = new[]
            {
                new DialogPage
                {
                    Id = 0,
                    IsLeftButtonEnabled = true,
                    LeftButtonText = "Complete",
                    Text = "So it must be coming from somewhere?",
                    Dialog = CompleteDialog
                }
            };

            QuestStartLink = new DialogLink {Target = _questId, Text = _questName, Type = DialogLinkType.QuestStart};
            QuestCompleteLink = new DialogLink
            {
                Target = _questId,
                Text = _questName,
                Type = DialogLinkType.QuestComplete
            };

            AddEventToNpc(npcFactory, QuestEventType.Start, SkeletonNpc);
            AddEventToNpc(npcFactory, QuestEventType.Complete, SkeletonNpc);
        }

        public override string Name
        {
            get { return _questName; }
        }

        public override int QuestId
        {
            get { return _questId; }
        }

        public override string OnAreaTrigger(GPlayerInstance player, string areaName)
        {
            QuestState state = player.GetQuestState(this);

            if (state.Progress == QuestProgressState.Started && areaName.Equals(AreaName))
            {
                state.SetProgress(QuestProgressState.TurnIn);
                return "You felt a rush of fresh breeze!";
            }
            return null;
        }

        public override void OnQuestCompleted(GPlayerInstance player)
        {
            player.AddCoins(500);
            player.UnlockZoneStar(player.Zone.ZoneId, StarCode.First);
        }
    }
}