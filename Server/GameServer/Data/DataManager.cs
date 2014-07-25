using System.Collections.Generic;
using GameServer.Data.Templates;
using GameServer.Model;

namespace GameServer.Data
{
    public class DataManager
    {
        public DataManager()
        {
            NpcTable = new Dictionary<int, NpcTemplate>();
            ItemTable = new Dictionary<int, ItemTemplate>();
        }

        public Dictionary<int, NpcTemplate> NpcTable { get; set; }
        public Dictionary<int, ItemTemplate> ItemTable { get; set; }
        public Dictionary<int, GSpawn> SpawnTable { get; set; }
    }
}