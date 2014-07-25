using GameServer.Model.Interfaces;

namespace GameServer.Model.Stats
{
    public class Level : IStat
    {
        public string Name
        {
            get { return "Level"; }
        }

        public bool IsBaseStat
        {
            get { return true; }
        }

        public bool IsNonZero
        {
            get { return true; }
        }

        public float BaseValue { get; set; }
    }
}