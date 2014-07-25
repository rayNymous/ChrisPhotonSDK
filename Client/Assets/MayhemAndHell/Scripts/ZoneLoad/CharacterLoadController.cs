using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using MayhemCommon;

class CharacterLoadController : ViewController
{

    public CharacterLoadController(View controlledView, byte subOperationCode = 0) : base(controlledView, subOperationCode)
    {
        EventSubCodeHandlers.Add((int)MessageSubCode.FinishLoading, new FinishLoadingHandler(this));
        EventSubCodeHandlers.Add((int)MessageSubCode.LoadZone, new LoadZoneHandler(this));
    }

    public void SendCharacterLoading()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();

        parameters.Add((byte)ClientParameterCode.SubOperationCode, MessageSubCode.CharacterLoading);

        OperationRequest request = new OperationRequest
        {
            OperationCode = (byte)ClientOperationCode.Game,
            Parameters = parameters
        };

        SendOperation(request, true, 0, true);
    }
}
