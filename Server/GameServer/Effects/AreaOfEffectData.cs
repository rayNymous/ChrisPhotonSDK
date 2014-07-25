using System;
using System.Collections.Generic;
using GameServer.Model.Interfaces;

namespace GameServer.Effects
{
    public class AreaOfEffectData
    {
        public string Name { get; set; }
        public float StartX { get; set; }
        public float StartY { get; set; }
        public float EndX { get; set; }
        public float EndY { get; set; }
        public Dictionary<string, object> Options { get; set; }
        public AreaOfEffectType Type { get; set; }
        public Boolean PlayersOnly { get; set; }
        public Action<ICharacter> Effect { get; set; }
        public Action<ICharacter> OnEnter { get; set; }
        public Action<ICharacter> OnLeave { get; set; }
    }
}