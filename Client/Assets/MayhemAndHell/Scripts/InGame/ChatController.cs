
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

    public void AddChatHandlers()
    {
        EventHandlers.Add((byte)ClientEventCode.Chat, new ChatHandler(this));
    }

    public void AddChatText(string chatInput)
    {
        InGameView.Gui.Chat.OnMessageReceived(chatInput);
    }

    public void ParseChat(string chatInput)
    {
        if (chatInput.StartsWith("/"))
        {
            // Commands like group invite, whisper and etc.
        }
        else
        {
            ChatItem item = new ChatItem() {Type = ChatType.General, WhisperPlayer = null, Text = chatInput};
            SendChatItem(item);
        }
    }

    private void SendChatItem(ChatItem item)
    {
        XmlSerializer chatSerializer = new XmlSerializer(typeof(ChatItem));
        StringWriter outStream = new StringWriter();
        chatSerializer.Serialize(outStream, item);

        Dictionary<byte, object> param = new Dictionary<byte, object>()
        {
            {(byte) ClientParameterCode.SubOperationCode, MessageSubCode.Chat},
            {(byte) ClientParameterCode.Object, outStream.ToString()}
        };

        OperationRequest request = new OperationRequest()
        {
            OperationCode = (byte) ClientOperationCode.Chat,
            Parameters = param
        };

        SendOperation(request, true, 0, true);
    }

}