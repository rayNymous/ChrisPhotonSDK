using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LoginServer.Operations;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Server;
using NHibernate;
using Photon.SocketServer;
using ProtoBuf;
using SubServerCommon;
using SubServerCommon.Data.NHibernate;

namespace LoginServer.Handlers
{
    internal class LoginServerListCharactersHandler : PhotonServerHandler
    {
        public LoginServerListCharactersHandler(PhotonApplication application) : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Request; }
        }

        public override byte Code
        {
            get { return (byte) ClientOperationCode.Login; }
        }

        public override int? SubCode
        {
            get { return (byte) MessageSubCode.ListCharacters; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var operation = new ListCharacters(serverPeer.Protocol, message);
            if (!operation.IsValid)
            {
                Log.DebugFormat("Invalid Operation - {0}", operation.GetErrorMessage());
                serverPeer.SendOperationResponse(
                    new OperationResponse(message.Code)
                    {
                        ReturnCode = (int) ErrorCode.OperationInvalid,
                        DebugMessage = operation.GetErrorMessage()
                    }, new SendParameters());
                return true;
            }

            try
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        User user =
                            session.QueryOver<User>().Where(u => u.Id == operation.UserId).List().FirstOrDefault();
                        if (user != null)
                        {
                            var para = new Dictionary<byte, object>
                            {
                                {(byte) ClientParameterCode.CharacterSlots, user.CharacterSlots},
                                {
                                    (byte) ClientParameterCode.PeerId,
                                    message.Parameters[(byte) ClientParameterCode.PeerId]
                                },
                                {
                                    (byte) ClientParameterCode.SubOperationCode,
                                    message.Parameters[(byte) ClientParameterCode.SubOperationCode]
                                }
                            };

                            IList<MayhemCharacter> characters =
                                session.QueryOver<MayhemCharacter>().Where(cc => cc.User == user).List();


                            var characterData = new CharacterListItem[characters.Count];


                            // Retrieving characters
                            for (int i = 0; i < characters.Count; i++)
                            {
                                characterData[i] = new CharacterListItem
                                {
                                    Deployed = characters[i].Zone != null,
                                    Fame = 777,
                                    Id = characters[i].Id,
                                    Sex = "Male",
                                    Name = characters[i].Name
                                };
                            }

                            // Retrieving zones
                            IList<UserZone> userZones =
                                session.QueryOver<UserZone>().Where(uz => uz.User.Id == user.Id).List();
                            IList<MayhemZone> zones = session.QueryOver<MayhemZone>().OrderBy(x => x.Order).Asc.List();
                                // TODO Don't show private zones

                            ZoneListItem[] zoneData = zones.Select(x =>
                            {
                                UserZone userZone = userZones.FirstOrDefault(uz => uz.Zone.Id == x.Id);

                                return new ZoneListItem
                                {
                                    CharacterName = null,
                                    InstanceId = x.Id,
                                    MaxPlayers = x.MaxPlayers,
                                    Name = x.Name,
                                    PlayersOnline = 0,
                                    Price = x.Price,
                                    Stars = userZone != null ? userZone.Stars : 0,
                                    Thumbnail = x.Thumbnail,
                                    Unlocked = userZone != null || x.Price == 0
                                };
                            }).ToArray();

                            var data = new CharacterSelectData
                            {
                                Characters = characterData,
                                Coins = user.Coins,
                                Zones = zoneData
                            };

                            byte[] b = null;
                            using (var ms = new MemoryStream())
                            {
                                Serializer.Serialize(ms, data);
                                b = new byte[ms.Position];
                                byte[] fullB = ms.GetBuffer();
                                Array.Copy(fullB, b, b.Length);
                            }

                            para.Add((byte) ClientParameterCode.CharacterList, b);

                            transaction.Commit();

                            serverPeer.SendOperationResponse(
                                new OperationResponse((byte) ClientOperationCode.Login) {Parameters = para},
                                new SendParameters());
                        }
                        else
                        {
                            serverPeer.SendOperationResponse(
                                new OperationResponse(message.Code)
                                {
                                    ReturnCode = (int) ErrorCode.OperationInvalid,
                                    DebugMessage = "User not found"
                                }, new SendParameters());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                serverPeer.SendOperationResponse(
                    new OperationResponse(message.Code)
                    {
                        ReturnCode = (int) ErrorCode.OperationInvalid,
                        DebugMessage = e.ToString()
                    }, new SendParameters());
            }
            return true;
        }
    }
}