using GameServer.Calculators;

namespace GameServer.Model.Interfaces
{
    public interface IFunction
    {
        IStat Stat { get; }
        int Order { get; }
        GObject Owner { get; set; }
        ICondition Condition { get; set; }

        string StringFormat { get; }
        float LambdaValue { get; }

        void Calc(GEnvironment env);
    }
}