using UnityEngine;
using System.Collections;

public class ClickFallThrough : MonoBehaviour
{
    public static bool IsHandled;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update ()
	{
	    IsHandled = false;
	}

    public void OnClick()
    {
        IsHandled = true;
        Debug.Log("CIA");
    }
}
