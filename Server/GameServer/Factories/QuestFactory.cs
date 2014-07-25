using System.Collections.Generic;
using ExitGames.Logging;
using GameServer.Data;
using GameServer.Model.Interfaces;
using GameServer.Quests;

namespace GameServer.Factories
{
    public class QuestFactory : IFactory
    {
        protected ILogger Log = LogManager.GetCurrentClassLogger();

        public QuestFactory(NpcFactory npcFactory, IEnumerable<Quest> quests)
        {
            NpcFactory = npcFactory;

            Quests = new Dictionary<int, Quest>();

            foreach (Quest quest in quests)
            {
                if (!Quests.ContainsKey(quest.QuestId))
                {
                    Quests.Add(quest.QuestId, quest);
                    Log.DebugFormat("Quest '{0}' registered successfully.", quest.Name);
                }
                else
                {
                    Log.WarnFormat("Quest '{0}' could not be registered. Id {1} allready exists", quest.Name,
                        quest.QuestId);
                }
            }
        }

        public NpcFactory NpcFactory { get; protected set; }
        public Dictionary<int, Quest> Quests { get; set; }
    }
}