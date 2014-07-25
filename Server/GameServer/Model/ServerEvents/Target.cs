using System;
using GameServer.Model.Interfaces;
using GameServer.Model.Stats;
using MayhemCommon;
using MayhemCommon.MessageObjects;

namespace GameServer.Model.ServerEvents
{
    public class Target : ServerPacket
    {
        public Target(IObject target, String[] actions) : base(MessageSubCode.Target, null)
        {
            var info = new TargetInfo
            {
                CurrentHealth = 1,
                InstanceId = target.InstanceId,
                Level = 1,
                MaxHealth = 1,
                Name = target.Name,
                Actions = actions
            };
            AddSerializedParameter(info, ClientParameterCode.Object);
        }

        public Target(GCharacter target, String[] actions)
            : base(MessageSubCode.Target, null)
        {
            var info = new TargetInfo
            {
                CurrentHealth = target.Health,
                InstanceId = target.InstanceId,
                Level = 2,
                MaxHealth = (int) target.Stats.GetStat<MaxHealth>(),
                Name = target.Name,
                Actions = actions
            };
            AddSerializedParameter(info, ClientParameterCode.Object);
            Log.Debug("SENDING SUCCESSDFASFS");
        }
    }
}