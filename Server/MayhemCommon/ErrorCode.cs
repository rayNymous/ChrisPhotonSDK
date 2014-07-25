namespace MayhemCommon
{
    public enum ErrorCode : short
    {
        // GENERAL
        OperationDenied = -3,
        OperationInvalid = -2,
        InternalServerError = -1,

        OK = 0,

        // LOGIN
        UsernameInUse,
        IncorrectUsernameOrPassword,
        UserCurrentlyLoggedIn,
        InvalidCharacter
    }
}