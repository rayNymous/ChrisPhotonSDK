using System.Collections.Generic;
using GameServer.Effects;
using GameServer.Model;
using MayhemCommon;
using SubServerCommon.Data.NHibernate;

namespace GameServer.Data
{
    public class ZoneTemplate
    {
        public List<AreaOfEffectData> AoEData;

        public ZoneTemplate()
        {
            Spawns = new List<NpcSpawn>();
            AoEData = new List<AreaOfEffectData>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string SceneName { get; set; }
        public ZoneType Type { get; set; }
        public Position StartPosition { get; set; }
        public float ZoneHeight { get; set; }
        public float ZoneWidth { get; set; }
        public float BlockWidth { get; set; }
        public float BlockHeight { get; set; }
        public int BlockCountX { get; set; }
        public int BlockCountY { get; set; }
        public List<NpcSpawn> Spawns { get; set; }
        public string NavMeshData { get; set; }
        public Position PathFinderOffset { get; set; }
    }
}