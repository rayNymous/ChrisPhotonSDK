using System.Net.Sockets;
using ExitGames.Client.Photon;
using UnityEngine;

public class PhotonEngine : MonoBehaviour, IPhotonPeerListener
{
    public PhotonPeer Peer { get; protected set; }
    public GameState State { get; protected set; }
    public ViewController Controller { get; set; }

    public string ServerAddress = "localhost:5055";
    public string ApplicationName = "ComplexServer";

    private static PhotonEngine instance;

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        DontDestroyOnLoad(this);
        State = new Disconnected(instance);
        Application.runInBackground = true;
        Initialize();
    }

    public static PhotonEngine Instance
    {
        get { return instance; }
    }

    public void Initialize()
    {
        PhotonNetwork.ConnectUsingSettings("1.0");
        PhotonNetwork.networkingPeer.CustomListener = this;
        Peer = PhotonNetwork.networkingPeer;

        State = new WaitingForConnection(instance);
    }

    void OnConnectedToPhoton()
    {
        Debug.Log("This client has connected to a server");
    }

    void OnDisconnectedFromPhoton()
    {
        
    }

    public void Disconnect()
    {
        if (Peer != null)
        {
            Peer.Disconnect();
        }
        State = new Disconnected(instance);
    }

    public void Update()
    {
        //Check connection state..
        //if (!PhotonNetwork.connected && !PhotonNetwork.connecting)
        //{
        //    //We are currently disconnected
        //    //GUILayout.Label("Connection status: Disconnected");
        //    Debug.Log("DISCONNECTED");
        //}
        //else
        //{
        //    Debug.Log("CONNECTED " + PhotonNetwork.connecting);
        //}

        State.OnUpdate();
    }

    public void SendOp(OperationRequest request, bool sendReliable, byte channellId, bool encrypt)
    {
        State.SendOperation(request, sendReliable, channellId, encrypt);
    }

    public static void UseExistingOrCreateNewPhotonEngine(string serverAddress, string applicationName)
    {
        GameObject tempEngine;
        PhotonEngine myEngine;

        tempEngine = GameObject.Find("PhotonEngine");
        if (tempEngine == null)
        {
            tempEngine = new GameObject("PhotonEngine");
            tempEngine.AddComponent<PhotonEngine>();
        }

        myEngine = tempEngine.GetComponent<PhotonEngine>();

        myEngine.ApplicationName = applicationName;
        myEngine.ServerAddress = serverAddress;
    }

    #region Implementation of  IPhotonPeerListener

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log(message);
        Controller.DebugReturn(level, message);
    }

    public void OnEvent(EventData eventData)
    {
        Controller.OnEvent(eventData);
    }

    public void OnOperationResponse(OperationResponse operationResponse)
    {
        Controller.OnOperationResponse(operationResponse);
    }

    public void OnStatusChanged(StatusCode statusCode)
    {
        //switch (statusCode)
        //{
        //    case StatusCode.Connect:
        //        Peer.EstablishEncryption();
        //        break;
        //    case StatusCode.Disconnect:
        //    case StatusCode.DisconnectByServer:
        //    case StatusCode.DisconnectByServerLogic:
        //    case StatusCode.DisconnectByServerUserLimit:
        //    case StatusCode.Exception:
        //    case StatusCode.ExceptionOnConnect:
        //    case StatusCode.TimeoutDisconnect:
        //        Controller.OnDisconnected("" + statusCode);
        //        State = new Disconnected(instance);
        //        break;
        //    case StatusCode.EncryptionEstablished:
        //        State = new Connected(instance);
        //        break;
        //    default:
        //        Controller.OnUnexpectedStatusCode(statusCode);
        //        State = new Disconnected(instance);
        //        break;
        //}
    }

    #endregion
}

