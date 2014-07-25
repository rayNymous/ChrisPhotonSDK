using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Detour;
using ExitGames.Concurrency.Channels;
using GameServer.Ai;
using GameServer.Ai.Implementations;
using GameServer.Data;
using GameServer.Etc;
using GameServer.Factories;
using GameServer.Model;
using GameServer.Model.Interfaces;
using GameServer.Model.Items;
using GameServer.Model.Stats;
using GameServer.Quests;
using GameServer.Quests.Implementations;
using MayhemCommon;
using MayhemCommon.MessageObjects.Views;
using Newtonsoft.Json;
using ProtoBuf;
using Quartz;
using Quartz.Impl;
using SubServerCommon;
using SubServerCommon.Data.NHibernate;
using Zone = System.Security.Policy.Zone;

namespace Tests
{
    class Program
    {
        public static long startRef = 0, endRef = 0;
        public const int MaxPolys = 256;
        public static long[] polys = new long[MaxPolys];
        public static int polyCount = 0;
        public static float[] nearestPt = new float[3];
        public static float[] startPos;
        public static float[] endPos;

        public class CustomJob : IJob
        {
            private string _textas;

            public CustomJob(String textas)
            {
                _textas = textas;
            }

            public void Execute(IJobExecutionContext context)
            {
                Console.WriteLine(_textas);
            }
        }

        private static int _skaicius = 0;

        public static List<Position> GetPositions()
        {
          Console.WriteLine("SITIEK");  
          return new List<Position>();
        }

        public static int GetStarsCount(int starsInt)
        {
            int stars = 0;

            if (starsInt == 1 || starsInt == 2 || starsInt == 4)
            {
                return 1;
            }
            else if (starsInt == 3 || starsInt == 5 || starsInt == 6)
            {
                return 2;
            } else if (starsInt == 7)
            {
                return 3;
            }

            return stars;
        }

        static void Main(string[] args)
        {
            int stars = 0;

            stars |= (byte)StarCode.First | (byte) StarCode.Second | (byte) StarCode.Third;

            Console.WriteLine(stars);

            stars = GetStarsCount(stars);

            Console.WriteLine(stars);

            //// PATH FINDER

            //String data = ReadFile("../../../Data/NavigationMeshes/Esterfall.json");
            //NavMesh mesh = JsonConvert.DeserializeObject<NavMeshSerializer>(data).Reconstitute();

            //// Initialize query
            //var navQuery = new NavMeshQuery();
            //navQuery.Init(mesh, 1024);

            //var filter = new QueryFilter();
            //filter.IncludeFlags = 15;
            //filter.ExcludeFlags = 0;
            //filter.SetAreaCost(1, 1.0f);
            //filter.SetAreaCost(2, 10.0f);
            //filter.SetAreaCost(3, 1.0f);
            //filter.SetAreaCost(4, 1.0f);
            //filter.SetAreaCost(5, 2);
            //filter.SetAreaCost(6, 1.5f);

            ////var startPos = new float[] { 13, 26, -3 }; //  var startPos = new float[] {13, 26, -3};
            ////var endPos = new float[] {30, 26, -9};

            //var pathFinder = new PathFinder(mesh, navQuery, filter, new Position(0, 29, 27.56f));

            //var path = pathFinder.GetPath(new Position(30, 28.4f, -4), new Position(23, 32, -4));

            //foreach (var position in path)
            //{
            //    Console.WriteLine(position);
            //}

            //Console.WriteLine("---------");

            //Status status = navQuery.FindNearestPoly(startPos, new[] { 2f, 4f, 2f }, filter, ref startRef, ref nearestPt);
            //Console.WriteLine(string.Format("Found start position status: {0}, Ref {1}, pos {2}, {3}, {4} ", status, startRef, startPos[0], startPos[1], startPos[2]));
            //status = navQuery.FindNearestPoly(endPos, new[] { 2f, 4f, 2f }, filter, ref endRef, ref nearestPt);
            //Console.WriteLine(string.Format("Found end position status: {0}, Ref {1}, pos {2}, {3}, {4} ", status, endRef, endPos[0], endPos[1], endPos[2]));
            //status = navQuery.FindPath(startRef, endRef, startPos, endPos, filter, ref polys, ref polyCount, MaxPolys);

            //float[] straightPath = new float[MaxPolys];
            //short[] straightPathFlags = new short[20];
            //long[] straightPathRefs = new long[MaxPolys];
            //int straightPathCount = 0;

            //status = navQuery.FindStraightPath(startPos, endPos, polys, polyCount, ref straightPath, ref straightPathFlags,
            //    ref straightPathRefs, ref straightPathCount, 100);

            //Console.WriteLine("DOES IT: " + status);

            //for (int i = 0; i < straightPathCount; i++)
            //{
            //    Console.WriteLine(straightPath[i * 3] + " " + straightPath[i * 3 + 1] + " " + straightPath[i * 3 + 2]);
            //}

            //Console.WriteLine(Guid.NewGuid());

            //var itemFactory = new ItemFactory();

            //var player = new GPlayerInstance(null, new StatHolder(StatHolder.CreateStatsList()));

            //var item = itemFactory.GetItemFromTemplate(3);

            //Console.WriteLine(player.Stats.GetStat<Strength>());

            //player.Equipment.EquipItem(item);
            //Console.WriteLine(player.Stats.GetStat<Strength>());

            //var selector = new PrioritySelector();

            //// Survival
            //var survival = new Sequence();
            //survival.Add<Condition>().CanRun = () => true;
            //survival.Add<Behaviour>().Update = () =>
            //{
            //    Console.WriteLine("Update survival");
            //    return BhStatus.Running;
            //};
            //survival.Add<Condition>().CanRun = () => false;

            //// Aggro
            //var aggro = new Sequence();
            //aggro.Add<Condition>().CanRun = () =>
            //{
            //    Console.WriteLine("Can run");
            //    return false;
            //};

            //// Idle
            //var idle = new Sequence();
            //idle.Add<Condition>().CanRun = () => true;
            //idle.Add<Behaviour>().Update = () =>
            //{
            //    Console.WriteLine("WALKING");
            //    return BhStatus.Success;
            //};

            //selector.Add(survival);
            //selector.Add(aggro);
            //selector.Add(idle);

            //selector.Update();
            //selector.Update();
            //selector.Update();
        }

        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string ReadFile(string path)
        {
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(System.IO.Path.GetFullPath(path))) 
            {
                String line;
                while ((line = sr.ReadLine()) != null) 
                {
                    sb.AppendLine(line);
                }
            }
            string allines = sb.ToString();

            return allines;
        }

        public static async void Schedule(Action action, TimeSpan delay)
        {
            await Task.Delay(delay);
            action();
            Schedule(action, delay);
        }
        
        public static async void Darbas()
        {
            _skaicius++;
        }

       
    }
}
