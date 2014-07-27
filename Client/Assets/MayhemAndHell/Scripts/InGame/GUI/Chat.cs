using System;
using UnityEngine;
using System.Collections;

public class Chat : MonoBehaviour
{
    public UISprite ToggleButton;
    public UILabel Text;
    public UIInput Input;
    public GameObject ChatWindow;

    public Color RegularColor;
    public Color UpdatedColor;

    private Gui _gui;

    // Use this for initialization
    void Start()
    {
        _gui = FindObjectOfType<Gui>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Toggle()
    {
        NGUITools.SetActive(ChatWindow.gameObject, !NGUITools.GetActive(ChatWindow));
        ToggleButton.color = RegularColor;
    }

    public void AddMessage(String message)
    {
        Text.text += "\n"+message;
        if (!NGUITools.GetActive(ChatWindow))
        {
            ToggleButton.color = UpdatedColor;
        }
    }

    public void OnMessageSend()
    {
        _gui.GameView.InGameController.ParseChat(Input.value);
        Input.value = "";
        Input.label.text = "[Touch to start typing]";
    }
}
