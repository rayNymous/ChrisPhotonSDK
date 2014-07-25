using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Model.Interfaces;

namespace GameServer.Factories
{
    public class IdFactory : IFactory
    {
        private readonly object _worldLock = new object();

        private Dictionary<int, Guid> _globalIds;
        private Dictionary<Guid, int> _worldIds;

        private int _lastId = 0;

        public Guid GetGlobalId(int worldId)
        {
            return _globalIds[worldId];
        }

        public int GetWorldId(Guid globalId)
        {
            return _worldIds[globalId];
        }

        public int CreateNewWorldId()
        {
            int newId;
            lock (_worldLock)
            {
                newId = ++_lastId;
            }

            return newId;
        }

        public Guid CreateNewGlobalId()
        {
            return Guid.NewGuid();
        }
    }
}
