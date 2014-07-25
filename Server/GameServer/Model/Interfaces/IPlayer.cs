using System;
using MMO.Photon.Client;
using MMO.Photon.Server;

namespace GameServer.Model.Interfaces
{
    public interface IPlayer : ICharacter
    {
        SubServerClientPeer Client { get; set; }
        PhotonServerPeer ServerPeer { get; set; }
        Guid UserId { get; set; }
        Guid CharacterId { get; set; }
    }
}