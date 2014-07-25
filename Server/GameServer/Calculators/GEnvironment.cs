using GameServer.Model.Interfaces;

namespace GameServer.Calculators
{
    public class GEnvironment
    {
        public ICharacter Character { get; set; }
        public ICharacter Target { get; set; }
        public float Value { get; set; }
    }
}