using GameServer.Model.Interfaces;

namespace GameServer.Model.Stats
{
    public class Fame : IStat
    {
        public string Name
        {
            get { return "Fame"; }
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
            get { return 0; }
            set { }
        }
    }
}