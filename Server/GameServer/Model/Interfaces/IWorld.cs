using System;

namespace GameServer.Model.Interfaces
{
    public interface IWorld
    {
        int GameTick { get; }
        IZone GetZone(Guid id);
        void AddPlayer(IPlayer player);
        void RemovePlayer(IPlayer player);
        T GetFactory<T>() where T : class, IFactory;
    }
}