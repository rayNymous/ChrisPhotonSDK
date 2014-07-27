
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using ExitGames.Client.Photon;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using UnityEngine;

public class ChatHandler : PacketHandler
{
    private EntityManager _entityManager;

    public ChatHandler(InGameController controller) : base(controller)
    {
        _entityManager = UnityEngine.Object.FindObjectOfType<EntityManager>();
    }

    public override MessageSubCode MessageSubCode
    {
        get { return MessageSubCode.Chat; }
    }

    public override void OnHandleEvent(EventData eventData)
    {
        var chatItem = Deserialize<ChatItem>((byte[])eventData.Parameters[(byte)ClientParameterCode.Object]);

        var wObject = _entityManager.GetObject(chatItem.InstanceId);

        Controller.InGameView.Gui.SpeechBubbleManager.DisplayBubble(wObject, chatItem.Text);

        var text = String.Format("[{0}] [{1}] {2}", chatItem.Type, chatItem.Name, chatItem.Text);

        (controller as InGameController).AddChatText(text);

    }
}
