using GameServer.Model.Interfaces;

namespace GameServer.Model.Stats
{
    public class MovementSpeed : IStat
    {
        public string Name
        {
            get { return "Movement Speed"; }
        }

        public bool IsBaseStat
        {
            get { return true; }
        }

        public bool IsNonZero
        {
            get { return false; }
        }

        public float BaseValue
        {
            get { return 6.0f; }
            set { }
        }
    }
}