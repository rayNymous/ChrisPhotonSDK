using System.Collections.Generic;
using GameServer.Model;
using GameServer.Model.Interfaces;

namespace GameServer.Ai
{
    public abstract class AiSelector : PrioritySelector
    {
        public abstract void PrepareAi(ICharacter actor, Dictionary<string, object> settings);
    }
}