
using System;
using System.Collections.Generic;
using Assets.MayhemAndHell.Scripts.InGame.Handlers;
using ExitGames.Client.Photon;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using MayhemCommon.MessageObjects.Views;
using UnityEngine;

public partial class InGameController : ViewController
{
    private CurrentPlayer _currentPlayer;
    private EntityManager _entityManager;

    private static InGameController _instance;
    public static InGameController Instance {get { return _instance; }}

    public InGame InGameView {get { return ControllerView as InGame; }}

    public CurrentPlayer Player { get { return _currentPlayer; }}

    public InGameController(View controlledView, byte subOperationCode = 0) : base(controlledView, subOperationCode)
    {
        _entityManager = GameObject.FindObjectOfType<EntityManager>();
        _instance = this;
        EventSubCodeHandlers.Add((int)MessageSubCode.PlayerInit, new PlayerInitHandler(this));
        EventSubCodeHandlers.Add((int)MessageSubCode.MoveTo, new MoveToHandler(this));
        EventSubCodeHandlers.Add((int)MessageSubCode.ShowObjects, new ShowObjectsHandler(this));
        EventSubCodeHandlers.Add((int)MessageSubCode.DeleteObjects, new DeleteObjectsHandler(this));
        EventSubCodeHandlers.Add((int)MessageSubCode.Target, new TargetHandler(this));
        EventSubCodeHandlers.Add((int)MessageSubCode.CharacterDeath, new CharacterDeathHandler(this));
        EventSubCodeHandlers.Add((int)MessageSubCode.DialogPage, new DialogPageHandler(this));
        EventSubCodeHandlers.Add((int)MessageSubCode.EventNotification, new EventNotificationHandler(this));
        EventSubCodeHandlers.Add((int)MessageSubCode.TargetStatus, new TargetStatusHandler(this));
        EventSubCodeHandlers.Add((int)MessageSubCode.ObjectNotification, new ObjectNotificationHandler(this));
        EventSubCodeHandlers.Add((int)MessageSubCode.LootContainer, new LootContainerHandler(this));
        EventSubCodeHandlers.Add((int)MessageSubCode.LootContainerItemRemove, new LootContainerItemRemoveHandler(this));
        EventSubCodeHandlers.Add((int)MessageSubCode.ContainerSetItem, new ContainerSetItemHandler(this));
        EventSubCodeHandlers.Add((int)MessageSubCode.Attack, new AttackEventHandler(this));
        EventSubCodeHandlers.Add((int)MessageSubCode.StopMove, new StopMoveHandler(this));
        EventSubCodeHandlers.Add((int)MessageSubCode.ObjectHint, new ObjectHintUpdate(this));
        EventSubCodeHandlers.Add((int)MessageSubCode.PlayerStatus, new PlayerStatusHandler(this));
        EventSubCodeHandlers.Add((int)MessageSubCode.LeaveZone, new LeaveZoneHandler(this));
        EventSubCodeHandlers.Add((int)MessageSubCode.DeathNotification, new DeathNotificationHandler(this));
        EventSubCodeHandlers.Add((int)MessageSubCode.GlobalStorageData, new GlobalStorageInfoHandler(this));
        EventSubCodeHandlers.Add((int)MessageSubCode.Chat, new ChatHandler(this));

        InitializeChat();
    }

    public void InstantiatePlayer(PlayerInitData initData)
    {
        var playerPrefab = Resources.Load("Prefabs/Player");
        var position = initData.Position;
        var playerObject = (GameObject)View.Instantiate(playerPrefab, new Vector3(position.X, position.Y, position.Z), Quaternion.identity);
        playerObject.name = "Player";
        playerObject.transform.parent = _entityManager.transform;
        _currentPlayer = playerObject.GetComponent<CurrentPlayer>();
        _currentPlayer.InstanceId = initData.InstanceId;
        _entityManager.AddObject(playerObject.GetComponent<WorldObject>(), "Prefabs/Player");

        InGameView.Gui.PlayerHealth.UpdateHealth(initData.CurrentHealth, initData.MaxHealth);
        InGameView.Gui.HeatBar.SetCount(initData.CurrentHeat);
        InGameView.Gui.CoinsBar.SetCount(initData.Coins);

        InGameView.Gui.Inventory.UpdateData(initData.Inventory);
        InGameView.Gui.Equipment.UpdateData(initData.Equipment);
        Camera.main.GetComponent<CameraScript>().target = _currentPlayer.transform;
    }

    public int GetPlayerId()
    {
        return PersistentData.PlayerInstanceId;
    }

    public void SendPlayerInGame()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();

        parameters.Add((byte)ClientParameterCode.SubOperationCode, MessageSubCode.PlayerInGame);

        OperationRequest request = new OperationRequest { OperationCode = (byte)ClientOperationCode.Game, Parameters = parameters };
        
        SendOperation(request, true, 0, false);
    }

    public void SendActionRequest(int index)
    {
        OperationRequest request = new OperationRequest();
        Dictionary<byte, object> para = new Dictionary<byte, object>();
        para.Add((byte)ClientParameterCode.SubOperationCode, MessageSubCode.ActionRequest);
        para.Add((byte)ClientParameterCode.Object, index);
        request.Parameters = para;

        SendOperation(new OperationRequest()
        {
            OperationCode = (byte)ClientOperationCode.Game,
            Parameters = para
        }, true, 0, false);
    }


}
