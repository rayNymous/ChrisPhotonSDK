using System;
using System.Collections.Generic;
using Detour;
using ExitGames.Logging;
using GameServer.Model;

namespace GameServer.Etc
{
    public class PathFinder
    {
        public const int MaxPolys = 256;
        private readonly Position _difference;

        private readonly NavMesh _navMesh;
        private readonly NavMeshQuery _query;
        private readonly QueryFilter _queryFilter;
        private readonly Object pathFinderLock = new Object();
        protected ILogger Log = LogManager.GetCurrentClassLogger();

        //private long endRef;
        //private float[] nearestPt = new float[3];
        //private int polyCount;
        //private long[] polys = new long[MaxPolys];
        //private long startRef;

        private float[] straightPath = new float[MaxPolys];
        private short[] straightPathFlags = new short[20];
        private long[] straightPathRefs = new long[MaxPolys];

        public PathFinder(NavMesh navMesh, NavMeshQuery query, QueryFilter queryFilter, Position difference)
        {
            _navMesh = navMesh;
            _query = query;
            _queryFilter = queryFilter;
            _difference = difference;
        }

        public List<Position> GetPath(Position startPosition, Position endPosition)
        {
            // Transforming Y to Z (// Setting y for path finder)
            float[] startPos = {startPosition.X, _difference.Y, startPosition.Y - _difference.Z};
            float[] endPos = {endPosition.X, _difference.Y, endPosition.Y - _difference.Z};

            lock (pathFinderLock)
            {
                long startRef = 0, endRef = 0;
                long[] polys = new long[MaxPolys];
                int polyCount = 0;
                float[] nearestPt = new float[3];

                Status status = _query.FindNearestPoly(startPos, new[] {2f, 4f, 2f}, _queryFilter, ref startRef, ref nearestPt);
                //Console.WriteLine(string.Format("Found start position status: {0}, Ref {1}, pos {2}, {3}, {4} ", status, startRef, startPos[0], startPos[1], startPos[2]));
                status = _query.FindNearestPoly(endPos, new[] {2f, 4f, 2f}, _queryFilter, ref endRef, ref nearestPt);
                //Console.WriteLine(string.Format("Found end position status: {0}, Ref {1}, pos {2}, {3}, {4} ", status, endRef, endPos[0], endPos[1], endPos[2]));
                status = _query.FindPath(startRef, endRef, startPos, endPos, _queryFilter, ref polys, ref polyCount, MaxPolys);

                int straightPathCount = 0;

                status = _query.FindStraightPath(startPos, endPos, polys, polyCount, ref straightPath, ref straightPathFlags,
                    ref straightPathRefs, ref straightPathCount, 100);

                var path = new List<Position>();

                if (status != Status.Success)
                {
                    path.Add(new Position(endPosition.X, endPosition.Y, endPosition.Z));
                    return path;
                }

                Console.WriteLine("Count: " + straightPathCount);

                for (int i = 0; i < straightPathCount; i++)
                {
                    if (i != 0)
                    {
                        // Ignore the first position, because it's the source

                        // Transforming Z to Y
                        path.Add(new Position(straightPath[i*3] + _difference.X, straightPath[i*3 + 2] + _difference.Z,
                            0));

                        // Original
                        //path.Add(new Position(straightPath[i * 3], straightPath[i * 3 + 1], straightPath[i * 3 + 2]));
                    }
                }

                if (path.Count < 1)
                {
                    path.Add(new Position(endPosition.X, endPosition.Y, endPosition.Z));
                }

                return path;
            }
        }
    }
}