using System;
using Assets.MayhemAndHell.Scripts.InGame;
using MayhemCommon;
using UnityEngine;
using System.Collections;

public class Hint : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;

    public static Sprite GetSpriteFromHint(ObjectHint hint)
    {
        switch (hint)
        {
            case ObjectHint.QuestGive:
                return StaticAssets.Instance.SheetSprites[1];
            case ObjectHint.QuestReturn:
                return StaticAssets.Instance.SheetSprites[3];
            case ObjectHint.QuestInProgress:
                return StaticAssets.Instance.SheetSprites[2];
            case ObjectHint.QuestAttack:
                break;
        }
        return null;
    }

	// Use this for initialization
	void Start () {
	}

    public void Show(ObjectHint hint)
    {
        SpriteRenderer.enabled = true;
        var sprite = GetSpriteFromHint(hint);

        if (sprite != null)
        {
            SpriteRenderer.sprite = sprite;
            SpriteRenderer.enabled = true;
        }
        else
        {
            SpriteRenderer.enabled = false;
        }
    }

	// Update is called once per frame
	void Update () {
	
	}
}
