using System;
using GameServer.Data;
using GameServer.Model;
using MayhemCommon;
using SubServerCommon;

namespace GameServer.Quests.Implementations
{
    public class GrabThatNastyBlob : Quest
    {
        protected const int SkeletonNpc = 0;
        protected const int BlobNpc = 1;

        protected const string BlobsKilledValue = "blobs";
        protected const int BlobsRequired = 2;
        private int _questId = 0;
        private string _questName = "Get That Nasty Blob";
        private int _requiredQuest = 1;

        public GrabThatNastyBlob(NpcFactory factory) : base(factory)
        {
            StartDialog = new Dialog {Title = "Blob problem", Id = 0, Quest = this};
            StartDialog.Pages = new[]
            {
                new DialogPage
                {
                    Id = 0,
                    IsLeftButtonEnabled = true,
                    LeftButtonText = "Accept",
                    Text =
                        "I'm starting to think that smell is coming from blobs on the other side of the bridge? Could you kill a couple of those, please?",
                    Dialog = StartDialog
                }
            };

            CompleteDialog = new Dialog {Title = "Source of problem", Id = 1, Quest = this};
            CompleteDialog.Pages = new[]
            {
                new DialogPage
                {
                    Id = 0,
                    IsLeftButtonEnabled = true,
                    LeftButtonText = "Complete",
                    Text = "What do you mean it's me? It's proposterous!",
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

            AddEventToNpc(factory, QuestEventType.Start, SkeletonNpc);
            AddEventToNpc(factory, QuestEventType.Complete, SkeletonNpc);

            AddEventToNpc(factory, QuestEventType.OnKill, BlobNpc);
        }

        public override string Name
        {
            get { return _questName; }
        }

        public override int QuestId
        {
            get { return _questId; }
        }

        public override string OnKill(GPlayerInstance killer, GNpc prey)
        {
            QuestState state = killer.GetQuestState(this);
            String notification = null;

            if (state != null && state.Progress != QuestProgressState.TurnIn &&
                state.Progress != QuestProgressState.Completed)
            {
                int ratsKilled = state.GetInt(BlobsKilledValue, 0) + 1;

                if (ratsKilled >= BlobsRequired)
                {
                    notification = String.Format("Blobs killed ({0}/{1}) (Completed)", ratsKilled, BlobsRequired);
                    state.SetProgress(QuestProgressState.TurnIn);
                }
                else
                {
                    notification = String.Format("Blobs killed ({0}/{1})", ratsKilled, BlobsRequired);
                }

                state.SetInt(BlobsKilledValue, ratsKilled);
                return notification;
            }

            return null;
        }

        public override void OnQuestCompleted(GPlayerInstance player)
        {
            player.AddCoins(500);
            player.UnlockZoneStar(player.Zone.ZoneId, StarCode.Third);
        }

        public override bool CanStart(GPlayerInstance player)
        {
            if (!base.CanStart(player))
            {
                return false;
            }

            QuestState requiredState = player.GetQuestState(_requiredQuest);
            if (requiredState != null)
            {
                return requiredState.Progress == QuestProgressState.Completed;
            }
            return false;
        }

        public override string OnQuestStarted(GPlayerInstance player, QuestState state)
        {
            state.SetInt(BlobsKilledValue, 0);
            return base.OnQuestStarted(player, state);
        }

        public void InactivateLast()
        {
        }
    }
}