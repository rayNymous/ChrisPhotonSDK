using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using MayhemCommon;
using UnityEngine;


class FinishLoadingHandler : PhotonEventHandler
{
    public FinishLoadingHandler(ViewController controller) : base(controller)
    {
    }

    public override byte Code
    {
        get { return (byte) ClientEventCode.ServerPacket; }
    }

    public override int? SubCode
    {
        get { return (int)MessageSubCode.FinishLoading; }
    }

    public override void OnHandleEvent(EventData eventData)
    {
        Application.LoadLevel("InGame");
    }
}
