using System;
using UnityEngine;
using System.Collections;

public class Gui : MonoBehaviour
{
    public UIAtlas IconAtlas;
    public Actions Actions;
    public DialogWindow DialogWindow;
    public EventNotifications EventNotifications;
    public InGame GameView;
    public TargetBar TargetBar;
    public PlayerHealth PlayerHealth;
    public CombatTextCollection CombatTextCollection;
    public LootContainer LootContainer;
    public Inventory Inventory;
    public Storage Storage;
    public CoinsBar CoinsBar;
    public DynamicStatusBar HeatBar;
    public LeaveZoneButton LeaveZoneButton;
    public Equipment Equipment;
    public Chat Chat;
    public DeathWindow DeathWindow;

    public Guid TargetId { get; set; }

	// Use this for initialization
	void Start ()
	{
	    GameView = FindObjectOfType<InGame>();
	}

    public void TargetLost()
    {
        NGUITools.SetActive(TargetBar.gameObject, false);
        NGUITools.SetActive(Actions.gameObject, false);
        GameView.Target.Disable();
    }
	
	// Update is called once per frame
	void Update () {
	}
}
