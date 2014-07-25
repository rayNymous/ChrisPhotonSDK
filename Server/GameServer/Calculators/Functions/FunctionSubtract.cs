using GameServer.Model;
using GameServer.Model.Interfaces;

namespace GameServer.Calculators.Functions
{
    public class FunctionSubtract : IFunction
    {
        private readonly ILambda _lambda;

        public FunctionSubtract(IStat stat, int order, GObject owner, ILambda lambda)
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

        public string StringFormat
        {
            get { return Stat.Name + ": -{0}"; }
        }

        public float LambdaValue
        {
            get { return -_lambda.Value; }
        }

        public void Calc(GEnvironment env)
        {
            if (Condition == null || Condition.Test(env))
            {
                env.Value -= _lambda.Calculate(env);
            }
        }
    }
}