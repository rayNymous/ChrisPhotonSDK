
public abstract class PhotonEventHandler : IPhotonEventHandler
{
    protected readonly ViewController controller;
    public abstract byte Code { get; }
    public virtual int? SubCode {get { return null; }}

    protected PhotonEventHandler(ViewController controller) 
    {
        this.controller = controller;
    }

    public delegate void BeforeEventReceived();
    public BeforeEventReceived beforeEventReceived;

    public delegate void AfterEventReceived();
    public AfterEventReceived afterEventReceived;

    public void HandleEvent(ExitGames.Client.Photon.EventData eventData)
    {
        if (beforeEventReceived != null)
        {
            beforeEventReceived();
        }
        OnHandleEvent(eventData);
        if (afterEventReceived != null)
        {
            afterEventReceived();
        }
    }

    public abstract void OnHandleEvent(ExitGames.Client.Photon.EventData eventData);
}
