using System;
using System.Collections.Generic;
using GameServer.Model.Interfaces;
using GameServer.Quests;
using MayhemCommon;

namespace GameServer.Data.Templates
{
    public class NpcTemplate
    {
        public NpcTemplate()
        {
            Values = new Dictionary<string, object>();
            QuestEvents = new Dictionary<QuestEventType, List<Quest>>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public RelationshipType Type { get; set; }
        public String Prefab { get; set; }
        public int RespawnTime { get; set; }
        public IEnumerable<ItemDrop> DropList { get; set; }
        public Dictionary<QuestEventType, List<Quest>> QuestEvents { get; protected set; }
        public Dictionary<string, object> Values { get; protected set; }
        public Dictionary<IStat, float> Stats { get; set; }

        public Type AiType { get; set; }
        public Dictionary<String, object> AiSettings { get; set; }
        public float WidthRadius { get; set; }

        public int CoinsReward { get; set; }

        public void AddQuestEvent(QuestEventType eventType, Quest quest)
        {
            List<Quest> questList;
            QuestEvents.TryGetValue(eventType, out questList);

            if (questList == null)
            {
                questList = new List<Quest>();
                QuestEvents.Add(eventType, questList);
            }

            if (!questList.Contains(quest))
            {
                questList.Add(quest);
            }
        }

        public List<Quest> GetQuests(QuestEventType eventType)
        {
            List<Quest> quests;
            QuestEvents.TryGetValue(eventType, out quests);
            return quests;
        }

        public T GetValue<T>(string key) where T : class
        {
            object result;
            Values.TryGetValue(key, out result);
            return result as T;
        }

        public int GetValueInt(string key, int defaultValue)
        {
            object result;
            Values.TryGetValue(key, out result);
            if (result == null)
            {
                return defaultValue;
            }
            return Convert.ToInt32(result);
        }
    }
}