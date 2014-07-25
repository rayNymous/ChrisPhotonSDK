using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using ExitGames.Client.Photon;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using ProtoBuf;

public class CharacterListHandler : PhotonOperationHandler
{
    public CharacterListHandler(ViewController controller) : base(controller)
    {
    }

    public override byte Code
    {
        get { return (byte) MessageSubCode.ListCharacters; }
    }

    public override void OnHandleResponse(OperationResponse response)
    {
        var selectController = this.controller as CharacterSelectController;

        if (selectController != null)
        {
            CharacterSelectData data;
           
            var bytes = (Byte[]) response.Parameters[(byte) ClientParameterCode.CharacterList];

            using (var ms = new MemoryStream(bytes))
            {
                data = Serializer.Deserialize<CharacterSelectData>(ms);
            };

            if (data != null)
            {
                selectController.View.Gui.Coins.text = "" + data.Coins;

                selectController.Data = data;
                var view = selectController.ControllerView as CharacterSelect;
                if (view != null)
                {
                    view.Gui.Initialize(data);
                }
            }
            else
            {
                this.controller.DebugReturn(DebugLevel.WARNING, "Invalid data");
            }
        }
    }
}
