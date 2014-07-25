using System;
using System.Collections.Generic;
using GameServer.Model;
using GameServer.Model.Interfaces;

namespace GameServer.Ai.Implementations
{
    public class GeneralAi : AiSelector
    {
        private ICharacter _actor;

        private Random _random;

        public override void PrepareAi(ICharacter actor, Dictionary<string, object> settings)
        {
            _actor = actor;
            _random = new Random();

            // Survival
            var survivalSequence = new Sequence();
            survivalSequence.Add<Condition>().CanRun = actor.IsInCombat;
            survivalSequence.Add<Behaviour>().Update = Fight;
            survivalSequence.Add<Condition>().CanRun = IsAboutToFlee;
            Add(survivalSequence);

            // Aggressive
            var aggressiveSequence = new Sequence();
            aggressiveSequence.Add<Condition>().CanRun = actor.IsAggressive;
            aggressiveSequence.Add<Behaviour>().Update = FindPrey;
            aggressiveSequence.Add<Behaviour>().Update = WalkToPrey;
            aggressiveSequence.Add<Behaviour>().Update = Attack;
            Add(aggressiveSequence);

            // Idle
            Add<Condition>().CanRun = actor.IsMoving;
            Add(new WonderSequence(actor, Util.Random, 14000));
        }

        public BhStatus Attack()
        {
            return BhStatus.Success;
        }

        public BhStatus WalkToPrey()
        {
            return BhStatus.Success;
        }

        public BhStatus FindPrey()
        {
            return BhStatus.Success;
        }

        public BhStatus Fight()
        {
            ICharacter target = _actor.AttackableTarget;
            if (target != null)
            {
                _actor.Attack(target);
                return BhStatus.Running;
            }
            return BhStatus.Success;
        }

        public bool IsAboutToFlee()
        {
            return false;
        }

        public BhStatus Flee()
        {
            return BhStatus.Success;
        }
    }
}