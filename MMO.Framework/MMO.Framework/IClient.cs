using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMO.Framework
{
    public interface IClient : IPeer
    {
        Guid PeerId { get; }
        T ClientData<T>() where T : class, IClientData ;
    }
}
