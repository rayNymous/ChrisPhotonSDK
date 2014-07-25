using System;
using System.Collections.Generic;
using GameServer.Etc;

namespace GameServer.Model.Interfaces
{
    public interface IZone
    {
        string Name { get; }
        Guid ZoneId { get; }
        IWorld World { get; }
        int GameTick { get; }

        int OnlinePlayersCount { get; }
        PathFinder PathFinder { get; }

        void OnCharacterPositionChange(ICharacter character);

        void AddObject(IObject obj);
        void RemoveObject(IObject obj);

        void AddMovingCharacter(ICharacter character);
        void RemoveMovingCharacter(ICharacter character);
        void OnWorldTick(int tick);

        void EstablishZoneBlock(GCharacter obj, bool teleported);
        Position StartPosition();
        void AddSpawn(ISpawn spawn);

        Position GetNextPositionTowards(Position source, Position target);
        List<Position> GetPath(Position source, Position target);
    }
}