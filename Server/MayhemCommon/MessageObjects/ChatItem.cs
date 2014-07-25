using System;

namespace MayhemCommon.MessageObjects
{
    [Serializable]
    public class ChatItem
    {
        public string WhisperPlayer { get; set; }
        public string Text { get; set; }
        public ChatType Type { get; set; }
    }
}