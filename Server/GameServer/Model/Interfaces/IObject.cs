using System;
using System.Collections.Generic;
using GameServer.Model.ServerEvents;
using MayhemCommon;
using MayhemCommon.MessageObjects.Views;

namespace GameServer.Model.Interfaces
{
    public interface IObject
    {
        IObject Target { get; }
        Guid GlobalId { get; set; }
        int InstanceId { get; set; }
        bool IsVisible { get; set; }
        string Name { get; set; }
        String Description { get; set; }
        Position Position { get; set; }
        GZoneBlock ZoneBlock { get; set; }
        IZone Zone { get; set; }
        ObjectType Type { get; }

        string Prefab { get; set; }
        float WidthRadius { get; }
        float HeightRadius { get; }
        List<IObject> TargetedBy { get; }
        List<IObject> VisibleObjects { get; }
        ObjectView GetObjectView(GPlayerInstance player);

        ObjectHint GetHint(GPlayerInstance player);

        void Spawn();
        void Decay();

        void SendPacket(ServerPacket packet);
        void SendPacket(SystemMessageId id);

        void SendInfo(IObject obj);
        void OnSpawn();

        void AddTargetedBy(IObject obj);
        void RemoveTargetedBy(IObject obj);
    }
}