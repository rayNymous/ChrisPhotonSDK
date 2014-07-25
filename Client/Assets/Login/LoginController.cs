
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using ExitGames.Client.Photon;
using MayhemCommon;

public class LoginController : ViewController
{
    public LoginController(View controlledView, byte subOperationCode = 0) : base(controlledView, subOperationCode)
    {
        OperationHandlers.Add((byte)MessageSubCode.Login, new LoginHandler(this));
        OperationHandlers.Add((byte)MessageSubCode.Register, new RegisterHandler(this));
    }

    static Regex ValidEmailRegex = CreateValidEmailRegex();

    /// <summary>
    /// Taken from http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx
    /// </summary>
    /// <returns></returns>
    public static Regex CreateValidEmailRegex()
    {
        string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
            + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
            + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

        return new Regex(validEmailPattern, RegexOptions.IgnoreCase);
    }

    public static bool EmailIsValid(string emailAddress)
    {
        bool isValid = ValidEmailRegex.IsMatch(emailAddress);

        return isValid;
    }

    public void SendLogin(string username, string password)
    {
        var param = new Dictionary<byte, object>()
        {
            {(byte) ClientParameterCode.UserName, username},
            {(byte) ClientParameterCode.Password, password},
            {(byte) ClientParameterCode.SubOperationCode, (int) MessageSubCode.Login}
        };

        SendOperation(new OperationRequest() { OperationCode = (byte)ClientOperationCode.Login, Parameters = param }, true, 0, false);
    }

    public void SendRegister(string username, string password, string email)
    {
        var param = new Dictionary<byte, object>()
        {
            {(byte) ClientParameterCode.UserName, username},
            {(byte) ClientParameterCode.Password, password},
            {(byte) ClientParameterCode.Email, email},
            {(byte) ClientParameterCode.SubOperationCode, (int) MessageSubCode.Register}
        };
        
        SendOperation(new OperationRequest(){OperationCode = (byte)ClientOperationCode.Login, Parameters = param}, true, 0, false );

    }
}
