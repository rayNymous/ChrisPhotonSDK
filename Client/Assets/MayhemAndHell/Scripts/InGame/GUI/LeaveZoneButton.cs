using System.Collections.Generic;
using ExitGames.Client.Photon;
using MayhemCommon;
using UnityEngine;
using System.Collections;

public class LeaveZoneButton : MonoBehaviour {

	private Gui _gui;

	// Use this for initialization
    private void Start()
    {
        _gui = FindObjectOfType<Gui>();
        NGUITools.SetActive(gameObject, false);
    }

    public void OnClick()
    {
        _gui.GameView.Controller.SendOperation(new OperationRequest()
        {
            OperationCode = (byte)ClientOperationCode.Game,
            Parameters = new Dictionary<byte, object>()
            {
                {(byte)ClientParameterCode.SubOperationCode, MessageSubCode.LeaveZone},
                {(byte)ClientParameterCode.Object, false}
            }
        }, true, 0, false);
    }
}
