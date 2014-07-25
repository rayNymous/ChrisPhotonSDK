using System;
using System.Collections.Generic;
using GameServer.Effects;
using GameServer.Model;
using GameServer.Model.Interfaces;
using MayhemCommon;
using NHibernate;
using SubServerCommon;
using SubServerCommon.Data.NHibernate;

namespace GameServer.Data
{
    public class ZoneFactory : IFactory
    {
        public ZoneFactory()
        {
            Templates = new Dictionary<int, ZoneTemplate>();
            ZoneInstances = new Dictionary<Guid, Zone>();

            LoadTemplates(); // TODO Load from a file 
            LoadInstances();
            LoadSpawns();
        }

        public Dictionary<int, ZoneTemplate> Templates { get; set; }
        public Dictionary<Guid, Zone> ZoneInstances { get; set; }

        public void LoadTemplates()
        {
            Templates.Add(0, new ZoneTemplate
            {
                Id = 0,
                Name = "Esterfall Islands",
                SceneName = "EsterfallIsland",
                Type = ZoneType.WorldZone,
                BlockCountX = 1,
                BlockCountY = 1,
                BlockWidth = 100,
                BlockHeight = 100,
                StartPosition = new Position(47, 33, 0, 0),
                ZoneHeight = 100,
                ZoneWidth = 100,
                NavMeshData = "Esterfall.json",
                PathFinderOffset = new Position(0, 29, 27.56f)
            });

            Templates.Add(1, new ZoneTemplate
            {
                Id = 1,
                Name = "The Flatlands of Fraya",
                SceneName = "TheFlatlandsOfFraya",
                Type = ZoneType.WorldZone,
                BlockCountX = 1,
                BlockCountY = 1,
                BlockWidth = 100,
                BlockHeight = 100,
                StartPosition = new Position(14, 30, 0, 0),
                ZoneHeight = 100,
                ZoneWidth = 100,
            });

            Templates.Add(2, new ZoneTemplate
            {
                Id = 2,
                Name = "The Northern Shore",
                SceneName = "TheNorthernShore",
                Type = ZoneType.WorldZone,
                BlockCountX = 1,
                BlockCountY = 1,
                BlockWidth = 100,
                BlockHeight = 100,
                StartPosition = new Position(26, 15, 0, 0),
                ZoneHeight = 100,
                ZoneWidth = 100,
                AoEData = new List<AreaOfEffectData>
                {
                    new AreaOfEffectData
                    {
                        Name = "Cold",
                        StartX = 10,
                        StartY = 37,
                        EndX = 41,
                        EndY = 20,
                        Options = new Dictionary<string, object>
                        {
                            {"repeat", 500}
                        },
                        PlayersOnly = true,
                        Type = AreaOfEffectType.Timer,
                        Effect = c =>
                        {
                            var player = c as GPlayerInstance;
                            if (player != null)
                            {
                                player.OffsetHeat(-20);
                            }
                        }
                    },
                    new AreaOfEffectData
                    {
                        Name = "Warmth",
                        StartX = 18,
                        StartY = 18,
                        EndX = 32,
                        EndY = 13,
                        Options = new Dictionary<string, object>
                        {
                            {"repeat", 500}
                        },
                        PlayersOnly = true,
                        Type = AreaOfEffectType.Timer,
                        Effect = c =>
                        {
                            var player = c as GPlayerInstance;
                            if (player != null)
                            {
                                player.OffsetHeat(10);
                            }
                        }
                    }
                }
            });
        }

        public void LoadInstances()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    IList<MayhemZone> zones = session.QueryOver<MayhemZone>().List();

                    foreach (MayhemZone zone in zones)
                    {
                        var zoneInstance = new Zone
                        {
                            Id = zone.Id,
                            ZoneTemplate = zone.TemplateId
                        };
                        ZoneInstances.Add(zone.Id, zoneInstance);
                    }
                }
            }
        }

        public void LoadSpawns()
        {
            //Templates[0].Spawns.Add(new NpcSpawn
            //{
            //    Id = 0,
            //    Count = 1,
            //    X = 28.3f,
            //    Y = 35.1f,
            //    Width = 0,
            //    Height = 0,
            //    NpcTemplate = 0,
            //});

            // Blob in camp
            //Templates[0].Spawns.Add(new NpcSpawn
            //{
            //    Id = 1,
            //    Count = 1,
            //    X = 27,
            //    Y = 29,
            //    Width = 5,
            //    Height = 5,

            //    //X = 10,
            //    //Y = 13,
            //    //Width = 22,
            //    //Height = 13,
            //    NpcTemplate = 1,
            //});

            // Blobs in island
            //Templates[0].Spawns.Add(new NpcSpawn
            //{
            //    Id = 1,
            //    Count = 10,
            //    X = 85,
            //    Y = 22,
            //    Width = 18,
            //    Height = 10,

            //    //X = 10,
            //    //Y = 13,
            //    //Width = 22,
            //    //Height = 13,
            //    NpcTemplate = 1,
            //});

            // Storage
            //Templates[0].Spawns.Add(new NpcSpawn
            //{
            //    Id = 1,
            //    Count = 1,
            //    X = 32,
            //    Y = 34,
            //    Width = 0,
            //    Height = 0,

            //    //X = 10,
            //    //Y = 13,
            //    //Width = 22,
            //    //Height = 13,
            //    NpcTemplate = 2,
            //});
        }
    }
}