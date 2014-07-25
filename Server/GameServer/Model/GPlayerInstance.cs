using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GameServer.Factories;
using GameServer.Model.Interfaces;
using GameServer.Model.Items;
using GameServer.Model.ServerEvents;
using GameServer.Model.Stats;
using GameServer.Quests;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using MayhemCommon.MessageObjects.Views;
using MMO.Framework;
using MMO.Photon.Client;
using MMO.Photon.Server;
using Newtonsoft.Json;
using NHibernate;
using Photon.SocketServer;
using SubServerCommon;
using SubServerCommon.Data.NHibernate;

namespace GameServer.Model
{
    public class GPlayerInstance : GCharacter, IPlayer, IClientData
    {
        private readonly Dictionary<int, QuestState> _quests;

        public Equipment Equipment;
        public Inventory Inventory;
        public GlobalStorage Storage;

        public GPlayerInstance(IWorld world, IStatHolder stats)
            : base(null, stats)
        {
            World = world;
            _quests = new Dictionary<int, QuestState>();
            Inventory = new Inventory(this);
            Equipment = new Equipment(this);
            Storage = new GlobalStorage(this);
        }

        public IWorld World { protected get; set; }

        public Dialog Dialog { get; set; }

        public int Coins { get; protected set; }

        public int Heat { get; set; }
        public SubServerClientPeer Client { get; set; }
        public PhotonServerPeer ServerPeer { get; set; }
        public Guid UserId { get; set; }
        public Guid CharacterId { get; set; }

        public override ObjectView GetObjectView(GPlayerInstance player)
        {
            return new PlayerView
            {
                Armor = 0,
                DataType = typeof (ObjectView).ToString(),
                Eyes = 0,
                HairStyle = 0,
                Helmet = 0,
                InstanceId = InstanceId,
                Name = Name,
                LeftHand = 0,
                ObjectType = ObjectType.Player,
                Prefab = "PCharacter",
                Position = new PositionData(Position.X, Position.Y, Position.Z)
            };
        }


        public override void SendPacket(ServerPacket packet)
        {
            if (packet.Parameters.ContainsKey((byte) ClientParameterCode.Invalid))
            {
                return;
            }
            if (Client != null && !(packet.IgnoreSelf && packet.Player == this))
            {
                packet.AddParameter(Client.PeerId.ToByteArray(), ClientParameterCode.PeerId);
                packet.LateInitialization(this);
                ServerPeer.SendEvent(new EventData(packet.Code, packet.Parameters), new SendParameters());
            }
        }

        public override bool Die(ICharacter killer)
        {
            bool die = base.Die(killer);
            DeleteFromDatabase();
            SendPacket(new DeathNotification());
            return die;
        }

        public override RelationshipType RelationshipWith(ICharacter character)
        {
            if (character as GPlayerInstance == null)
            {
                return character.RelationshipWith(this);
            }
            return RelationshipType.Friendly;
        }

        public void OffsetHeat(int offset)
        {
            int oldHeat = Heat;

            if (offset < 0)
            {
                offset = Math.Min(0, offset += (int) Stats.GetStat<FrostResistance>());
            }

            Heat += offset;

            if (Heat <= 0)
            {
                Heat = 0;
                OffsetHealth(oldHeat + offset, null, true);
            }

            if (Heat > Stats.GetStat<MaxHeat>())
            {
                Heat = (int) Stats.GetStat<MaxHeat>();
            }

            if (oldHeat != Heat)
            {
                SendPacket(new PlayerStatusUpdate(new Dictionary<byte, object>
                {
                    {(byte) PlayerStatusValue.Heat, Heat}
                }));
            }
        }

        public void UpdatePosition(PositionData data)
        {
            Position.XYZ(data.X, data.Y, data.Z);
            Zone.OnCharacterPositionChange(this);
            BroadcastMessage(new MoveTo(this, new MoveToData
            {
                Destination = Position,
                Speed = 3f
            }, this));
            StopFollowing();
            StopAutoAttack();
            Zone.RemoveMovingCharacter(this);
        }

        public void AddCoins(int coins)
        {
            Coins += coins;
            SendPacket(new PlayerStatusUpdate(new Dictionary<byte, object>
            {
                {(byte) PlayerStatusValue.Coins, Coins}
            }));
        }

        public void UnlockZoneStar(Guid zoneId, StarCode star)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    User user =
                            session.QueryOver<User>().Where(u => u.Id == UserId).List().FirstOrDefault();

                    MayhemZone zone =
                            session.QueryOver<MayhemZone>().Where(z => z.Id == zoneId).SingleOrDefault();

                    UserZone userZone =
                           session.QueryOver<UserZone>()
                               .Where(c => c.User == user && c.Zone == zone)
                               .SingleOrDefault();



                    Log.Debug("WTF1: " + session.QueryOver<UserZone>().Where(c => c.User == user).List().Count);
                    Log.Debug("WTF2: " + session.QueryOver<UserZone>().Where(c => c.User == user && c.Zone == zone).List().Count);
                    Log.Debug("WTF3: " + session.QueryOver<UserZone>().Where(c => c.User.Id == UserId).List().Count);
                    Log.Debug("WTF4: " + session.QueryOver<UserZone>().Where(c => c.User.Id == UserId && c.Zone.Id == zoneId).List().Count);

                    Log.Debug("Almost found it + " + UserId + " ||||| " + zoneId);

                    if (userZone != null)
                    {
                        
                        userZone.Stars |= (byte) star;
                        Log.Debug("WE FOUND THE ZONE! " + star);
                        session.SaveOrUpdate(userZone);
                    } else if (zone.Price == 0)
                    {
                        session.Save(new UserZone()
                        {
                            Stars = 0 | (byte)star,
                            User = user,
                            Zone = zone
                        });
                    }

                    transaction.Commit();
                }

            }
        }

        public QuestState GetQuestState(Quest quest)
        {
            QuestState state;
            _quests.TryGetValue(quest.QuestId, out state);
            return state;
        }

        public QuestState GetQuestState(int questId)
        {
            QuestState state;
            _quests.TryGetValue(questId, out state);
            return state;
        }

        public void AddQuestState(QuestState state)
        {
            Log.Debug("Quest state added");
            _quests.Add(state.QuestId, state);
        }

        public override void Deregister()
        {
            Log.Debug("DEREGISTER PLAYER");
            World.RemovePlayer(this);
            CleanUp();
            Store();
            base.Deregister();
        }

        public override GContainer GenerateLoot()
        {
            return null;
        }

        public override bool Restore(Guid characterId)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    MayhemCharacter character =
                        session.QueryOver<MayhemCharacter>().Where(c => c.Id == characterId).List().FirstOrDefault();

                    var questStates = session.QueryOver<MayhemQuest>().Where(q => q.UserId == character.User.Id).List();

                    transaction.Commit();

                    if (character != null)
                    {
                        GlobalId = characterId;
                        Name = character.Name;

                        // Attaching a zone to the player
                        if (character.Zone != null && World.GetZone(character.Zone.Id) != null)
                        {
                            // Character was previously in a zone
                            Zone = World.GetZone(character.Zone.Id);
                        }
                        else
                        {
                            // New character in a new zone
                            // Find a way to set the zone, or return if failed
                            Zone = World.GetZone(Guid.Empty);
                        }

                        if (Zone == null)
                        {
                            Log.Debug("Restoring a character failed. Failed to attach the zone");
                            return false;
                        }

                        // Setting a position of a player
                        if (!String.IsNullOrEmpty(character.Position))
                        {
                            Position = JsonConvert.DeserializeObject<Position>(character.Position);
                            Log.Debug("Character position: " + Position);
                        }
                        else
                        {
                            Position = new Position(Zone.StartPosition().X, Zone.StartPosition().Y, 0);
                        }
                        Coins = character.Coins;

                        // Quests
                        if (questStates != null)
                        {
                            var factory = Zone.World.GetFactory<QuestFactory>();
                            foreach (var mayhemQuest in questStates)
                            {
                                var newQuestState = new QuestState(factory.Quests[mayhemQuest.QuestId], this);
                                newQuestState.OverrideProgress(mayhemQuest.Progress);
                                newQuestState.DatabaseId = mayhemQuest.Id;
                                var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(mayhemQuest.Data);
                                if (values != null)
                                {
                                    newQuestState.OverrideValues(values);
                                }
                                AddQuestState(newQuestState);
                            }
                        }


                        // Appearance
                        // Level
                        // Stats
                        // Position

                        Storage.RestoreFromDatabase();
                        Inventory.RestoreFromDatabase();
                        Equipment.RestoreFromDatabase();

                        // Equipment
                        // Inventory
                        // Effects

                        // Social - guild notification, group notification and etc.
                    }
                    else
                    {
                        Log.FatalFormat("GameRegisterEventhandler: Should not reach - Character not found in database");
                        return false;
                    }
                }
            }
            return true;
        }

        public override void CleanUp()
        {
            base.CleanUp();

            // Remove temp items

            // Remove from instances
            // Stop crafting

            // Decay from server;
            Decay();

            // Remove from all groups
            // Unsummon pets
            // Notify guild of logoff
            // Cancel trading

            // Notify firend list of logoff
        }

        public void Store()
        {
            Log.Debug("STORING CHARACTER DATA");
            // Save character to DB
            try
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        User user = session.QueryOver<User>().Where(u => u.Id == UserId).SingleOrDefault();
                        MayhemCharacter character = session.QueryOver<MayhemCharacter>()
                            .Where(cc => cc.User == user && cc.Name == Name)
                            .SingleOrDefault();
                        character.Coins = Coins;
                        character.Position = JsonConvert.SerializeObject(Position);
                        
                        session.Persist(character);

                        foreach (var questState in _quests)
                        {
                            var state = questState.Value;
                            if (state.IsNew)
                            {
                                session.Save(new MayhemQuest
                                {
                                    Data = JsonConvert.SerializeObject(state.Values),
                                    Progress = state.Progress,
                                    QuestId = state.Quest.QuestId,
                                    UserId = UserId
                                });
                            }
                            else
                            {
                                session.Update(new MayhemQuest
                                {
                                    Data = JsonConvert.SerializeObject(state.Values),
                                    Progress = state.Progress,
                                    QuestId = state.Quest.QuestId,
                                    UserId = UserId,
                                    Id = state.DatabaseId
                                });
                            }
                        }

                        transaction.Commit();
                        //character.Level = (int) Stats.GetStat<Level>();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private void DeleteFromDatabase()
        {
            try
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        var character = session.Get<MayhemCharacter>(GlobalId);

                        if (character != null)
                        {
                            session.Delete(character);
                        }
                        else
                        {
                            Log.Error("Tried to delete a null character");
                        }

                        transaction.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}