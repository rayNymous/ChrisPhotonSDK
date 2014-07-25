using UnityEngine;
using System.Collections;

public class DialogLinkButton : MonoBehaviour
{
    public int Index;

    public DialogWindow Window;

    public UILabel Text;
    public UISprite Icon;

	void Start () {
	
	}
	
	void Update () {
	
	}

    void OnClick()
    {
        Window.OnLinkClick(Index);
    }
}
