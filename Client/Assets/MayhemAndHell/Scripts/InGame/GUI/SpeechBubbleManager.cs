using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class SpeechBubbleManager : MonoBehaviour
{
    private const int MaxBubbles = 20;

    private List<SpeechBubble> _bubbles;

    private SpeechBubble _prefab;

    private Camera _uiCamera;
    private Camera _mainCamera;

	// Use this for initialization
	void Start () {
        _bubbles = new List<SpeechBubble>();
        _prefab = Resources.Load<SpeechBubble>("Prefabs/Gui/SpeechBubble");
	    _uiCamera = FindObjectOfType<UICamera>().GetComponent<Camera>();
	    _mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void DisplayBubble(WorldObject obj, string text)
    {
        float lowestTimeout = SpeechBubble.MaxDisplayTime;
        int lowestTimeoutIndex = 0;
        int index = 0;
        foreach (var speechBubble in _bubbles)
        {
            if (speechBubble.TimeOut <= 0)
            {
                InitializeSpeechBubble(speechBubble, obj, text);
                return;
            }

            if (speechBubble.TimeOut < lowestTimeout)
            {
                lowestTimeout = speechBubble.TimeOut;
                lowestTimeoutIndex = index;
            }
            index++;
        }

        // If we're here, no recyclable bubbles were found.
        if (_bubbles.Count < MaxBubbles)
        {
            // If there's some free space in the list, let's create a new one
            var newBubble = Instantiate(_prefab) as SpeechBubble;
            newBubble.FollowTarget.uiCamera = _uiCamera;
            newBubble.FollowTarget.gameCamera = _mainCamera;
            newBubble.transform.parent = transform;
            newBubble.transform.name = "" + _bubbles.Count;
            newBubble.transform.localScale = new Vector3(1, 1, 1);
            InitializeSpeechBubble(newBubble, obj, text);
            _bubbles.Add(newBubble);
        }

        // Buffer is full. Let's rewrite the oldest bubble
        InitializeSpeechBubble(_bubbles[lowestTimeoutIndex], obj, text);
    }

    private void InitializeSpeechBubble(SpeechBubble bubble, WorldObject obj, string text)
    {
        bubble.FollowTarget.target = obj.TopPosition;

        bubble.DisplayText(text);
    }

    public void TestIfWorking()
    {
        var obj = GameObject.FindObjectOfType<CurrentPlayer>();
        var text = "Cia mano super tekstukas";
        DisplayBubble(obj, text);
    }
}
