using System;
using ExitGames.Logging;
using GameServer.Model;
using GameServer.Model.Interfaces;
using MayhemCommon;

namespace GameServer
{
    public class GameActions
    {
        public static ILogger Log = LogManager.GetCurrentClassLogger();
        private static readonly GameAction[] Empty = {};

        /// <summary>
        ///     Not attackable
        /// </summary>
        private static readonly GameAction[] FriendlyNpc =
        {
            GameAction.Talk,
            GameAction.Inspect
        };

        /// <summary>
        ///     Not aggressive but still attackable
        /// </summary>
        private static readonly GameAction[] NeutralNpc =
        {
            GameAction.Attack
        };

        /// <summary>
        ///     Aggressive
        /// </summary>
        private static readonly GameAction[] HostileNpc =
        {
            GameAction.Attack,
            GameAction.Inspect
        };

        private static readonly GameAction[] Storage =
        {
            GameAction.Storage
        };

        /// <summary>
        ///     Dead npc that is capable of being looted
        /// </summary>
        private static readonly GameAction[] DeadNpcLoot =
        {
            GameAction.Loot
        };

        public static GameAction[] GetActions(IPlayer player, IObject obj)
        {
            if (obj.Name.Equals("Storage"))
            {
                Log.Debug("Accessing Storage");
                return Storage;
            }

            var character = obj as GCharacter;
            if (character != null)
            {
                if (character.IsDead)
                {
                    if (character.Killer == player && character.Loot != null)
                    {
                        return DeadNpcLoot;
                    }
                    return Empty;
                }

                RelationshipType relationship = player.RelationshipWith(character);

                switch (relationship)
                {
                    case RelationshipType.Friendly:
                        return FriendlyNpc;
                    case RelationshipType.Neutral:
                        return NeutralNpc;
                    case RelationshipType.Aggressive:
                        return HostileNpc;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return HostileNpc;
        }

        public static String[] ActionsToStringArray(GameAction[] actions)
        {
            var strActions = new string[actions.Length];

            for (int i = 0; i < actions.Length; i++)
            {
                strActions[i] = actions[i].ToString();
            }
            return strActions;
        }
    }
}