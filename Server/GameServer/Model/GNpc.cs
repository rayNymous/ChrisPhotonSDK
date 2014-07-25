using System;
using System.Collections.Generic;
using GameServer.Ai;
using GameServer.Data.Templates;
using GameServer.Factories;
using GameServer.Model.Interfaces;
using GameServer.Model.Items;
using GameServer.Model.ServerEvents;
using GameServer.Quests;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using MayhemCommon.MessageObjects.Views;
using SubServerCommon;

namespace GameServer.Model
{
    public class GNpc : GCharacter
    {
        private readonly NpcTemplate _template;

        public GSpawn SpawnManager { get; set; }

        public GNpc(IZone zone, NpcTemplate template, IStatHolder stats)
            : base(zone, stats)
        {
            _template = template;
            foreach (var stat in template.Stats)
            {
                Stats.SetStat(stat.Key.GetType(), stat.Value);
            }

            if (template.AiType != null)
            {
                var ai = Activator.CreateInstance(template.AiType) as AiSelector;
                if (ai != null)
                {
                    ai.PrepareAi(this, template.AiSettings);
                    Ai = new AIntelligence(ai);
                }
                else
                {
                    Log.ErrorFormat("Failed to create AI of type {0} for npc {1}", template.AiType, template.Name);
                }
            }
        }

        public NpcTemplate Template
        {
            get { return _template; }
        }

        public AIntelligence Ai { get; set; }

        public override string Name
        {
            get { return _template.Name; }
        }

        public override string Description
        {
            get { return _template.Description; }
        }

        public override float WidthRadius
        {
            get { return Template.WidthRadius; }
        }

        public override ObjectHint GetHint(GPlayerInstance player)
        {
            List<Quest> startingQuests = Template.GetQuests(QuestEventType.Start);
            List<Quest> completingQuests = Template.GetQuests(QuestEventType.Complete);

            var hint = ObjectHint.None;

            if (completingQuests != null)
            {
                foreach (Quest quest in completingQuests)
                {
                    QuestState state = player.GetQuestState(quest);
                    if (state != null && state.Progress == QuestProgressState.TurnIn)
                    {
                        return ObjectHint.QuestReturn;
                    }
                }
            }

            if (startingQuests != null)
            {
                foreach (Quest quest in startingQuests)
                {
                    QuestState state = player.GetQuestState(quest);
                    if (quest.CanStart(player))
                    {
                        return ObjectHint.QuestGive;
                    }
                    if (state != null && state.Progress != QuestProgressState.Completed)
                    {
                        hint = ObjectHint.QuestInProgress;
                    }
                }
            }

            return hint;
        }

        public override ObjectView GetObjectView(GPlayerInstance player)
        {
            return new NpcView
            {
                DataType = typeof (NpcView).ToString(),
                Type = _template.Type,
                InstanceId = InstanceId,
                Name = Name,
                ObjectType = ObjectType.Npc,
                Position = new PositionData(Position.X, Position.Y, Position.Z),
                Prefab = _template.Prefab,
                Hint = GetHint(player)
            };
        }

        public override bool Die(ICharacter killer)
        {
            if (IsDead)
            {
                return false;
            }

            if (Ai != null)
            {
                Ai.Stop();
            }

            var player = killer as GPlayerInstance;

            if (player != null)
            {
                List<Quest> quests = Template.GetQuests(QuestEventType.OnKill);
                if (quests != null)
                {
                    foreach (Quest quest in quests)
                    {
                        String notification = quest.OnKill(player, this);
                        if (notification != null)
                        {
                            player.SendPacket(new EventNotification(notification));
                        }
                    }
                }
            }

            return base.Die(killer);
        }

        public override GContainer GenerateLoot()
        {
            IWorld world = Zone.World;
            GContainer container = world.GetFactory<ContainerFactory>().GetLootContainer(_template.DropList);
            return container;
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            if (ZoneBlock.NearbyPlayersCount > 0)
            {
                Ai.Start();
            }
        }

        public override RelationshipType RelationshipWith(ICharacter character)
        {
            return _template.Type;
        }

        public override bool IsInCombat()
        {
            return AttackableTarget != null;
        }

        public override void CalculateRewards(ICharacter killer)
        {
            var player = killer as GPlayerInstance;

            if (player != null)
            {
                player.AddCoins(_template.CoinsReward);
            }
        }
    }
}