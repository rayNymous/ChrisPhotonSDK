using System.Collections.Generic;
using GameServer.Model.Interfaces;

namespace GameServer.Model.Stats
{
    public class MaxHeat : IDerivedStat
    {
        private readonly List<IFunction> _functions;
        private float _baseMaxHeat = 100;

        public MaxHeat()
        {
            _functions = new List<IFunction>();
        }

        public string Name
        {
            get { return "Max heat"; }
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
            get { return _baseMaxHeat; }
            set { _baseMaxHeat = value; }
        }

        public List<IFunction> Functions
        {
            get { return _functions; }
        }
    }
}