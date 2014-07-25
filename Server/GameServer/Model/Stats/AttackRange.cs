using System.Collections.Generic;
using GameServer.Model.Interfaces;

namespace GameServer.Model.Stats
{
    public class AttackRange : IStat
    {
        private readonly List<IFunction> _functions;

        public AttackRange()
        {
            _functions = new List<IFunction>();
        }

        public List<IFunction> Functions
        {
            get { return _functions; }
        }

        public string Name
        {
            get { return "Attack range"; }
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
            get { return 2f; }
            set { }
        }
    }
}