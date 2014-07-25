using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MMO.Photon.Application;

namespace MMO.Photon.Server
{
    public abstract class DefaultEventHandler : PhotonServerHandler
    {
        public DefaultEventHandler(PhotonApplication application)
            : base(application)
        {
        }
    }
}
