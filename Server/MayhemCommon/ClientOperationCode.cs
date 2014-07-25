using System;

namespace MayhemCommon
{
    /// <summary>
    ///     Represents a code, used to detech which server we are communicating with
    /// </summary>
    [Flags]
    public enum ClientOperationCode
    {
        Chat = 0x1,
        Login = 0x2,
        Game = 0x4
    }
}