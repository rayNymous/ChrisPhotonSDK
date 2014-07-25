﻿using System;
using System.Collections.Generic;
using GameServer.Model.ServerEvents;
using MayhemCommon;

namespace GameServer.Model.Interfaces
{
    public interface ICharacter : IObject
    {
        //IObject Target { get; set; }
        //int TargetId { get; }

        int Health { get; set; }

        bool IsTeleporting { get; }
        bool IsDead { get; }
        IList<ICharacter> StatusListeners { get; }
        IStatHolder Stats { get; }
        int AttackCooldown { get; }
        ICharacter AttackableTarget { get; }
        void OffsetHealth(int offset, ICharacter attacker, bool notify = true);

        void BroadcastMessage(ServerPacket packet);
        void SendMessage(string text);

        RelationshipType RelationshipWith(ICharacter character);

        // Cooldowns

        // Combat related
        bool Attack(ICharacter target);
        void ReceiveAttack(ICharacter attacker, int damage);
        bool StartAutoAttack();
        void StopAutoAttack();

        // Movement and position related
        void StartFollowing(ICharacter obj, Action onArrive);
        void StopFollowing();
        bool IsMoving();
        bool IsAbleToMove();
        void StopMove(Position pos);
        void MoveTo(Position pos, Action onArrive = null);
        void MoveTo(IObject obj, Action onArrive = null);
        bool UpdateMovement(int tick);
        void Teleport(Position position);
        void Teleport(float x, float y, float z, short heading);
        void Teleport(float x, float y, float z);
        void Teleport(ITeleportType teleportType);

        bool Die(ICharacter killer);
        void CalculateRewards(ICharacter killer);
        void UpdateBroadcastStatus(int broadcastType);
        void Reset();

        // Combat
        bool IsAggressive();
        bool IsInCombat();
        bool IsAttackingDisabled();
    }
}