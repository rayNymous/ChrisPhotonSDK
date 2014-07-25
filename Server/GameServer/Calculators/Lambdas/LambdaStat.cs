using GameServer.Model.Interfaces;

namespace GameServer.Calculators.Lambdas
{
    public class LambdaStat : ILambda
    {
        private readonly IStat _stat;
        private readonly bool _useTarget;

        public LambdaStat(IStat stat, bool useTarget = false)
        {
            _stat = stat;
            _useTarget = useTarget;
        }

        public float Calculate(GEnvironment env)
        {
            if (_useTarget && env.Target == null)
            {
                return 1;
            }
            if (!_useTarget && env.Character == null)
            {
                return 1;
            }
            if (_useTarget)
            {
                return env.Target.Stats.GetStat(_stat);
            }

            return env.Character.Stats.GetStat(_stat);
        }

        public float Value
        {
            get { return 1; }
        }
    }
}