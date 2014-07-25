using GameServer.Model.Interfaces;

namespace GameServer.Model.Stats
{
    public class Constitution : IStat
    {
        public string Name
        {
            get { return "Constitution"; }
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
            get { return 5; }
            set { }
        }
    }
}