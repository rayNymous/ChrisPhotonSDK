using System;
using UnityEngine;
using System.Collections;

public class ActionButton : MonoBehaviour
{
    public UILabel Label;
    public int Index;

    private Actions _actionsController;

    private const int Cooldown = 1000;

    private int _cooldownUntil = 0;
    private bool _isOnCooldown = false;

    public UIButton Button;

	// Use this for initialization
	void Start ()
	{
	    Button = GetComponent<UIButton>();
	    _actionsController = transform.parent.GetComponent<Actions>();

	    if (_actionsController == null)
	    {
            _actionsController = transform.parent.parent.GetComponent<Actions>();
	    }

	    if (_actionsController == null)
	    {
	        Debug.Log("Failed to find a parent for action");
	    }
        NGUITools.SetActive(this.gameObject, false);
	}

    void Update()
    {
        if (_isOnCooldown && _cooldownUntil < Environment.TickCount)
        {
            _isOnCooldown = false;
            Button.isEnabled = !_isOnCooldown;
        }
    }

    public void SetEnabled(bool isEnabled)
    {
        Button.isEnabled = isEnabled;
    }

    private void StartCooldown()
    {
        _cooldownUntil = Environment.TickCount + Cooldown;
        _isOnCooldown = true;
        Button.isEnabled = !_isOnCooldown;
    }

    public void OnClick()
    {
        if (_actionsController != null)
        {
            _actionsController.NotifyActionSelection(Index);
            _actionsController.CollapseAltActions();
            StartCooldown();
        }
        else
        {
            Debug.Log("Actions controller is null");
        }
    }

}
