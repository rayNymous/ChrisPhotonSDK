using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using MayhemCommon;
using MayhemCommon.MessageObjects.Views;
using UnityEngine;

/// <summary>
/// Represents a basic object in a world
/// </summary>
public class WorldObject : MonoBehaviour
{
    public int InstanceId { get; set; }
    public string Name { get; set; }
    public string Prefab { get; set; }
    public ObjectType ObjectType { get; set; }
    protected Vector3 CurrentPosition;
    private InGameController _gameController;

    public Transform BottomPosition;
    public Transform TopPosition;

    public Hint Hint;

    void Awake()
    {
        BottomPosition = transform.FindChild("BottomPosition");
        TopPosition = transform.FindChild("TopPosition");
        OnAwake();
    }

    void Start()
    {
        _gameController = InGameController.Instance;
        CurrentPosition = new Vector3();
        OnStart();
    }

    void Update()
    {
        OnUpdate();
        UpdateZ();
    }

    public virtual void UpdateZ()
    {
        CurrentPosition.x = transform.position.x;
        CurrentPosition.y = transform.position.y;
        CurrentPosition.z = HelperMethods.CalculateZ(CurrentPosition.y);
        transform.position = CurrentPosition;
    }

    public virtual void InitializeFromView(ObjectView view)
    {
        this.name = view.InstanceId + "";
        Name = view.Name;
        ObjectType = view.ObjectType;
        InstanceId = view.InstanceId;
        transform.position = new Vector2(view.Position.X, view.Position.Y);
        Prefab = view.Prefab;
    }

    void OnClick()
    {
        Dictionary<byte, object> para = new Dictionary<byte, object>();
        para.Add((byte)ClientParameterCode.SubOperationCode, MessageSubCode.TargetRequest);
        para.Add((byte)ClientParameterCode.InstanceId, InstanceId);
        _gameController.SendOperation(
            new OperationRequest() { OperationCode = (byte)ClientOperationCode.Game, Parameters = para }, true, 0, false);
    }

    public virtual void PrepareForDestruction()
    {
        
    }

    public virtual void OnAwake()
    {
    }

    public virtual void OnStart()
    {
    }

    public virtual void OnUpdate()
    {
    }

}
