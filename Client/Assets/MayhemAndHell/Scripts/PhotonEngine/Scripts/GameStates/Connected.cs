public class Connected : GameState
{
    public Connected(PhotonEngine engine)
        : base(engine)
    {
    }

    public override void OnUpdate()
    {
        engine.Peer.Service();
    }

    public override void SendOperation(ExitGames.Client.Photon.OperationRequest request, bool sendReliable, byte channelId, bool encrypt)
    {
        engine.Peer.OpCustom(request, sendReliable, channelId, encrypt);
    }
}
