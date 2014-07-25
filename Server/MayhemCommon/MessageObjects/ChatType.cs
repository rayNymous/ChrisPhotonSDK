using System;

namespace MayhemCommon.MessageObjects
{
    [Serializable]
    public enum ChatType
    {
        Local,
        Region,
        Guild,
        Group,
        General,
        Trade,
        Whisper
    }
}