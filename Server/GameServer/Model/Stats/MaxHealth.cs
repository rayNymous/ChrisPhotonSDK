using System.Collections.Generic;
using GameServer.Model.Interfaces;

namespace GameServer.Model.Stats
{
    public class MaxHealth : IDerivedStat
    {
        private readonly List<IFunction> _functions;
        private float _baseMaxHealh = 100;

        public MaxHealth()
        {
            _functions = new List<IFunction>();
        }

        public string Name
        {
            get { return "Max health"; }
        }

        public bool IsBaseStat
        {
            get { return false; }
        }

        public bool IsNonZero
        {
            get { return false; }
        }

        public float BaseValue
        {
            get { return _baseMaxHealh; }
            set { _baseMaxHealh = value; }
        }

        public List<IFunction> Functions
        {
            get { return _functions; }
        }
    }
}