using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using UnityEngine;
using System.Collections;

public class Storage : MonoBehaviour
{
    public ItemStorage GlobalStorage;
    public UILabel Coins;

    public GameObject InputWindow;
    public UILabel InputAmount;
    public UIInput Input;
    public UILabel InputOkText;

    private bool _withdraw;

    private Gui _gui;

	// Use this for initialization
    void Start()
    {
        _gui = FindObjectOfType<Gui>();
        NGUITools.SetActive(InputWindow, false);
        NGUITools.SetActive(this.gameObject, false);
    }

    public void SetCoins(int coins)
    {
        Coins.text = "" + coins;
    }

    public void Show(GlobalStorageData data)
    {
        SetCoins(data.Coins);

        foreach (var itemData in data.Items)
        {
            GlobalStorage.ForceReplace(itemData.Key, itemData.Value == null ? null : new ContainerItem(itemData.Value)
            {
                Atlas = _gui.IconAtlas
            });
        }

        NGUITools.SetActive(gameObject, true);
    }

    public void CloseWindow()
    {
        NGUITools.SetActive(gameObject, false);
    }

    public void OnInputCancel()
    {
        NGUITools.SetActive(InputWindow, false);
    }

    public void OnCoinsSend()
    {
        GlobalStorage.Gui.GameView.InGameController.SendOperation(new OperationRequest()
        {
            OperationCode = (byte)ClientOperationCode.Game,
            Parameters = new Dictionary<byte, object>()
            {
                {(byte)ClientParameterCode.SubOperationCode, MessageSubCode.CoinsTransfer},
                {(byte)ClientParameterCode.Object, InputAmount.text},
                {(byte)ClientParameterCode.Object2, _withdraw}
            }
        }, true, 0, false);
        NGUITools.SetActive(InputWindow, false);
    }

    public void OnWithdraw()
    {
        _withdraw = true;
        InputOkText.text = "Withdraw";
        InputAmount.text = "0";
        NGUITools.SetActive(InputWindow, true);
        Input.value = "0";
    }

    public void OnDeposit()
    {
        _withdraw = false;
        InputOkText.text = "Deposit";
        InputAmount.text = "0";
        NGUITools.SetActive(InputWindow, true);
        Input.value = "0";
    }
}
