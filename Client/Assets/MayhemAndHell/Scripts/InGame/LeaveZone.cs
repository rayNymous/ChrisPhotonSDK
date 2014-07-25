using UnityEngine;
using System.Collections;

public class LeaveZone : MonoBehaviour
{
    private LeaveZoneButton _button;

	// Use this for initialization
	void Start () {
        _button = FindObjectOfType<Gui>().LeaveZoneButton;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void OnTriggerEnter2D(Collider2D coll)
    {
        NGUITools.SetActive(_button.gameObject, true);
    }

    private void OnTriggerExit2D(Collider2D coll)
    {
        NGUITools.SetActive(_button.gameObject, false);
    }
}

