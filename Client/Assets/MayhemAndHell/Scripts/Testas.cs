using UnityEngine;
using System.Collections;

public class Testas : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTooltip(bool show)
    {
        Debug.Log("VA");
    }

    public void Ziurim()
    {
        UITooltip.ShowText("Vsskfjsjffffffffds fdf dfdfdf sdf dsfsdf dfsdf sdfsd fdsf sdfdsjfhdjhf dhfdkf hkjsdhfkjhsdkjfhkjsdhfkdshkjfdkh" +
                           "\n Vsskfjsjffffffffds fdf dfdfdf sdf dsfsdf dfsdf sdfsd fdsf sdfdsjfhdjhf dhfdkf hkjsdhfkjhsdkjfhkjsdhfkdshkjfdkh");
    }

}
