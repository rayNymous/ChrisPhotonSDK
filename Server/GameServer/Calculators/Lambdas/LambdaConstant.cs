using GameServer.Model.Interfaces;

namespace GameServer.Calculators.Lambdas
{
    public class LambdaConstant : ILambda
    {
        private readonly float _value;

        public LambdaConstant(float value)
        {
            _value = value;
        }

        public float Calculate(GEnvironment env)
        {
            return _value;
        }

        public float Value
        {
            get { return _value; }
        }
    }
}