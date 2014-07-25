using UnityEngine;
using System.Collections;

public class CombatTextCollection : MonoBehaviour
{
    private GameObject TextPrefab;
    public Camera UiCamera;
    private Camera _mainCamera;

	// Use this for initialization
	void Start () {
        TextPrefab = Resources.Load<GameObject>("Prefabs/Gui/CombatText");
	    _mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
	}

    public HUDText CreateHudText(GameObject followTarget)
    {
        var hudText = NGUITools.AddChild(this.gameObject, TextPrefab);

        UIFollowTarget follow = hudText.GetComponent<UIFollowTarget>();
        follow.target = followTarget.transform;
        follow.uiCamera = UiCamera;
        follow.gameCamera = _mainCamera;

        return hudText.GetComponent<HUDText>();
    }

}
