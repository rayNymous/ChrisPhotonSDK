using System;
using UnityEngine;
using System.Collections;

public class Actions : MonoBehaviour
{

    public GameObject AltActions;
    private ActionButton[] _buttons;
    public GameObject MoreActions;

    private TweenPosition _tweenPosition;

    private Gui _gui;

	// Use this for initialization
	void Start ()
	{
        _gui = FindObjectOfType<Gui>();
	    NGUITools.SetActive(AltActions, false);
	    _buttons = transform.GetComponentsInChildren<ActionButton>(true);
        
        // Checking if buttons are in order
	    int lastIndex = -1;
	    foreach (var button in _buttons)
	    {
	        lastIndex++;
	        if (button.Index != lastIndex)
	        {
                Debug.Log("One of the action buttons is not in order " 
                    + button.Index + " / " + lastIndex);
	        }
	    }
	    _tweenPosition = AltActions.GetComponent<TweenPosition>();
	}

    public void NotifyActionSelection(int index)
    {
        _gui.GameView.InGameController.SendActionRequest(index);
    }

    public void PrepareActions(String[] actions)
    {

        if (actions == null || actions.Length < 1)
        {
            NGUITools.SetActive(gameObject, false);
            return;
        }

        if (actions.Length > 0)
        {
            foreach (var button in _buttons)
            {
                if (button.Index < actions.Length)
                {
                    NGUITools.SetActive(button.gameObject, true);
                    button.Label.text = actions[button.Index];
                }
                else
                {
                    NGUITools.SetActive(button.gameObject, false);
                }
            }

            NGUITools.SetActive(MoreActions, actions.Length > 1);
        }
        NGUITools.SetActive(gameObject, true);
    }

    public void SetButtonsEnabled(bool isEnabled)
    {
        foreach (var actionButton in _buttons)
        {
            actionButton.SetEnabled(isEnabled);
        }
    }

    public void ExpandAltActions()
    {
        NGUITools.SetActive(AltActions, !NGUITools.GetActive(AltActions));
        _tweenPosition.ResetToBeginning();
        _tweenPosition.PlayForward();
    }

    public void CollapseAltActions()
    {
        NGUITools.SetActive(AltActions, false);
    }

	// Update is called once per frame
	void Update () {
	
	}

}
