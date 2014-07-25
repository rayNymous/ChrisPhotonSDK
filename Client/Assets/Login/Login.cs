using System;
using System.Reflection.Emit;
using UnityEngine;
using System.Collections;

public class Login : View
{
    public string ServerAddress = "localhost:5055";
    public string ApplicationName = "ComplexServer";

    public bool loggingIn = false;

    private string Username = "tttttt";
    private string Password = "tttttt";
    private string Email = "tttttt";

    public UIInput LoginUsername;
    public UIInput LoginPassword;

    public UIInput RegistrationUsername;
    public UIInput RegistrationPassword;
    public UIInput RegistrationEmail;

    public AlertGui Alert;

    public TweenAlpha RightAlphaAnimation;
    public TweenPosition RightPositionAnimation;

    public UILabel StatusLabel;
    public Color StatusOnlineColor = new Color(161/255f, 225/255f, 47/225f, 1);
    public Color StatusOfflineColor = new Color(1f, 84 / 255f, 84 / 225f, 1);
    public bool _lastStatusConnected = false;

    public override void Awake()
    {
    }

    // Use this for initialization
	void Start ()
	{
        Controller = new LoginController(this);
        PhotonEngine.UseExistingOrCreateNewPhotonEngine(ServerAddress, ApplicationName);

	    //string[] argList = new string[0];
	    //if (Application.srcValue.Split(new[] {"?"}, StringSplitOptions.RemoveEmptyEntries).Length > 1)
	    //{
	    //    argList = Application.srcValue.Split(new[] {"?"}, StringSplitOptions.RemoveEmptyEntries)[1].Split(new[] {","},
	    //        StringSplitOptions.RemoveEmptyEntries);
	    //}
	    //if (argList.Length == 2)
	    //{
	    //    controller.SendLogin(argList[0], argList[1]);
	    //    loggingIn = true;
	    //}
	}
	
	// Update is called once per frame
	void Update () {
	    if (!_lastStatusConnected && PhotonNetwork.connected)
	    {
	        StatusLabel.text = "Online";
	        StatusLabel.color = StatusOnlineColor;
	        _lastStatusConnected = true;
	    } else if (_lastStatusConnected && !PhotonNetwork.connected)
	    {
            StatusLabel.text = "Offline";
	        StatusLabel.color = StatusOfflineColor;
	    }
	}

    public void ExitGame()
    {
        Application.Quit();
    }

    void OnGUI()
    {
        //Username = GUI.TextField(new Rect(5, 5, 300, 30), Username, 64);
        //Password = GUI.TextField(new Rect(5, 40, 300, 30), Password, 64);
        //Email = GUI.TextField(new Rect(5, 75, 300, 30), Email, 64);
        //if (GUI.Button(new Rect(5, 110, 300, 30), "Register"))
        //{
        //    controller.SendRegister(Username, Password, Email);
        //}
    }

    public void SendLogin()
    {
        controller.SendLogin(LoginUsername.value, LoginPassword.value);
    }

    public void CloseRegistration()
    {
        NGUITools.SetActive(RightAlphaAnimation.gameObject, false);
        RightAlphaAnimation.ResetToBeginning();
        RightPositionAnimation.ResetToBeginning();
    }

    public void SendRegistration()
    {
        if (!LoginController.EmailIsValid(RegistrationEmail.value))
        {
            Alert.DisplayWarning("Invalid email address");
            return;
        }

        if (RegistrationUsername.value.Length < 5)
        {
            Alert.DisplayWarning("Username is too short. It has to be at least 5 characters long");
            return;
        }

        if (RegistrationPassword.value.Length < 4)
        {
            Alert.DisplayWarning("Password is too short. It has to contain at leas 4 letters");
            return;
        }

        controller.SendRegister(RegistrationUsername.value, RegistrationPassword.value, RegistrationEmail.value);
    }

    public void OnClickRegister()
    {
        NGUITools.SetActive(RightAlphaAnimation.gameObject, true);
        RightAlphaAnimation.PlayForward();
        RightPositionAnimation.PlayForward();
    }

    public void OnRegistrationSuccessful()
    {
        CloseRegistration();
        Alert.DisplayWarning("Registration Successful!");
        LoginUsername.value = RegistrationUsername.value;
        LoginPassword.value = "";
    }

    private LoginController controller;

    public override IViewController Controller {
        get { return (IViewController) controller; } 
        protected set { controller = value as LoginController; } 
    }
}
