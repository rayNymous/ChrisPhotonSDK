using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using MayhemCommon;
using MayhemCommon.MessageObjects;

public class ZoneItemUpdateHandler : PhotonOperationHandler
{
    private CharacterSelectController _controller;

    public ZoneItemUpdateHandler(ViewController controller) : base(controller)
    {
        _controller = controller as CharacterSelectController;
    }

    public override byte Code
    {
        get { return (byte)MessageSubCode.ZoneItemUpdate; }
    }

    public override void OnHandleResponse(OperationResponse response)
    {
        _controller.ControllerView.LogDebug("ZoneItemUpdateHandler");
        if (response.Parameters.ContainsKey((byte)ClientParameterCode.ZoneId))
        {
            // If zone unlock status has changed
            _controller.ControllerView.LogDebug("Trying to unlock a zone");
            var zoneId = new Guid((Byte[])response.Parameters[(byte)ClientParameterCode.ZoneId]);

            ZoneListItem[] zones = _controller.Data.Zones;

            for (int i = 0; i < zones.Length; i++)
            {
                if (zones[i].InstanceId == zoneId)
                {
                    _controller.ControllerView.LogDebug("Found a zone to unlock");
                    _controller.View.Gui.UnlockZoneVisually(i);
                    break;
                }
            }

        }
        if (response.Parameters.ContainsKey((byte) ClientParameterCode.Object2))
        {
            // If zone online players count has changed
        }

        if (response.Parameters.ContainsKey((byte) ClientParameterCode.Object3))
        {
            // If number of coins is changed
            int coins = Convert.ToInt32(response.Parameters[(byte) ClientParameterCode.Object3]);
            _controller.View.Gui.Coins.text = "" + coins;
        }
    }
}