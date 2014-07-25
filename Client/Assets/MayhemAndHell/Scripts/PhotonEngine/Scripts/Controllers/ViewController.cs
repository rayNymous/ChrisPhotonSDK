using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCommon;
using UnityEngine;
using ExitGames.Client.Photon;

public class ViewController : IViewController
{
    private readonly View controlledView;
    private readonly byte subOperationCode;
    public View ControllerView { get { return controlledView; } }
    private readonly Dictionary<byte, IPhotonOperationHandler> operationHandlers = new Dictionary<byte,IPhotonOperationHandler>();
    private readonly Dictionary<int, IPhotonEventHandler> eventSubCodeHandlers = new Dictionary<int, IPhotonEventHandler>();
    private readonly Dictionary<byte, IPhotonEventHandler> eventHandlers = new Dictionary<byte, IPhotonEventHandler>();

    public ViewController(View controlledView, byte subOperationCode = 0)
    {
        this.controlledView = controlledView;
        this.subOperationCode = subOperationCode;
        if (PhotonEngine.Instance == null)
        {
            Application.LoadLevel(0);
        }
        else
        {
            PhotonEngine.Instance.Controller = this;
        }
    }

    public Dictionary<byte, IPhotonOperationHandler> OperationHandlers
    {
        get { return operationHandlers; }
    }

    public Dictionary<byte, IPhotonEventHandler> EventHandlers
    {
        get { return eventHandlers; }
    }

    public Dictionary<int, IPhotonEventHandler> EventSubCodeHandlers
    {
        get { return eventSubCodeHandlers; }
    }

    #region Implementation of IViewController
    public bool IsConnected { get { return PhotonEngine.Instance.State is Connected; } }

    public void ApplicationQuit()
    {
        PhotonEngine.Instance.Disconnect();
    }

    public void Connect()
    {
        if (!IsConnected)
        {
            PhotonEngine.Instance.Initialize();
        }
    }

    public void SendOperation(OperationRequest request, bool sendReliable, byte channelId, bool encrypt)
    {
        PhotonEngine.Instance.SendOp(request, sendReliable, channelId, encrypt);
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        controlledView.LogDebug(string.Format("{0} - {1}", level, message));
    }

    public void OnOperationResponse(OperationResponse operationResponse)
    {
        IPhotonOperationHandler handler;
        if (operationResponse.Parameters.ContainsKey(subOperationCode) &&
            OperationHandlers.TryGetValue(
                Convert.ToByte(operationResponse.Parameters[subOperationCode]), out handler))
        {
            handler.HandleResponse(operationResponse);
        }
        else
        {
            OnUnexpectedOperationResponse(operationResponse);
        }
    }

    public void OnEvent(EventData eventData)
    {
        IPhotonEventHandler handler;

        if (eventData.Parameters.ContainsKey((byte) ClientParameterCode.SubOperationCode) &&
            EventSubCodeHandlers.TryGetValue((int) eventData.Parameters[(byte) ClientParameterCode.SubOperationCode],
                out handler))
        {
            handler.HandleEvent(eventData);
        } 
        else if (eventHandlers.TryGetValue(eventData.Code, out handler))
        {
            handler.HandleEvent(eventData);
        }
        else
        {
            OnUnexpectedEvent(eventData);
        }
    }

    public void OnUnexpectedEvent(EventData eventData)
    {
        controlledView.LogError(string.Format("Unexpected Event {0} ", eventData.Code));
    }

    public void OnUnexpectedOperationResponse(OperationResponse operationResponse)
    {
        String subCode = operationResponse.Parameters.ContainsKey((byte) ClientParameterCode.SubOperationCode)
            ? ((MessageSubCode)operationResponse.Parameters[(byte)ClientParameterCode.SubOperationCode]).ToString()
            : "none";
        Debug.LogError(string.Format("Unexpected Operation Error {0} from operation {1} suboperation {2}", (int)operationResponse.ReturnCode, (ClientOperationCode)operationResponse.OperationCode, subCode));
    }

    public void OnUnexpectedStatusCode(StatusCode statusCode)
    {
        controlledView.LogError(string.Format("Unexpected Status {0} ", statusCode));
    }

    public void OnDisconnected(string message)
    {
        controlledView.Disconnected(message);
    }
    #endregion
}
