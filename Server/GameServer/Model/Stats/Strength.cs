using GameServer.Model.Interfaces;

namespace GameServer.Model.Stats
{
    public class Strength : IStat
    {
        public string Name
        {
            get { return "Strength"; }
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
            get { return 2; }
            set { }
        }
    }
}