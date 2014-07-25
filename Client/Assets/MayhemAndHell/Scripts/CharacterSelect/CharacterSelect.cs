using System;
using System.Security.Policy;
using MayhemCommon.MessageObjects;
using UnityEngine;
using System.Collections;

public class CharacterSelect : View
{

    public CharacterSelectGui Gui;

	// Use this for initialization
	public override void Awake ()
	{
	    Controller = new CharacterSelectController(this);
        controller.SendGetList();
	}

    private CharacterSelectController controller;

    public override IViewController Controller
    {
        get { return controller; }
        protected set { controller = value as CharacterSelectController; }
    }

    public void EnterZone(int zoneIndex, int characterIndex)
    {
        controller.SendCharacterSelect(characterIndex, zoneIndex);
    }

    public void UnlockZone(int zoneIndex)
    {
        controller.UnlockZone(zoneIndex);
    }

}
