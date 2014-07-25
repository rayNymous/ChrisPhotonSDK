using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Model.Interfaces;
using SubServerCommon.Math;

namespace GameServer.Calculators.Lambdas
{
    public class LambdaRandom : ILambda
    {
        private readonly Random _random;
        private readonly ILambda _max;
        private readonly bool _linear;

        public LambdaRandom(ILambda max, bool linear = true)
        {
            _random = new Random();
            _max = max;
            _linear = linear;
        }

        public float Calculate(GEnvironment env)
        {
            if (_linear)
            {
                return _max.Calculate(env)*(float) _random.NextDouble();
            }
            return _max.Calculate(env)*(float) _random.NextGaussian();
        }
    }
}
