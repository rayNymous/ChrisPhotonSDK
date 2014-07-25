using GameServer.Model.Interfaces;

namespace GameServer.Model.Stats
{
    public class Intelligence : IStat
    {
        public string Name
        {
            get { return "Intelligence"; }
        }

        public bool IsBaseStat
        {
            get { return true; }
        }

        public bool IsNonZero
        {
            get { return true; }
        }

        public float BaseValue
        {
            get { return 1; }
            set { }
        }
    }
}