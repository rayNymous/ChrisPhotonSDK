using System;
using MayhemCommon.MessageObjects;
using UnityEngine;
using System.Collections;

public class ZoneItem : MonoBehaviour
{
    public UILabel Name;
    public UILabel Players;
    public UISprite[] Stars;
    public UIButton EnterButton;
    public UILabel EnterButtonText;
    public GameObject Lock;
    public UILabel Price;
    public UIButton UnlockButton;
    public UI2DSprite Thumbnail;

    private const String GrayStar = "star_gray";
    private const String YellowStar = "star_yellow";

    private int _index;

    private CharacterSelectGui _gui;

    void Start()
    {
        _gui = FindObjectOfType<CharacterSelectGui>();
    }

    public void Prepare(int index, ZoneListItem zone)
    {
        _index = index;
        Name.text = zone.Name;
        Players.text = "Online: " + zone.PlayersOnline + "/" + zone.MaxPlayers;

        for (int i = 0; i < Stars.Length; i++)
        {
            
            Stars[i].spriteName = i+1 <= zone.Stars ? YellowStar : GrayStar;
        }

        Thumbnail.sprite2D = Resources.LoadAll<Sprite>("Sprites/backgrounds")[Convert.ToInt32(zone.Thumbnail)];

        EnterButtonText.text = String.IsNullOrEmpty(zone.CharacterName) ? "Jump In" : "Continue";
        Price.text = zone.Price+"";
        Unlock(zone.Unlocked);
    }

    public void Unlock(bool isUnlocked)
    {
        NGUITools.SetActive(Lock, !isUnlocked);
        EnterButton.isEnabled = isUnlocked;
    }

    public void OnUnlockClick()
    {
        _gui.ZoneUnlock(_index);
    }

    public void OnEnterClick()
    {
        _gui.ZoneEnter(_index);
    }
}
