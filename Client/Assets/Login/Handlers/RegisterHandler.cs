using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using MayhemCommon;
using UnityEngine;

public class RegisterHandler : PhotonOperationHandler
{
    private Login _loginView;

    public RegisterHandler(ViewController controller): base(controller)
    {
        _loginView = controller.ControllerView as Login;
    }

    public override byte Code
    {
        get { return (byte) MessageSubCode.Login; }
    }

    public override void OnHandleResponse(OperationResponse response)
    {
        if (response.ReturnCode == (byte) ClientReturnCode.UserCreated)
        {
            _loginView.OnRegistrationSuccessful();
        }
        else
        {
            _loginView.Alert.DisplayWarning(response.DebugMessage);
        }
    }
}
