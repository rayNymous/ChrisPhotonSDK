using System;

namespace GameServer.Quests
{
    public class Dialog
    {
        public int Id { get; set; }
        public DialogPage[] Pages { get; set; }
        public Quest Quest { get; set; }

        public Boolean IsQuestDialog
        {
            get { return Quest != null; }
        }

        public String Title { get; set; }
    }
}