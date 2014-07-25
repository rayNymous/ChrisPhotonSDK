using GameServer.Model;
using GameServer.Model.Interfaces;

namespace GameServer.Calculators.Functions
{
    public class FunctionDivide
    {
        private readonly ILambda _lambda;

        public FunctionDivide(IStat stat, int order, GObject owner, ILambda lambda)
        {
            Stat = stat;
            Order = order;
            Owner = owner;
            _lambda = lambda;
        }

        public IStat Stat { get; private set; }
        public int Order { get; private set; }
        public GObject Owner { get; set; }
        public ICondition Condition { get; set; }

        public float LambdaValue
        {
            get { return _lambda.Value; }
        }

        public string StringFormat
        {
            get { return Stat.Name + ": /{0}"; }
        }

        public void Calc(GEnvironment env)
        {
            if (Condition == null || Condition.Test(env))
            {
                env.Value /= _lambda.Calculate(env);
            }
        }
    }
}