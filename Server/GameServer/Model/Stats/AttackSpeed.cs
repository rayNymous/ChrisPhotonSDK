using System.Collections.Generic;
using GameServer.Calculators.Functions;
using GameServer.Calculators.Lambdas;
using GameServer.Model.Interfaces;

namespace GameServer.Model.Stats
{
    public class AttackSpeed : IDerivedStat
    {
        private readonly List<IFunction> _functions;

        public AttackSpeed()
        {
            _functions = new List<IFunction>
            {
                new FunctionSubtract(this, 0, null, new LambdaConstant(10)),
                new FunctionMultiply(this, 1, null, new LambdaStat(new Constitution()))
            };
        }

        public string Name
        {
            get { return "AttackSpeed"; }
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
            get { return 1000; }
            set { }
        }

        public List<IFunction> Functions
        {
            get { return _functions; }
        }
    }
}