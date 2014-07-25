using System.Collections.Generic;
using System.Xml.Serialization;
using ExitGames.Client.Photon;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using UnityEngine;
using System.Collections;

public class CharacterSelectGui : MonoBehaviour
{

    public CharacterSelector Selector;
    public UITable ZoneTable;

    public AlertGui Alert;

    private GameObject _zonePrefab;

    private ZoneItem[] _zones;

    private CharacterSelect _view;

    public UILabel Coins;

    public GameObject CharacterCreateWindow;
    public UIButton CharacterCreateButton;
    public UIInput CharacterName;

    public void OnCharacterCreateSubmit()
    {
        if (_view.Controller as CharacterSelectController != null)
        {
            (_view.Controller as CharacterSelectController).SendCreateCharacter(CharacterName.value, "M", "Fighter");
        }
    }

    public void OnCharacterCreateCancel()
    {
        NGUITools.SetActive(CharacterCreateWindow.gameObject, false);
    }

    private void Start()
    {
        _view = FindObjectOfType<CharacterSelect>();
        NGUITools.SetActive(CharacterCreateWindow.gameObject, false);

        _zonePrefab = Resources.Load<GameObject>("Prefabs/Gui/ZoneItem");
        //Initialize(new CharacterSelectData()
        //{
        //    Characters = new CharacterListItem[]
        //    {
        //        new CharacterListItem()
        //        {
        //            Deployed = false,
        //            Name = "Roflcopter"
        //        },
        //        new CharacterListItem()
        //        {
        //            Deployed = true,
        //            Name = "Antrukas"
        //        },
        //        new CharacterListItem()
        //        {
        //            Deployed = false,
        //            Name = "Sajanas13"
        //        }
        //    },
        //    Coins = 1566,
        //    Zones = new ZoneListItem[]
        //    {
        //        new ZoneListItem()
        //        {
        //            MaxPlayers = 99,
        //            Name = "The Flatlands of Freya",
        //            PlayersOnline = 5,
        //            Price = 0,
        //            Stars = 2,
        //            Unlocked = true
        //        },
        //        new ZoneListItem()
        //        {
        //            MaxPlayers = 10,
        //            Name = "The Frozen Canyon",
        //            PlayersOnline = 1,
        //            Price = 10000,
        //            Stars = 0,
        //            Unlocked = false
        //        }
        //    }
        //});
    }

    public void UnlockZoneVisually(int index)
    {
        _zones[index].Unlock(true);
    }

    public void ZoneUnlock(int zoneIndex)
    {
        _view.UnlockZone(zoneIndex);
    }

    public void ZoneEnter(int zoneIndex)
    {
        _view.EnterZone(zoneIndex, GetCharacterIndex());
    }

    public void CreateCharacter()
    {
        NGUITools.SetActive(CharacterCreateWindow.gameObject, true);
        //Application.LoadLevel("CharacterCreate");
    }

    public int GetCharacterIndex()
    {
        return Selector.CurrentCharacterIndex-1;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Initialize(CharacterSelectData data)
    {
        Selector.Initialize(data.Characters);

        _zones = new ZoneItem[data.Zones.Length];

        for (int i = 0; i < _zones.Length; i++)
        {
            _zones[i] = NGUITools.AddChild(ZoneTable.gameObject, _zonePrefab)
                .GetComponent<ZoneItem>();
            _zones[i].Prepare(i, data.Zones[i]);
        }

        ZoneTable.Reposition();
    }

	
}
