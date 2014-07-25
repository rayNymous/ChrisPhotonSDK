using System;
using System.Collections.Generic;
using ExitGames.Logging;
using GameServer.Model.Interfaces;
using GameServer.Model.ServerEvents;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using MayhemCommon.MessageObjects.Views;

namespace GameServer.Model
{
    public class GObject : IObject
    {
        private readonly List<IObject> _targetedBy;
        protected ILogger Log = LogManager.GetCurrentClassLogger();

        private IObject _target;

        public GObject(IZone zone)
        {
            Zone = zone;
            Prefab = "WObject";
            Position = new Position();
            _targetedBy = new List<IObject>();
        }

        public Guid GlobalId { get; set; }
        public int InstanceId { get; set; }
        public bool IsVisible { get; set; }
        public virtual string Name { get; set; }
        public virtual String Description { get; set; }
        public virtual Position Position { get; set; }
        public GZoneBlock ZoneBlock { get; set; }
        public IZone Zone { get; set; }

        public string Prefab { get; set; }

        public virtual float WidthRadius
        {
            get { return 0.5f; }
        }

        public float HeightRadius
        {
            get { return 0.3f; }
        }

        public ObjectType Type
        {
            get { return ObjectType.Object; }
        }

        public virtual ObjectHint GetHint(GPlayerInstance player)
        {
            return ObjectHint.None;
        }

        public virtual ObjectView GetObjectView(GPlayerInstance player)
        {
            return new ObjectView
            {
                InstanceId = InstanceId,
                Name = Name,
                Prefab = Prefab,
                DataType = typeof (ObjectView).ToString(),
                ObjectType = Type,
                Position = new PositionData(Position.X, Position.Y, Position.Z)
            };
        }


        public virtual void Spawn()
        {
            Log.DebugFormat("Spawned an object");
            IsVisible = true;
            // Region Code

            if (Zone != null)
            {
                Zone.AddObject(this);
            }
            else
            {
                Log.DebugFormat("Object has no zone set to be spawned to/");
                return;
            }

            OnSpawn();
        }

        public virtual void Decay()
        {
            Log.Debug("DECAY");

            IsVisible = false;
            Zone.RemoveObject(this);
            SetTarget(null);

            // Remove target from anyone who is still targeting
            foreach (IObject gObject in _targetedBy)
            {
                SetTarget(null);
            }

            Log.Debug("Removed Object: " + Name);
        }

        public void AddTargetedBy(IObject obj)
        {
            if (!_targetedBy.Contains(obj))
            {
                _targetedBy.Add(obj);
            }
        }

        public void RemoveTargetedBy(IObject obj)
        {
            _targetedBy.Remove(obj);
        }

        public IObject Target
        {
            get { return _target; }
        }

        public virtual void SendPacket(ServerPacket packet)
        {
        }

        public virtual void SendPacket(SystemMessageId id)
        {
        }

        public virtual void SendInfo(IObject obj)
        {
        }

        public virtual void OnSpawn()
        {
        }

        public List<IObject> TargetedBy
        {
            get { return _targetedBy; }
        }

        public List<IObject> VisibleObjects
        {
            get { return ZoneBlock.GetVisibleObjects(); }
        }

        public bool SetTarget(IObject obj)
        {
            if (obj == null && _target != null)
            {
                _target.RemoveTargetedBy(this);
            }

            _target = obj;
            if (obj != null)
            {
                obj.AddTargetedBy(this);
                return true;
            }
            return false;
        }
    }
}