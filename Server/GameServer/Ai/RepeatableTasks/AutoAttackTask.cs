using System;
using GameServer.Model.Interfaces;
using GameServer.Model.Stats;

namespace GameServer.Ai.RepeatableTasks
{
    public class AutoAttackTask : RepeatableTask
    {
        private const int PrematureAttackDelay = 50;

        private readonly ICharacter _attacker;
        private readonly ICharacter _target;

        public AutoAttackTask(ICharacter attacker, ICharacter target)
            : base(null, TimeSpan.FromMilliseconds(attacker.Stats.GetStat<AttackSpeed>()))
        {
            _attacker = attacker;
            _target = target;
            SetAction(Attack);
        }

        public void Attack()
        {
            // In case we're too early
            if (_attacker.AttackCooldown > Environment.TickCount)
            {
                SetDelay(
                    TimeSpan.FromMilliseconds(_attacker.AttackCooldown - Environment.TickCount + PrematureAttackDelay));
                return;
            }
            // In case attack speed changes
            SetDelay(TimeSpan.FromMilliseconds(_attacker.Stats.GetStat<AttackSpeed>()));
            _attacker.Attack(_target);
        }
    }
}