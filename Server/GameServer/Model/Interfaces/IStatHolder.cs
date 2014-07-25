using System;
using System.Collections.Generic;

namespace GameServer.Model.Interfaces
{
    public interface IStatHolder
    {
        ICharacter Character { get; set; }
        Dictionary<Type, IStat> Stats { get; }
        float GetStat<T>() where T : class, IStat;
        float GetStat<T>(T stat) where T : class, IStat;
        void SetStat<T>(float value) where T : class, IStat;
        void SetStat(Type type, float value);
        string SerializeStats();
        void DeserializeStats(string stats);
        void AddModifier(IFunction function);
        void RemoveModifier(IFunction function);
    }
}