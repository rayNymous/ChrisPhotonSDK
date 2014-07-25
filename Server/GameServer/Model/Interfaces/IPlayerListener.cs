using System;

namespace GameServer.Model.Interfaces
{
    public interface IPlayerListener
    {
        event Action<IPlayer> OnAddPlayer;
        event Action<IPlayer> OnRemovePlayer;
    }
}