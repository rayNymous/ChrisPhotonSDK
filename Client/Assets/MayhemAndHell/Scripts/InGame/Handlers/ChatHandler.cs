
using System;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using ExitGames.Client.Photon;
using MayhemCommon;
using MayhemCommon.MessageObjects;

public class ChatHandler : PhotonEventHandler
{

    public string ChatText { get; set; }

    public ChatHandler(InGameController controller) : base(controller)
    {
    }

    public override byte Code
    {
        get { return (byte) ClientEventCode.Chat; }
    }

    public override void OnHandleEvent(EventData eventData)
    {
        InGameController controller = this.controller as InGameController;

        if (controller != null)
        {
            if (eventData.Parameters.ContainsKey((byte) ClientParameterCode.Object))
            {
                XmlSerializer mySerializer = new XmlSerializer(typeof(ChatItem));
                StringReader inStream = new StringReader((string)eventData.Parameters[(byte)ClientParameterCode.Object]);
                controller.AddChatText(((ChatItem)mySerializer.Deserialize(inStream)).Text);
            }
        }
    }
}
