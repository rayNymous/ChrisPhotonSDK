using ExitGames.Client.Photon;
using MayhemCommon;
using UnityEngine;
using System.Collections;

public class LoginHandler : PhotonOperationHandler
{
    private Login _loginView;

    public LoginHandler(ViewController controller) : base(controller)
    {
        _loginView = controller.ControllerView as Login;
    }

    public override byte Code
    {
        get { return (byte) MessageSubCode.Login; }
    }

    public override void OnHandleResponse(OperationResponse response)
    {
        if (response.ReturnCode != 0)
        {
            _loginView.Alert.DisplayWarning(response.DebugMessage);
            return;
        }
        Application.LoadLevel("CharacterSelect");
    }
}
