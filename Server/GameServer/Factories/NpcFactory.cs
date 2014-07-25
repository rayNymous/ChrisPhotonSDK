using System.Collections.Generic;
using ExitGames.Logging;
using GameServer.Ai.Implementations;
using GameServer.Data.Templates;
using GameServer.Model.Interfaces;
using GameServer.Model.Stats;
using MayhemCommon;
using SubServerCommon.Data.NHibernate;

namespace GameServer.Data
{
    public class NpcFactory : IFactory
    {
        protected ILogger Log = LogManager.GetCurrentClassLogger();

        public NpcFactory()
        {
            Templates = new Dictionary<int, NpcTemplate>();
            Spawns = new Dictionary<int, NpcSpawn>();

            LoadTemplates();
        }

        public Dictionary<int, NpcTemplate> Templates { get; set; }
        public Dictionary<int, NpcSpawn> Spawns { get; set; }

        public NpcTemplate GetTemplate(int templateId)
        {
            NpcTemplate template;
            Templates.TryGetValue(templateId, out template);
            return template;
        }

        public void LoadTemplates()
        {
            Templates.Add(0, new NpcTemplate
            {
                Id = 0,
                Name = "Skeleton",
                Prefab = "Skeleton",
                Type = RelationshipType.Friendly,
                RespawnTime = 2000,
                AiType = null,
                DropList = new List<ItemDrop>
                {
                    new ItemDrop(0, 0.5f),
                    new ItemDrop(1, 1),
                    new ItemDrop(2, 1),
                    new ItemDrop(3, 1),
                    new ItemDrop(4, 1),
                },
                Stats = new Dictionary<IStat, float>
                {
                    {new MaxHealth(), 10}
                },
                WidthRadius = 0.5f
            });

            Templates[0].Values.Add("dialog", 0);

            Templates.Add(1, new NpcTemplate
            {
                Id = 1,
                Name = "Blob",
                Prefab = "Blob",
                Type = RelationshipType.Neutral,
                RespawnTime = 2000,
                AiType = typeof (GeneralAi),
                AiSettings = new Dictionary<string, object>(),
                DropList = new List<ItemDrop>
                {
                    new ItemDrop(0, 0.5f),
                    new ItemDrop(1, 1),
                    new ItemDrop(2, 1),
                    new ItemDrop(3, 1),
                    new ItemDrop(4, 1),
                },
                Stats = new Dictionary<IStat, float>
                {
                    {new MaxHealth(), 10}
                },
                WidthRadius = 0.5f,
                CoinsReward = 10
            });

            Templates.Add(2, new NpcTemplate
            {
                Id = 2,
                Name = "Storage",
                Prefab = "Storage",
                Type = RelationshipType.Friendly,
                RespawnTime = 2000,
                AiType = null,
                Stats = new Dictionary<IStat, float>
                {
                    {new MaxHealth(), 10}
                },
                WidthRadius = 2f
            });
        }
    }
}