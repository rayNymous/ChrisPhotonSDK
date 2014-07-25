using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using UnityEngine;
using System.Collections;

public class TargetBar : MonoBehaviour
{
    public UILabel Label;
    public UIProgressBar ProgressBar;
    private Gui _gui;

    public int InstanceId { get; set; }

	// Use this for initialization
	void Start () {
        _gui = FindObjectOfType<Gui>();
        NGUITools.SetActive(this.gameObject, false);
	}

    public void Show(TargetInfo info)
    {
        InstanceId = info.InstanceId;
        Label.text = info.Name;
        SetHealth(info.CurrentHealth, info.MaxHealth);
        NGUITools.SetActive(this.gameObject, true);
    }

    private void SetHealth(int current, int max)
    {
        ProgressBar.value = max > 0 ? Math.Min(current/(float)max, 1) : 0;
    }

    public void UpdateStatus(TargetStatusData data)
    {
        SetHealth(data.CurrentHealth, data.MaxHealth);
    }

    public void Close()
    {
        _gui.GameView.InGameController.SendOperation(new OperationRequest()
        {
            OperationCode = (byte) ClientOperationCode.Game,
            Parameters = new Dictionary<byte, object>()
            {
                {(byte)ClientParameterCode.SubOperationCode, MessageSubCode.TargetRequest},
            }
        }, true,0, false);
        NGUITools.SetActive(this.gameObject, false);
        _gui.TargetLost();
    }


}
