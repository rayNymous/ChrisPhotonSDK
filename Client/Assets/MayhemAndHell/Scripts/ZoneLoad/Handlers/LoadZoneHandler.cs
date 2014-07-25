using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.MayhemAndHell.Scripts.InGame;
using ExitGames.Client.Photon;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using UnityEngine;

public class LoadZoneHandler : PhotonEventHandler
{
    public LoadZoneHandler(ViewController controller) : base(controller)
    {
    }

    public override byte Code
    {
        get { return (byte) ClientEventCode.ServerPacket; }
    }

    public override int? SubCode
    {
        get { return (int)MessageSubCode.LoadZone; }
    }

    public override void OnHandleEvent(EventData eventData)
    {
        string zoneName = (string) eventData.Parameters[(byte) ClientParameterCode.ZoneId];
        PersistentData.PlayerInstanceId = Convert.ToInt32(eventData.Parameters[(byte)ClientParameterCode.InstanceId]);
        byte[] rawPosition = (byte[]) eventData.Parameters[(byte) ClientParameterCode.Object];

        StaticAssets.Initialize();

        var position = PacketHandler.Deserialize<PositionData>(rawPosition);
        PersistentData.LoadZonePosition = position;
        Application.LoadLevel(zoneName);
    }
}
