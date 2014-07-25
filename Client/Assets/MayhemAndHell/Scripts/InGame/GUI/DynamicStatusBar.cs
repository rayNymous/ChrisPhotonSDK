using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class DynamicStatusBar : MonoBehaviour
{
    public UILabel Text;
    public UILabel Difference;

    private int _count;

    public TweenScale Scale;
    public TweenAlpha Alpha;
    public TweenPosition Position;

    public Color PositiveColor;
    public Color NegativeColor;

    void Start()
    {
        NGUITools.SetActive(Difference.gameObject, false);
    }

    public void SetCount(int count)
    {
        _count = count;
        UpdateText();
    }

    public void UpdateText()
    {
        Text.text = _count + "";
    }

    public void UpdateCount(int count)
    {
        int difference = count - _count;
        _count = count;

        UpdateText();

        if (difference > 0)
        {
            Difference.text = "+" + difference;
            Difference.color = PositiveColor;
        }
        else if (difference < 0)
        {
            Difference.text = "" + difference;
            Difference.color = NegativeColor;
        }
        else
        {
            return;
        }

        if (!NGUITools.GetActive(Difference.gameObject))
        {
            NGUITools.SetActive(Difference.gameObject, true);
        }

        Scale.ResetToBeginning();
        Scale.PlayForward();
        Alpha.ResetToBeginning();
        Alpha.PlayForward();
        Position.ResetToBeginning();
        Position.PlayForward();
    }
}
