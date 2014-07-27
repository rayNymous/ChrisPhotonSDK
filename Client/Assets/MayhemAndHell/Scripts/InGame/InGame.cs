using MayhemCommon.MessageObjects;
using UnityEngine;
using System.Collections;

public class InGame : View
{
    public Gui Gui;

    public TargetFollow Target;

	// Use this for initialization
	public override void Awake ()
	{
	    Controller = new InGameController(this);
	    _controller.SendPlayerInGame();
	    UICamera.fallThrough = gameObject;
	}

    void OnClick()
    {
        // If click falls throught
        var newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //InGameController.Player.MoveLocallyTo(newPosition);
    }

    void Start()
    {
        NGUITools.SetActive(Target.gameObject, false);
        // Teleporting camera to characters position
        Vector3 current = Camera.main.transform.position;
        PositionData position = PersistentData.LoadZonePosition;
        current.x = position.X;
        current.y = position.Y;
        current.z = position.Z;
        Camera.main.transform.position = current;
        //UICamera.fallThrough = GameObject.Find("ClickFallThrough");
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    public InGameController InGameController {get { return _controller; }}

    private InGameController _controller;

    public override IViewController Controller { get { return _controller; } protected set {_controller = value as InGameController;} }

    private int chatWidth = 500;
    private int chatHeight = 200;
    private Vector2 scrollPosition = Vector2.zero;
    private string chatInput = "";

    public void OnGUI()
    {
        //GUILayout.BeginArea(new Rect(5, 5, chatWidth, chatHeight), "", "");
        //scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        //controller.ChatText = GUILayout.TextArea(controller.ChatText, GUILayout.MaxHeight(200));
        //GUILayout.EndScrollView();
        //GUILayout.EndArea();

        //GUILayout.BeginArea(new Rect(5, chatHeight+10, chatWidth, 30), "", "");
        //GUILayout.BeginHorizontal();
        //GUI.SetNextControlName("ChatText");
        //chatInput = GUILayout.TextField(chatInput, GUILayout.Width(400));

        //if (GUILayout.Button("Chat"))
        //{
        //    PostChat();
        //}
        //if (Event.current.type == EventType.KeyDown && Event.current.character == '\n')
        //{
        //    if (GUI.GetNameOfFocusedControl().Equals("ChatText"))
        //    {
        //        PostChat();
        //    }
        //    else
        //    {
        //        GUI.FocusControl("ChatText");
        //    }
        //}
        //GUILayout.EndArea();
    }

    public void PostChat()
    {
        _controller.ParseChat(chatInput);
        chatInput = "";
        GUIUtility.keyboardControl = 0;
    }
}
