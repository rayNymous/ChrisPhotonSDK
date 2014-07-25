using MMO.Framework;
using MMO.Photon.Server;

namespace SubServerCommon.Data.ClientData
{
    public class ServerData : IClientData
    {
        public PhotonServerPeer ServerPeer { get; set; }
    }
}