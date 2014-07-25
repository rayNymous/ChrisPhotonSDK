using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using ProtoBuf;

public class PlayerInitHandler : PacketHandler
{
    public PlayerInitHandler(InGameController controller) : base(controller)
    {
    }

    public override MessageSubCode MessageSubCode
    {
        get { return MessageSubCode.PlayerInit; }
    }

    public override void OnHandleEvent(EventData eventData)
    {
        controller.ControllerView.LogDebug("HANDLED PLAYER INIT");
        var data = (byte[])eventData.Parameters[ (byte)ClientParameterCode.Object];
        PlayerInitData playerInitData = Deserialize<PlayerInitData>(data);
        Controller.InstantiatePlayer(playerInitData);
    }
}

