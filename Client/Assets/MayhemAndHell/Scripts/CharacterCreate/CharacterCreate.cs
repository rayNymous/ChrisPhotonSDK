using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ExitGames.Client.Photon;
using MayhemCommon;
using UnityEngine;
using System.Collections;

public class CharacterCreate : View {
    private string characterName = "";
    private string sex = "";
    private string characterClass = "";

    // Use this for initialization
    public override void Awake()
    {
        Controller = new CharacterCreateController(this);
    }

    private CharacterCreateController controller;

    public override IViewController Controller
    {
        get { return controller; }
        protected set { controller = value as CharacterCreateController; }
    }
    
    void OnGUI()
    {
        if (controller.ShowErrorDialog)
        {
            GUI.Label(new Rect(10, 300, 505, 40), controller.Error);
        }

        GUI.Label(new Rect(120, 116, 100, 100), "Name");
        characterName = GUI.TextField(new Rect(200, 116, 200, 20), characterName, 25);

        GUI.Box(new Rect(10, 10, 100, 300), "Classes");

        if (GUI.Button(new Rect(20, 50, 80, 50), "Fighter"))
        {
            characterClass = "Fighter";
        }

        if (GUI.Button(new Rect(20, 110, 80, 50), "Mage"))
        {
            characterClass = "Mage";
        }

        if (GUI.Button(new Rect(20, 170, 80, 50), "Rogue"))
        {
            characterClass = "Rogue";
        }

        if (GUI.Button(new Rect(20, 230, 80, 50), "Cleric"))
        {
            characterClass = "Cleric";
        }

        if (GUI.Button(new Rect(150, 170, 80, 50), "Male"))
        {
            sex = "M";
        }

        if (GUI.Button(new Rect(240, 170, 80, 50), "Female"))
        {
            sex = "F";
        }

        if (!controller.SendingCreate &&
            ((GUI.Button(new Rect(200, 265, 100, 25), "Create") ||
             (Event.current.type == EventType.KeyDown && Event.current.character == '\n'))))
        {
            if (!string.IsNullOrEmpty(characterName) && !string.IsNullOrEmpty(sex) &&
                !string.IsNullOrEmpty(characterClass))
            {
                controller.SendCreateCharacter(characterName, sex, characterClass);
            }
        }

        if (!controller.SendingCreate && GUI.Button(new Rect(305, 265, 100, 25), "Cancel"))
        {
            Application.LoadLevel("CharacterSelect");
        }

    }

}
