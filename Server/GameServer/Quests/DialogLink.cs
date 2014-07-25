using System;
using MayhemCommon;

namespace GameServer.Quests
{
    public class DialogLink
    {
        public DialogLinkType Type { get; set; }
        public int Target { get; set; }
        public String Text { get; set; }
    }
}