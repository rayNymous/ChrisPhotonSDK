using System.Net.Mime;
using ExitGames.Client.Photon;
using MayhemCommon;
using UnityEngine;

public class CharacterCreateHandler : PhotonOperationHandler 
{
    public CharacterCreateHandler(ViewController controller) : base(controller)
    {
    }

    public override byte Code
    {
        get { return (byte) MessageSubCode.CreateCharacter; }
    }

    public override void OnHandleResponse(OperationResponse response)
    {
        var controller = this.controller as CharacterSelectController;
        if (controller != null)
        {
            if (response.ReturnCode != 0)
            {
                controller.View.Gui.Alert.DisplayWarning(response.DebugMessage);
                return;
            }
            Application.LoadLevel("CharacterSelect");
        }
    }
}
