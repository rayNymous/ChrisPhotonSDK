using System.Collections.Generic;

namespace GameServer.Model.Interfaces
{
    public interface IDerivedStat : IStat
    {
        List<IFunction> Functions { get; }
    }
}