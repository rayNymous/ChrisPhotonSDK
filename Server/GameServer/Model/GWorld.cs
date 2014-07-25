using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using ExitGames.Logging;
using GameServer.Data;
using GameServer.Factories;
using GameServer.Model.Interfaces;
using SubServerCommon.Data.NHibernate;

namespace GameServer.Model
{
    public class GWorld : IWorld
    {
        // Time
        public const int TickLengthMillis = 100;
        public const float DeltaTime = 0.1f;
        protected readonly ILogger Log = LogManager.GetCurrentClassLogger();

        // Instances
        private readonly ConcurrentDictionary<int, IPlayer> _allPlayers = new ConcurrentDictionary<int, IPlayer>();
        private readonly Dictionary<Guid, GZone> _allZones = new Dictionary<Guid, GZone>();
        private readonly Dictionary<Type, IFactory> _factories;
        private int _ticks;
        private Timer tickTimer;

        public GWorld(NpcFactory npcFactory, ItemFactory itemFactory,
            ContainerFactory containerFactory, ZoneFactory zoneFactory, QuestFactory questFactory,
            DialogFactory dialogFactory, IdFactory idFactory)
        {
            _factories = new Dictionary<Type, IFactory>();
            _factories.Add(idFactory.GetType(), idFactory);
            _factories.Add(npcFactory.GetType(), npcFactory);
            _factories.Add(itemFactory.GetType(), itemFactory);
            _factories.Add(containerFactory.GetType(), containerFactory);
            _factories.Add(questFactory.GetType(), questFactory);
            _factories.Add(dialogFactory.GetType(), dialogFactory);

            Log.Debug("**-*-*-**-*-*-**-*- WORLD created");

            // Adding zones
            foreach (Zone zoneInstance in zoneFactory.ZoneInstances.Values)
            {
                ZoneTemplate template = zoneFactory.Templates[zoneInstance.ZoneTemplate];
                var zone = new GZone(this, template, zoneInstance.Id, npcFactory);
                _allZones.Add(zoneInstance.Id, zone);
            }

            tickTimer = new Timer(TickLengthMillis);
            tickTimer.Elapsed += UpdateTick;
            tickTimer.Enabled = true;
        }

        public T GetFactory<T>() where T : class, IFactory
        {
            IFactory factory;
            _factories.TryGetValue(typeof (T), out factory);
            return factory as T;
        }

        public int GameTick
        {
            get { return _ticks; }
        }

        public IZone GetZone(Guid id)
        {
            GZone zone;
            _allZones.TryGetValue(id, out zone);
            // TODO remove fail safe
            if (zone == null)
            {
                return _allZones.Values.First();
            }
            return zone;
        }

        public void AddPlayer(IPlayer player)
        {
            if (player == null)
            {
                Log.Debug("Player cannot be null");
                return;
            }
            _allPlayers.TryAdd(player.InstanceId, player);
        }

        /// <summary>
        ///     Removes a player from the world, zone and a zone block
        /// </summary>
        public void RemovePlayer(IPlayer player)
        {
            if (player != null)
            {
                _allPlayers.TryRemove(player.InstanceId, out player);
            }
        }

        private void UpdateTick(object source, ElapsedEventArgs e)
        {
            _ticks++;
            foreach (GZone zone in _allZones.Values)
            {
                zone.OnWorldTick(_ticks);
            }
        }
    }
}