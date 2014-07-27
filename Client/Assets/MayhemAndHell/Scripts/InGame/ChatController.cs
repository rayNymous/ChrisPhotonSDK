
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ExitGames.Client.Photon;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using UnityEngine;

public partial class InGameController
{
    public string ChatText { get; set; }

    public void InitializeChat()
    {
        ChatText = "";
    }

    public void AddChatText(string chatInput)
    {
        InGameView.Gui.Chat.AddMessage(chatInput);
    }

    public void ParseChat(string chatInput)
    {
        if (chatInput.StartsWith("/"))
        {
            // Commands like group invite, whisper and etc.
            SendChatText(chatInput);
        }
        else
        {
            SendChatText(chatInput);
        }
    }

    private void SendChatText(string text)
    {
        if (String.IsNullOrEmpty(text))
        {
            return;
        }

        var param = new Dictionary<byte, object>()
        {
            {(byte) ClientParameterCode.SubOperationCode, MessageSubCode.Chat},
            {(byte) ClientParameterCode.Object, text}
        };

        var request = new OperationRequest()
        {
            OperationCode = (byte) ClientOperationCode.Chat,
            Parameters = param
        };

        SendOperation(request, true, 0, true);
    }

}