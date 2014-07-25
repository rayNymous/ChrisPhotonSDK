using GameServer.Calculators;

namespace GameServer.Model.Interfaces
{
    public interface ICondition
    {
        bool Test(GEnvironment env);
        void NotifyChanged();
    }
}