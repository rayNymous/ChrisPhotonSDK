using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GameServer.Ai.RepeatableTasks;
using GameServer.Data;
using GameServer.Model.Interfaces;
using GameServer.Model.Items;
using GameServer.Model.ServerEvents;
using GameServer.Model.Stats;
using MayhemCommon;
using MayhemCommon.MessageObjects;

namespace GameServer.Model
{
    public abstract class GCharacter : GObject, ICharacter
    {
        public delegate void DeathListener(ICharacter victim, ICharacter killer);

        protected AutoAttackTask AutoAttackTask;
        public DeathListener DeathListeners;
        protected FollowTask FollowTask;
        protected MovementData Movement;

        private int _attackCooldownUntil;

        public GCharacter(IZone zone, IStatHolder stats) : base(zone)
        {
            Movement = new MovementData();
            Stats = stats;
            stats.Character = this;
            StatusListeners = new List<ICharacter>();
        }

        public GContainer Loot { get; protected set; }

        public ICharacter Killer { get; set; }

        public bool IsMoving
        {
            get { return false; }
        }

        public int Health { get; set; }

        public void OffsetHealth(int offset, ICharacter attacker, bool notify = false)
        {
            Health += offset;

            if (Health > Stats.GetStat<MaxHealth>())
            {
                Health = (int) Stats.GetStat<MaxHealth>();
            }

            if (Health <= 0)
            {
                Health = 0;
                Die(attacker);
            }

            if (notify)
            {
                // Show damage on a character
                BroadcastMessage(new ObjectNotification(InstanceId, ObjectNotificationType.Health, offset + ""));
                var targetStatus = new TargetStatus(this);

                // Send to self
                SendPacket(targetStatus);

                // Update target status
                foreach (IObject obj in TargetedBy)
                {
                    var player = obj as GPlayerInstance;
                    if (player != null)
                    {
                        player.SendPacket(targetStatus);
                    }
                }
            }
        }

        public bool IsTeleporting { get; private set; }
        public bool IsDead { get; private set; }

        public IList<ICharacter> StatusListeners { get; private set; }
        public IStatHolder Stats { get; protected set; }

        public virtual void BroadcastMessage(ServerPacket packet)
        {
            // If is here, because after CleanUp, zone becomes null, but we stilll try to send StopMove packet
            if (ZoneBlock != null)
            {
                ZoneBlock.BroadcastAround(packet);
            }
        }

        public virtual void SendMessage(string text)
        {
        }

        public virtual RelationshipType RelationshipWith(ICharacter character)
        {
            return RelationshipType.Neutral;
        }

        public int AttackCooldown
        {
            get { return _attackCooldownUntil; }
        }

        public bool Attack(ICharacter target)
        {
            if (IsAttackingDisabled())
            {
                Log.Debug("Attacking disabled");
                return false;
            }

            if (target == null || target.IsDead)
            {
                Log.Debug("Target null or dead");
                StopAutoAttack();
                return false;
            }

            if (Position.DistanceTo(target.Position) > Stats.GetStat<AttackRange>())
            {
                return false;
            }

            // Calculate if it's a critical hit
            bool isCritical = false;

            int damage = (int) Stats.GetStat<Strength>(); // Calculate damage

            int attackTime = 500; // How long attack animation takes
            int timeBetweenAttacks = 1000; // Time until the next attack

            _attackCooldownUntil = Environment.TickCount + timeBetweenAttacks + attackTime;

            // Notify to show an animation
            target.BroadcastMessage(new AttackEvent(new AttackData
            {
                AttackerId = InstanceId,
                TargetId = target.InstanceId,
                Damage = damage,
                IsCritical = isCritical
            }));

            // Receive attack
            GameServer.Schedule(() => target.ReceiveAttack(this, damage),
                TimeSpan.FromMilliseconds(attackTime));

            return true;
        }

        public void ReceiveAttack(ICharacter attacker, int damage)
        {
            if (!IsDead)
            {
                OffsetHealth(-damage, attacker, true);

                Log.Debug(Name + "Got damaged by " + attacker.Name + ". Health: " + Health + "/" +
                          Stats.GetStat<MaxHealth>());

                if (AttackableTarget == null && !IsDead)
                {
                    SetTarget(attacker);
                    StartAutoAttack();
                }
            }
        }

        public bool StartAutoAttack()
        {
            // Check if attackable
            ICharacter target = AttackableTarget;
            if (target != null)
            {
                if (AutoAttackTask != null)
                {
                    AutoAttackTask.Stop();
                }

                AutoAttackTask = new AutoAttackTask(this, target);
                AutoAttackTask.Start(true);
                StartFollowing(target, () => AutoAttackTask.Attack());
                FollowTask.FollowDistance = Stats.GetStat<AttackRange>();
                return true;
            }

            return false;
        }

        public void StopAutoAttack()
        {
            if (AutoAttackTask != null)
            {
                AutoAttackTask.Stop();
                AutoAttackTask = null;
            }
        }

        public void StartFollowing(ICharacter obj, Action onArrive = null)
        {
            if (obj == null)
            {
                return;
            }
            if (FollowTask != null)
            {
                FollowTask.Stop();
            }
            FollowTask = new FollowTask(this, obj, onArrive);
            FollowTask.Start(true);
        }

        public void StopFollowing()
        {
            if (FollowTask != null)
            {
                FollowTask.Stop();
                FollowTask = null;
            }
        }

        bool ICharacter.IsMoving()
        {
            return Movement.IsMoving;
        }

        public bool IsAbleToMove()
        {
            return true;
        }

        public virtual bool IsInCombat()
        {
            return false;
        }

        public bool IsAttackingDisabled()
        {
            return IsDead || _attackCooldownUntil > Environment.TickCount;
        }

        public void MoveTo(Position pos, Action onArrive = null)
        {
            Movement.Path = Zone.GetPath(Position, pos);
            Log.Debug("GOT PATH OF SIZE " + Movement.Path.Count + " last element: " + Movement.Path.Last());
            Movement.Speed = 3f;
            Movement.IsMoving = true;
            Movement.OnArriveAction = onArrive;
            Zone.AddMovingCharacter(this);
            BroadcastMessage(new MoveTo(this, new MoveToData
            {
                Destination = Movement.Path[0], // Path should contain at least one element
                Speed = Movement.Speed
            }));
        }

        public void MoveTo(IObject obj, Action onArrive = null)
        {
            float x = obj.Position.X > Position.X
                ? obj.Position.X - obj.WidthRadius
                : obj.Position.X + obj.WidthRadius;
            float y = obj.Position.Y > Position.Y
                ? obj.Position.Y - obj.WidthRadius
                : obj.Position.Y + obj.WidthRadius;

            var targetPosition = new Position(obj.Position.X, obj.Position.Y, obj.Position.Z);
            var angle = (float)Math.Atan2(Position.Y - targetPosition.Y, Position.X - targetPosition.X);

            targetPosition.OffsetByAngle(angle, obj.WidthRadius);

            MoveTo(targetPosition, onArrive);
        }

        /// <summary>
        /// </summary>
        /// <param name="tick"></param>
        /// <returns>True if reached a destination, false otherwise</returns>
        public bool UpdateMovement(int tick)
        {
            if (!Movement.IsMoving)
            {
                return true;
            }

            List<Position> path = Movement.Path;
            Position pathPosition = path.First();

            float dX = pathPosition.X - Position.X;
            float dY = pathPosition.Y - Position.Y;
            var angle = (float) Math.Atan2(dY, dX);

            float distanceCovered = GWorld.DeltaTime*Movement.Speed;

            Position.OffsetByAngle(angle, distanceCovered);

            if (dX*dX + dY*dY <= distanceCovered*distanceCovered)
            {
                Position.XYZ(pathPosition.X, pathPosition.Y, pathPosition.Z);
                path.RemoveAt(0);
                if (path.Count < 1)
                {
                    Movement.IsMoving = false;
                    Movement.OnArriveAction();
                    return true;
                }
                BroadcastMessage(new MoveTo(this, new MoveToData
                {
                    Destination = Movement.Path[0], // Path should contain at least one element
                    Speed = Movement.Speed
                }));
            }
            return false;
        }

        public void Teleport(Position position)
        {
            Teleport(position.X, position.Y, position.Z);
        }

        public void Teleport(float x, float y, float z, short heading)
        {
            StopMove(null);

            IsTeleporting = true;
            //Target = null;

            Decay();
            Position.XYZ(x, y, z);
            Position.Heading = heading;
        }

        public void Teleport(float x, float y, float z)
        {
            Teleport(x, y, z, Position.Heading);
        }

        public void Teleport(ITeleportType teleportType)
        {
            Teleport(teleportType.GetNearestTeleportLocation(this));
        }

        public virtual bool Die(ICharacter killer)
        {
            if (IsDead)
            {
                return false;
            }

            Killer = killer;

            // Stats.OnDeath();
            IsDead = true;

            if (DeathListeners != null)
            {
                DeathListeners(this, killer);
            }

            DeathListeners = null;

            //Target = null;
            // Effect.StopAllEffectsThroughDeath()

            if (killer != null)
            {
                CalculateRewards(killer);
            }

            // Region.OnDeath(this) / Or Game.

            CleanUp();

            Loot = GenerateLoot();

            BroadcastMessage(new CharacterDeathEvent(this));

            foreach (IObject obj in TargetedBy)
            {
                var player = obj as GPlayerInstance;
                if (player != null)
                {
                    player.SendPacket(new Target(this,
                        GameActions.ActionsToStringArray(GameActions.GetActions(player, this))));
                }
            }

            return true;
        }

        public void StopMove(Position pos)
        {
            StopFollowing();
            if (pos != null)
            {
                Position = pos;
            }
            Movement.IsMoving = false;
            Movement.Path = null;
            BroadcastMessage(new StopMove(this));
        }

        public virtual void CalculateRewards(ICharacter killer)
        {
        }

        public virtual void UpdateBroadcastStatus(int broadcastType)
        {
        }

        public void Reset()
        {
            IsDead = false;
            Killer = null;
            Health = (int) Stats.GetStat<MaxHealth>();
            Loot = null;
        }

        public bool IsAggressive()
        {
            return false;
        }

        public ICharacter AttackableTarget
        {
            get { return Target as ICharacter; }
        }

        public abstract GContainer GenerateLoot();

        public virtual bool Restore(Guid characterId)
        {
            return true;
        }

        public virtual void CleanUp()
        {
            // Abort auto attack or casting
            StopAutoAttack();
            SetTarget(null);
            StopMove(null);

            // Stop all timers
            // Stop temporary effects
        }

        public virtual void Deregister()
        {
        }
    }
}