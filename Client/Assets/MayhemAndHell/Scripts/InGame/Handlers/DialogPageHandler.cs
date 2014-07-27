
using ExitGames.Client.Photon;
using MayhemCommon;
using MayhemCommon.MessageObjects;

public class DialogPageHandler : PacketHandler
{
    public DialogPageHandler(InGameController controller) : base(controller)
    {
    }

    public override void OnHandleEvent(EventData eventData)
    {
        DialogPageData pageData = Deserialize<DialogPageData>((byte[])eventData.Parameters[(byte)ClientParameterCode.Object]);
        Controller.InGameView.Gui.DialogWindow.ShowDialog(pageData);
    }

    public override MessageSubCode MessageSubCode
    {
        get { return MessageSubCode.DialogPage; }
    }
}
