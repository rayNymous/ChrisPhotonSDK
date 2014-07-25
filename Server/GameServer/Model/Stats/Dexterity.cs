using GameServer.Model.Interfaces;

namespace GameServer.Model.Stats
{
    public class Dexterity : IStat
    {
        public string Name
        {
            get { return "Dexterity"; }
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