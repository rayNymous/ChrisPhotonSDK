using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Logging;
using GameServer.Model;
using GameServer.Model.Interfaces;
using GameServer.Model.ServerEvents;
using SubServerCommon;

namespace GameServer.Quests
{
    public class QuestState
    {
        private readonly GPlayerInstance _player;

        private QuestProgressState _progress;

        public QuestState(Quest quest, GPlayerInstance player)
        {
            _player = player;
            Quest = quest;
            Values = new Dictionary<string, string>();
            _progress = QuestProgressState.Started;
            DatabaseId = -1;
            IsNew = true;
        }

        public int DatabaseId { get; set; }

        public bool IsNew { get; private set; }
        public bool IsUpdated { get; private set; }

        public int QuestId
        {
            get { return Quest.QuestId; }
        }

        public Quest Quest { get; protected set; }
        public Dictionary<string, string> Values { get; private set; }

        public QuestProgressState Progress
        {
            get { return _progress; }
        }

        public void OverrideProgress(QuestProgressState state)
        {
            _progress = state;
        }

        public void SetProgress(QuestProgressState state)
        {
            _progress = state;
            List<IObject> objects = _player.ZoneBlock.GetVisibleObjects().Where(x => x as GCharacter != null).ToList();
            // TODO filter out unrelated characters
            _player.SendPacket(new ObjectHintUpdate(objects));
            IsUpdated = true;
        }

        public void OverrideValues(Dictionary<string, string> values)
        {
            Values = values;
            IsNew = false; // ATTENTION. If there are values to override, the quest is not new, it's already loaded from a database
        }

        public String GetValue(string key)
        {
            String value;
            Values.TryGetValue(key, out value);
            return value;
        }

        public void SetValue(string key, string value)
        {
            if (Values.ContainsKey(key))
            {
                Values[key] = value;
            }
            else
            {
                Values.Add(key, value);
            }
            IsUpdated = true;
        }

        public int GetInt(string key, int defaultValue)
        {
            if (Values.ContainsKey(key))
            {
                return Convert.ToInt32(Values[key]);
            }
            return defaultValue;
        }

        public void SetInt(string key, int value)
        {
            SetValue(key, value + "");
        }
    }
}