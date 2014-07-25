public class GameState : IGameState
{
    protected PhotonEngine engine;

    protected GameState(PhotonEngine engine)
    {
        this.engine = engine;
    }

    // do nothing
    public virtual void OnUpdate()
    {
    }

    // do nothing
    public virtual void SendOperation(ExitGames.Client.Photon.OperationRequest request, bool sendReliable, byte channelId, bool encrypt)
    {
    }
}
