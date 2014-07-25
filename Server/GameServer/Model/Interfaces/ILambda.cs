using GameServer.Calculators;

namespace GameServer.Model.Interfaces
{
    public interface ILambda
    {
        float Value { get; }
        float Calculate(GEnvironment env);
    }
}