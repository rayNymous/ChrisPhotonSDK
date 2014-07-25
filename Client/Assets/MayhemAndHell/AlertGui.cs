using System;
using UnityEngine;
using System.Collections;

public class AlertGui : MonoBehaviour
{
    public UILabel Message;

    public UIButton CloseButton;
    public UIButton NoButton;
    public UIButton YesButton;

	void Start () {
	    NGUITools.SetActive(gameObject, false);
	}

    public void DisplayWarning(String notification)
    {
        Message.text = notification;
        NGUITools.SetActive(NoButton.gameObject, false);
        NGUITools.SetActive(YesButton.gameObject, false);
        NGUITools.SetActive(CloseButton.gameObject, true);
        NGUITools.SetActive(gameObject, true);
    }

    public void DisplayYesNo(String notification)
    {
        NGUITools.SetActive(NoButton.gameObject, true);
        NGUITools.SetActive(YesButton.gameObject, true);
        NGUITools.SetActive(CloseButton.gameObject, false);
        NGUITools.SetActive(gameObject, true);
    }

    public void OnYes()
    {
        Close();
    }

    public void OnNo()
    {
        Close();
    }

    public void Close()
    {
        NGUITools.SetActive(gameObject, false);
    }
	
}
