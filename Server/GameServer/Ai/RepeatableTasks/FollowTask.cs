using System;
using GameServer.Model.Interfaces;

namespace GameServer.Ai.RepeatableTasks
{
    public class FollowTask : RepeatableTask
    {
        private readonly ICharacter _character;
        private readonly Action _onArrive;
        private readonly ICharacter _target;

        public FollowTask(ICharacter character, ICharacter target, Action onArrive = null, float followDistance = -1)
            : base(null, TimeSpan.FromSeconds(1))
        {
            FollowDistance = followDistance < 0 ? target.WidthRadius * 1.2f : followDistance;
            
            _character = character;
            _target = target;
            _onArrive = onArrive;
            SetAction(Follow);
        }

        public bool IsFollowing { get; protected set; }

        public float FollowDistance { get; set; }

        public override void OnStart()
        {
            IsFollowing = true;
        }

        public override void OnStop()
        {
            Log.Debug("TEST56: STOPPED FOLLOWING");
            IsFollowing = false;
        }

        public void Follow()
        {
            if (_target.IsDead)
            {
                _character.StopFollowing();
                return;
            }

            float distance = _character.Position.DistanceTo(_target.Position);


            if (distance > FollowDistance)
            {
                _character.MoveTo(_target, null);
            }
            else
            {
                _onArrive();
            }
        }
    }
}