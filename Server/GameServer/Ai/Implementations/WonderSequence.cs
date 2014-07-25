using System;
using GameServer.Model;
using GameServer.Model.Interfaces;

namespace GameServer.Ai.Implementations
{
    public class WonderSequence : Sequence
    {
        private readonly ICharacter _character;
        private readonly int _offset;
        private readonly Random _random;
        private readonly int _timeIntervals;

        private int _wonderAt;

        public WonderSequence(ICharacter character, Random random, int timeIntervals = 14000, int offset = 4000)
        {
            _character = character;
            _random = random;
            _timeIntervals = timeIntervals;

            if (_timeIntervals < offset)
            {
                 _timeIntervals = offset + 1;
            }

            Add<Condition>().CanRun = IsTimeToWonder;
            Add<Behaviour>().Update = Wonder;

            ResetWonderAt();
        }

        public BhStatus Wonder()
        {
            ResetWonderAt();
            GNpc npc = _character as GNpc;
            
            if (npc != null)
            {
                
                Position oldPosition = _character.Position;
                _character.MoveTo(npc.SpawnManager.GenerateRandomPosition());
            }
            
            return BhStatus.Success;
        }

        public bool IsTimeToWonder()
        {
            return Environment.TickCount > _wonderAt;
        }

        private void ResetWonderAt()
        {
            int newOffset = _random.Next(_offset*2);
            _wonderAt = Environment.TickCount + _timeIntervals - _offset + newOffset;
            Log.Debug(newOffset);
        }

        public override void Reset()
        {
            ResetWonderAt();
            base.Reset();
        }
    }
}