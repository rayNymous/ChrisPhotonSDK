using System;
using System.Threading.Tasks;
using ExitGames.Logging;
using GameServer.Data.Templates;
using GameServer.Factories;
using GameServer.Model.Interfaces;
using GameServer.Model.Stats;
using SubServerCommon.Data.NHibernate;

namespace GameServer.Model
{
    public class GSpawn : ISpawn
    {
        private const int CorpseDisplayTime = 1000*30;
        private readonly NpcSpawn _data;
        private readonly Random _random;
        private readonly NpcTemplate _template;

        private readonly IdFactory _idFactory;

        protected ILogger Log = LogManager.GetCurrentClassLogger();

        protected GZone _zone;

        public GSpawn(NpcTemplate npcTemplate, NpcSpawn data, GZone zone)
        {
            _template = npcTemplate;
            _data = data;
            _random = new Random();
            _zone = zone;
            _idFactory = zone.World.GetFactory<IdFactory>();
        }

        /// <summary>
        ///     Creates new instances
        /// </summary>
        public void Start()
        {
            for (int i = 0; i < _data.Count; i++)
            {
                var npc = new GNpc(_zone, _template, new StatHolder(StatHolder.CreateStatsList()));
                npc.SpawnManager = this;
                Respawn(npc);
            }
        }

        public void OnCharacterDeath(ICharacter victim, ICharacter killer)
        {
            var npc = victim as GNpc;

            if (npc != null)
            {
                ScheduleRespawn(npc);
            }
        }

        public Position GenerateRandomPosition()
        {
            return new Position((float) (_data.X + _random.NextDouble()*_data.Width),
                (float) (_data.Y + _random.NextDouble()*_data.Height), 0);
        }

        private async Task ScheduleRespawn(GNpc npc)
        {
            await Task.Delay(CorpseDisplayTime);
            npc.Decay();
            await Task.Delay(_template.RespawnTime);
            Respawn(npc);
        }

        private void Respawn(GNpc npc)
        {
            npc.Position = GenerateRandomPosition();
            npc.InstanceId = _idFactory.CreateNewWorldId();
            Log.Debug("GOT THIS ID: " + npc.InstanceId + " npc " + npc.Name);
            npc.DeathListeners += OnCharacterDeath;
            npc.Reset();
            npc.Spawn();
        }
    }
}