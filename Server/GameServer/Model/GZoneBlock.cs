using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Logging;
using GameServer.Model.Interfaces;
using GameServer.Model.ServerEvents;

namespace GameServer.Model
{
    public class GZoneBlock
    {
        private readonly ConcurrentDictionary<int, IObject> _objects;
        private readonly ConcurrentDictionary<int, GPlayerInstance> _players;

        private readonly int _x;
        private readonly int _y;
        protected ILogger Log = LogManager.GetCurrentClassLogger();
        private GZoneBlock[] _connectingBlocks;

        public GZoneBlock(int x, int y)
        {
            NearbyPlayersCount = 0;
            _x = x;
            _y = y;
            _objects = new ConcurrentDictionary<int, IObject>();
            _players = new ConcurrentDictionary<int, GPlayerInstance>();
            _connectingBlocks = new GZoneBlock[0];
        }

        public int NearbyPlayersCount { get; private set; }

        public IEnumerable<GPlayerInstance> Players
        {
            get { return _players.Values; }
        }

        public ICollection<IObject> Objects
        {
            get { return _objects.Values; }
        }

        public int X
        {
            get { return _x; }
        }

        public int Y
        {
            get { return _y; }
        }

        /// <summary>
        ///     Updates a number of players around (iterates through nearby regions).
        /// </summary>
        private void UpdateNearbyPlayersCount()
        {
            int count = 0;
            count += _players.Count;

            foreach (GZoneBlock block in _connectingBlocks)
            {
                count += block.Players.Count();
            }

            if (count == 0 && NearbyPlayersCount > 0)
            {
                OnVisiblePlayersDisappeared();
            }
            else if (count > 0 && NearbyPlayersCount == 0)
            {
                OnFirstVisiblePlayerAppeared();
            }

            NearbyPlayersCount = count;
        }

        /// <summary>
        ///     Calls "onPlayersAroundChange()" for every nearby region
        /// </summary>
        private void NotifyPlayersAroundChange()
        {
            OnPlayersAroundChange();

            foreach (GZoneBlock block in _connectingBlocks)
            {
                block.OnPlayersAroundChange();
            }
        }

        /// <summary>
        ///     Called when all players left a nearby region
        ///     (had to be at least 1 player before)
        /// </summary>
        public void OnVisiblePlayersDisappeared()
        {
            foreach (IObject obj in _objects.Values)
            {
                var npc = obj as GNpc;
                if (npc != null && npc.Ai != null)
                {
                    npc.Ai.Stop();
                }
            }
        }

        public void OnFirstVisiblePlayerAppeared()
        {
            foreach (IObject obj in _objects.Values)
            {
                var npc = obj as GNpc;
                if (npc != null && npc.Ai != null)
                {
                    npc.Ai.Start();
                }
            }
        }

        private void OnPlayersAroundChange()
        {
            UpdateNearbyPlayersCount();
        }

        /// <summary>
        ///     Value returned might include players/characters
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public IObject GetObject(int instanceId)
        {
            IObject obj;
            _objects.TryGetValue(instanceId, out obj);
            return obj;
        }

        /// <summary>
        ///     Just like GetObject, except looks through every nearby region
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public IObject GetNearbyObject(int instanceId)
        {
            IObject obj;
            _objects.TryGetValue(instanceId, out obj);

            if (obj != null)
            {
                return obj;
            }

            foreach (GZoneBlock block in _connectingBlocks)
            {
                obj = block.GetObject(instanceId);
                if (obj != null)
                {
                    return obj;
                }
            }
            return null;
        }

        /// <summary>
        ///     Broadcasts a packet to nearby blocks
        /// </summary>
        /// <param name="packet"></param>
        public void BroadcastAround(ServerPacket packet)
        {
            Broadcast(packet);

            foreach (GZoneBlock block in _connectingBlocks)
            {
                block.Broadcast(packet);
            }
        }

        public void BroadcastToSide(BlockSide side, ServerPacket packet)
        {
            if (side == BlockSide.None)
            {
                return;
            }

            foreach (GZoneBlock block in _connectingBlocks)
            {
                switch (side)
                {
                    case BlockSide.Left:
                        if (X > block.X)
                        {
                            block.Broadcast(packet);
                        }
                        break;
                    case BlockSide.Right:
                        if (X < block.X)
                        {
                            block.Broadcast(packet);
                        }
                        break;
                    case BlockSide.Top:
                        if (Y < block.Y)
                        {
                            block.Broadcast(packet);
                        }
                        break;
                    case BlockSide.Bottom:
                        if (Y > block.Y)
                        {
                            block.Broadcast(packet);
                        }
                        break;
                    case BlockSide.None:
                        break;
                }
            }
        }

        public List<IObject> GetVisibleObjects()
        {
            var visibleObjects = new List<IObject>();

            visibleObjects.AddRange(_objects.Values);

            foreach (GZoneBlock block in _connectingBlocks)
            {
                visibleObjects.AddRange(block.Objects);
            }

            return visibleObjects;
        }

        public List<IObject> GetVisibleObjectsAtSide(BlockSide side)
        {
            if (side == BlockSide.None)
            {
                return null;
            }

            var visibleObjects = new List<IObject>();

            foreach (GZoneBlock block in _connectingBlocks)
            {
                switch (side)
                {
                    case BlockSide.Left:
                        if (X > block.X)
                        {
                            visibleObjects.AddRange(block.Objects);
                        }
                        break;
                    case BlockSide.Right:
                        if (X < block.X)
                        {
                            visibleObjects.AddRange(block.Objects);
                        }
                        break;
                    case BlockSide.Top:
                        if (Y < block.Y)
                        {
                            visibleObjects.AddRange(block.Objects);
                        }
                        break;
                    case BlockSide.Bottom:
                        if (Y > block.Y)
                        {
                            visibleObjects.AddRange(block.Objects);
                        }
                        break;
                    case BlockSide.None:
                        break;
                }
            }

            return visibleObjects;
        }

        public void AddConnectingBlock(GZoneBlock block)
        {
            Array.Resize(ref _connectingBlocks, _connectingBlocks.Length + 1);
            _connectingBlocks[_connectingBlocks.Length - 1] = block;
        }

        /// <summary>
        ///     Sends packet to characters in this block
        /// </summary>
        /// <param name="packet"></param>
        public void Broadcast(ServerPacket packet)
        {
            foreach (var player in _players)
            {
                player.Value.SendPacket(packet);
            }
        }

        public void AddObject(IObject obj)
        {
            _objects.TryAdd(obj.InstanceId, obj);

            // Ckecking if object is a player
            var player = obj as GPlayerInstance;
            if (player != null)
            {
                _players.TryAdd(obj.InstanceId, player);
                NotifyPlayersAroundChange();
            }
            obj.ZoneBlock = this;
        }

        public void RemoveObject(IObject obj)
        {
            _objects.TryRemove(obj.InstanceId, out obj);

            // Ckecking if object is a player
            var player = obj as GPlayerInstance;
            if (player != null)
            {
                _players.TryRemove(obj.InstanceId, out player);
                NotifyPlayersAroundChange();
            }
        }
    }
}