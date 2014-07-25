using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Assets.MayhemAndHell.Scripts.CharacterSelect.Handlers;
using ExitGames.Client.Photon;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using UnityEngine;
using System.Collections;

public class CharacterSelectController : ViewController
{

    public CharacterSelect View { get; set; }
    public UILabel Coins;

    public CharacterSelectController(View controlledView) : base(controlledView)
    {
        View = controlledView as CharacterSelect;
        OperationHandlers.Add((byte)MessageSubCode.ListCharacters, new CharacterListHandler(this));
        OperationHandlers.Add((byte)MessageSubCode.SelectCharacter, new SelectCharacterHandler(this));
        OperationHandlers.Add((byte)MessageSubCode.ZoneItemUpdate, new ZoneItemUpdateHandler(this));
        OperationHandlers.Add((byte)MessageSubCode.CreateCharacter, new CharacterCreateHandler(this));
        OperationHandlers.Add((byte)MessageSubCode.Alert, new AlertHandler(this));
    }

    public CharacterSelectData Data { get; set; }

    public void SendGetList()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
       
        parameters.Add((byte)ClientParameterCode.SubOperationCode, MessageSubCode.ListCharacters);
        
        OperationRequest request = new OperationRequest{OperationCode = (byte) ClientOperationCode.Login, Parameters = parameters};
        
        SendOperation(request, true, 0, false);
    }

    public void UnlockZone(int zoneIndex)
    {
        var request = new OperationRequest
        {
            OperationCode = (byte)ClientOperationCode.Login,
            Parameters = new Dictionary<byte, object>()
            {
                {(byte) ClientParameterCode.SubOperationCode, MessageSubCode.ZoneUnlock},
                {(byte) ClientParameterCode.ZoneId, Data.Zones[zoneIndex].InstanceId.ToByteArray()}
            }
        };

        SendOperation(request, true, 0, false);
    }

    public void SendCharacterSelect(int characterIndex, int zoneIndex)
    {
        if (Data.Characters != null && Data.Characters.Length > characterIndex)
        {
            var request = new OperationRequest
            {
                OperationCode = (byte) ClientOperationCode.Login,
                Parameters = new Dictionary<byte, object>()
                {
                    {(byte) ClientParameterCode.SubOperationCode, MessageSubCode.SelectCharacter},
                    {(byte) ClientParameterCode.CharacterId, Data.Characters[characterIndex].Id.ToByteArray()},
                    {(byte) ClientParameterCode.ZoneId, Data.Zones[zoneIndex].InstanceId.ToByteArray()}
                }
            };
            SendOperation(request, true, 0, false);
        }
        else
        {
            View.Gui.Alert.DisplayWarning("You must select a character first!");
        }
    }

    public void SendCreateCharacter(string characterName, string sex, string characterClass)
    {
        CharacterCreateDetails details = new CharacterCreateDetails
        {
            CharacterName = characterName,
            Sex = sex,
            CharacterClass = characterClass
        };

        XmlSerializer mySerializer = new XmlSerializer(typeof(CharacterCreateDetails));
        StringWriter outStream = new StringWriter();
        mySerializer.Serialize(outStream, details);

        Dictionary<byte, object> parameters = new Dictionary<byte, object>()
        {
            {(byte) ClientParameterCode.SubOperationCode, MessageSubCode.CreateCharacter},
            {(byte) ClientParameterCode.CharacterCreateDetails, outStream.ToString()}
        };

        OperationRequest request = new OperationRequest { OperationCode = (byte)ClientOperationCode.Login, Parameters = parameters };
        SendOperation(request, true, 0, false);
    }
}
