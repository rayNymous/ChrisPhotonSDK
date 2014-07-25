using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Detour;
using ExitGames.Logging;
using GameServer.Data;
using GameServer.Effects;
using GameServer.Etc;
using GameServer.Model.Interfaces;
using GameServer.Model.ServerEvents;
using Newtonsoft.Json;
using SubServerCommon.Data.NHibernate;

namespace GameServer.Model
{
    public class GZone : IZone
    {
        private readonly AreaOfEffectManager AoEManager;
        protected readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<int, IObject> _allObjects;
        private readonly ConcurrentDictionary<int, GPlayerInstance> _allPlayers;

        private readonly ConcurrentDictionary<int, ICharacter> _movingCharacters;

        private readonly ZoneTemplate _template;
        private GZoneBlock[,] _blocks;

        // Path finding related 
        private NavMesh _navMesh;
        private NavMeshQuery _navQuery;
        private QueryFilter _navQueryFilter;
        private PathFinder _pathFinder;

        public GZone(IWorld world, ZoneTemplate template, Guid zoneId, NpcFactory npcFactory)
        {
            World = world;
            _template = template;
            ZoneId = zoneId;

            _allObjects = new ConcurrentDictionary<int, IObject>();
            _allPlayers = new ConcurrentDictionary<int, GPlayerInstance>();
            _movingCharacters = new ConcurrentDictionary<int, ICharacter>();

            InitializeBlocks();

            // AoE effects
            AoEManager = new AreaOfEffectManager(this);
            foreach (AreaOfEffectData aoeData in template.AoEData)
            {
                AoEManager.AddEffect(new AreaOfEffect(aoeData));
            }

            // Npc spawns
            foreach (NpcSpawn npcSpawn in template.Spawns)
            {
                AddSpawn(new GSpawn(npcFactory.Templates[npcSpawn.NpcTemplate], npcSpawn, this));
            }

            //LoadNavMeshData();
        }

        public ZoneTemplate Template
        {
            get { return _template; }
        }

        public IWorld World { get; private set; }

        public int GameTick
        {
            get { return World.GameTick; }
        }

        public int OnlinePlayersCount
        {
            get { return _allPlayers.Count; }
        }

        public string Name
        {
            get { return _template.Name; }
        }

        public Guid ZoneId { get; private set; }

        public void AddMovingCharacter(ICharacter character)
        {
            Log.Debug("Added moving character " + character.Name);
            _movingCharacters.TryAdd(character.InstanceId, character);
        }

        public void RemoveMovingCharacter(ICharacter character)
        {
            ICharacter tempCharacter;
            _movingCharacters.TryRemove(character.InstanceId, out tempCharacter);
        }

        public void OnWorldTick(int tick)
        {
            foreach (var characterEntry in _movingCharacters)
            {
                OnCharacterPositionChange(characterEntry.Value);
                if (characterEntry.Value.UpdateMovement(tick))
                {
                    ICharacter character;
                    _movingCharacters.TryRemove(characterEntry.Key, out character);
                }
            }
        }

        public void OnCharacterPositionChange(ICharacter character)
        {
            AoEManager.NotifyPositionChange(character);
        }

        /// <summary>
        ///     Adds an object to the zone and to one of it's blocks.
        /// </summary>
        /// <param name="obj"></param>
        public void AddObject(IObject obj)
        {
            _allObjects.TryAdd(obj.InstanceId, obj);

            // Ckecking if object is a player
            var player = obj as GPlayerInstance;
            if (player != null)
            {
                if (_allPlayers.Count == 0)
                {
                    OnFirstPlayerAppear();
                }
                _allPlayers.TryAdd(obj.InstanceId, player);

                // TODO Retrieve player position, if null, set to default of the zone
                if (player.Position == null)
                {
                    player.Position = new Position(_template.StartPosition.X, _template.StartPosition.Y,
                        _template.StartPosition.Z);
                    Log.Debug("Placed to default zone position " + player.Position);
                }
            }

            var character = obj as GCharacter;
            if (character != null)
            {
                AoEManager.NotifyPositionChange(character);
            }

            obj.Zone = this;

            // Adding object to one of the regions.
            _blocks[GetBlockIndexX(obj.Position.X), GetBlockIndexY(obj.Position.Y)].AddObject(obj);

            obj.ZoneBlock.BroadcastAround(new ShowObjects(new List<IObject> {obj}));
        }

        public void RemoveObject(IObject obj)
        {
            Log.Debug("GZONE REMOVE OBJECT");

            _allObjects.TryRemove(obj.InstanceId, out obj);

            if (obj == null)
            {
                Log.Debug("GZONE MYSTERIOUS ERROR");
                return;
            }

            // Checking if object is a player
            var player = obj as GPlayerInstance;
            if (player != null)
            {
                Log.Debug("PLAYER REMOOOOOOOOOOOVEDDSDSDS SDS DS");
                _allPlayers.TryRemove(obj.InstanceId, out player);
                if (_allPlayers.Count == 0)
                {
                    OnAllPlayersDisappear();
                }
            }

            var character = obj as GCharacter;
            if (character != null)
            {
                AoEManager.RemoveCharacter(character);
            }

            obj.ZoneBlock.BroadcastAround(new DeleteObjects(new List<IObject> {obj}));
            obj.ZoneBlock.RemoveObject(obj);
        }

        public List<Position> GetPath(Position source, Position target)
        {
            List<Position> path = PathFinder.GetPath(source, target) ?? new List<Position>();

            return path;
        }

        public PathFinder PathFinder
        {
            get { return _pathFinder; }
        }

        public Position GetNextPositionTowards(Position source, Position target)
        {
            if (PathFinder != null)
            {
                List<Position> path = PathFinder.GetPath(source, target);
                if (path != null && path.Count > 0)
                {
                    return path.First();
                }
            }
            return target;
        }

        /// <summary>
        ///     Checks whether character has moved from a block and got to another
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="teleported"></param>
        public void EstablishZoneBlock(GCharacter character, bool teleported)
        {
            int blockX = GetBlockIndexX(character.Position.X);
            int blockY = GetBlockIndexY(character.Position.Y);

            if (blockX >= _template.BlockCountX || blockX < 0 || blockY >= _template.BlockCountY || blockY < 0)
            {
                return;
            }

            GZoneBlock oldBlock = character.ZoneBlock;
            bool firstAppearance = oldBlock == null;

            if (_blocks[blockY, blockX] != oldBlock)
            {
                _blocks[blockY, blockX].AddObject(character);
                var characterInList = new List<IObject> {character};

                if (!firstAppearance)
                {
                    // Remove from old region
                    oldBlock.RemoveObject(character);

                    // Notify players at farthest side, indicating that this character left visible area
                    oldBlock.BroadcastToSide(GetBlockSideByOffsetX(oldBlock.X, blockX, true),
                        new DeleteObjects(characterInList));
                    oldBlock.BroadcastToSide(GetBlockSideByOffsetY(oldBlock.Y, blockY, true),
                        new DeleteObjects(characterInList));

                    // Notify players at newly visible side, indicating that a new character is now visible
                    oldBlock.BroadcastToSide(GetBlockSideByOffsetX(oldBlock.X, blockX, false),
                        new DeleteObjects(characterInList));
                    oldBlock.BroadcastToSide(GetBlockSideByOffsetY(oldBlock.Y, blockY, false),
                        new DeleteObjects(characterInList));

                    // If character is moving, send movement data to blocks
                    if (character.IsMoving)
                    {
                    }

                    // Send player data about newly visible objects in a newly visible side
                    character.SendPacket(new ShowObjects(_blocks[blockY, blockX].GetVisibleObjectsAtSide(
                        GetBlockSideByOffsetX(oldBlock.X, blockX, true))));
                    character.SendPacket(new ShowObjects(_blocks[blockY, blockX].GetVisibleObjectsAtSide(
                        GetBlockSideByOffsetY(oldBlock.Y, blockY, true))));

                    // Send player data about characters that are no longer visible
                    character.SendPacket(new DeleteObjects(oldBlock.GetVisibleObjectsAtSide(
                        GetBlockSideByOffsetX(oldBlock.X, blockX, true))));
                    character.SendPacket(new DeleteObjects(oldBlock.GetVisibleObjectsAtSide(
                        GetBlockSideByOffsetY(oldBlock.Y, blockY, true))));
                }
                else
                {
                    // Send player data about newly visible objects in a newly visible side
                    character.SendPacket(new ShowObjects(character.ZoneBlock.GetVisibleObjects()));

                    // Send nearby blocks data about newly visible object
                    _blocks[blockY, blockX].BroadcastAround(new ShowObjects(characterInList));
                }
            }
        }

        public Position StartPosition()
        {
            return _template.StartPosition;
        }

        public void AddSpawn(ISpawn spawn)
        {
            spawn.Start();
        }

        public void LoadNavMeshData()
        {
            if (_template.NavMeshData != null && _template.PathFinderOffset != null)
            {
                string data = Helpers.ReadFile("../MayhemAndHell/Data/NavigationMeshes/Esterfall.json");

                _navMesh = JsonConvert.DeserializeObject<NavMeshSerializer>(data).Reconstitute();

                _navQuery = new NavMeshQuery();
                _navQuery.Init(_navMesh, 1024);

                _navQueryFilter = new QueryFilter();
                _navQueryFilter.IncludeFlags = 15;
                _navQueryFilter.ExcludeFlags = 0;
                _navQueryFilter.SetAreaCost(1, 1.0f);
                _navQueryFilter.SetAreaCost(2, 10.0f);
                _navQueryFilter.SetAreaCost(3, 1.0f);
                _navQueryFilter.SetAreaCost(4, 1.0f);
                _navQueryFilter.SetAreaCost(5, 2);
                _navQueryFilter.SetAreaCost(6, 1.5f);

                _pathFinder = new PathFinder(_navMesh, _navQuery, _navQueryFilter, _template.PathFinderOffset);

                Log.Debug("Found navmesh data for zone " + _template.Name);
            }
            else
            {
                Log.Error("Navmesh data for zone " + _template.Name + " not found! " + (_template.NavMeshData != null) +
                          " " + (_template.PathFinderOffset != null));
            }
        }

        public void OnAllPlayersDisappear()
        {
            AoEManager.Stop();
        }

        public void OnFirstPlayerAppear()
        {
            AoEManager.Start();
        }

        public int GetBlockIndexX(float zoneX)
        {
            return (int) (zoneX/_template.BlockWidth);
        }

        public int GetBlockIndexY(float zoneY)
        {
            return (int) (zoneY/_template.BlockHeight);
        }

        /// <summary>
        ///     Returns a side to which blocks have changed (newly visible side)
        /// </summary>
        /// <param name="oldX"></param>
        /// <param name="newX"></param>
        /// <param name="flip">Flips directions</param>
        /// <returns>Returns newly visible side, or invisible if flip is set to true.</returns>
        private static BlockSide GetBlockSideByOffsetX(int oldX, int newX, bool flip)
        {
            if (oldX == newX)
            {
                return BlockSide.None;
            }

            return (oldX < newX) ^ flip ? BlockSide.Right : BlockSide.Left;
        }

        /// <summary>
        ///     Returns a side to which regions have changed (newly visible side)
        /// </summary>
        /// <param name="oldY"></param>
        /// <param name="newY"></param>
        /// <param name="flip">Flips directions</param>
        /// <returns>Return newly visible side, or invisible if flip is set to true.</returns>
        private static BlockSide GetBlockSideByOffsetY(int oldY, int newY, bool flip)
        {
            if (oldY == newY)
            {
                return BlockSide.None;
            }

            return (oldY < newY) ^ flip ? BlockSide.Top : BlockSide.Bottom;
        }

        /// <summary>
        ///     Creates zone blocks and fills them with information about
        ///     nearby blocks
        /// </summary>
        private void InitializeBlocks()
        {
            int blocksX = (int) (_template.ZoneWidth/_template.BlockWidth) + 1;
            int blocksY = (int) (_template.ZoneHeight/_template.BlockHeight) + 1;

            _blocks = new GZoneBlock[blocksY, blocksX];

            for (int y = 0; y < blocksY; y++)
            {
                for (int x = 0; x < blocksX; x++)
                {
                    _blocks[y, x] = new GZoneBlock(x, y);
                }
            }

            // Connecting nearby blocks
            for (int y = 0; y < blocksY; y++)
            {
                for (int x = 0; x < blocksX; x++)
                {
                    bool right = false;
                    bool left = false;
                    bool bottom = false;
                    bool top = false;

                    // If has a neigbour at right
                    if (x < blocksX - 1)
                    {
                        right = true;
                        _blocks[y, x].AddConnectingBlock(_blocks[y, x + 1]);
                    }

                    // If has a neighbour at left
                    if (x > 0)
                    {
                        left = true;
                        _blocks[y, x].AddConnectingBlock(_blocks[y, x - 1]);
                    }

                    // If has a neigbour at bottom
                    if (y >= 1)
                    {
                        bottom = true;
                        _blocks[y, x].AddConnectingBlock(_blocks[y - 1, x]);
                    }

                    // If has a neigbour at top 
                    if (y < blocksY - 1)
                    {
                        top = true;
                        _blocks[y, x].AddConnectingBlock(_blocks[y + 1, x]);
                    }

                    if (top && left)
                    {
                        _blocks[y, x].AddConnectingBlock(_blocks[y + 1, x - 1]);
                    }

                    if (top && right)
                    {
                        _blocks[y, x].AddConnectingBlock(_blocks[y + 1, x + 1]);
                    }

                    if (bottom && left)
                    {
                        _blocks[y, x].AddConnectingBlock(_blocks[y - 1, x - 1]);
                    }

                    if (bottom && right)
                    {
                        _blocks[y, x].AddConnectingBlock(_blocks[y - 1, x + 1]);
                    }
                }
            }
        }
    }
}