using System;
using System.Net.Mime;
using UnityEngine;
using System.Collections;

public class SpeechBubble : MonoBehaviour
{
    private const int MaxWidth = 200;
    public const float MaxDisplayTime = 2;

    public UILabel Text;
    private float _timeOut;
    public UIFollowTarget FollowTarget;

    void Update()
    {
        if (_timeOut > 0)
        {
            _timeOut -= Time.deltaTime;
            if (_timeOut < 0)
            {
                NGUITools.SetActive(gameObject, false);
            }
        }
    }

    public float TimeOut { get { return _timeOut; } }

    public void DisplayText(string text)
    {
        NGUITools.SetActive(gameObject, true);
        _timeOut = MaxDisplayTime;

        Text.text = text;

        Text.overflowMethod = UILabel.Overflow.ResizeFreely;

        if (Text.printedSize.x > MaxWidth)
        {
            Text.overflowMethod = UILabel.Overflow.ResizeHeight;
            Text.width = MaxWidth;
        }

    }
}
