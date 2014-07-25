using System;
using System.Collections.Generic;
using System.Linq;
using LoginServer.Operations;
using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Server;
using NHibernate;
using Photon.SocketServer;
using SubServerCommon;
using SubServerCommon.Data.NHibernate;

namespace LoginServer.Handlers
{
    internal class LoginServerSelectCharacterHandler : PhotonServerHandler
    {
        public LoginServerSelectCharacterHandler(PhotonApplication application) : base(application)
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
            get { return (int) MessageSubCode.SelectCharacter; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var para = new Dictionary<byte, object>
            {
                {(byte) ClientParameterCode.PeerId, message.Parameters[(byte) ClientParameterCode.PeerId]},
                {
                    (byte) ClientParameterCode.SubOperationCode,
                    message.Parameters[(byte) ClientParameterCode.SubOperationCode]
                }
            };

            var operation = new SelectCharacter(serverPeer.Protocol, message);

            if (!operation.IsValid)
            {
                Log.Error(operation.GetErrorMessage());
                serverPeer.SendOperationResponse(
                    new OperationResponse(message.Code)
                    {
                        ReturnCode = (int) ErrorCode.OperationInvalid,
                        DebugMessage = operation.GetErrorMessage(),
                        Parameters = para
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
                            Log.DebugFormat("Found user {0}", user.Username);
                        }

                        MayhemCharacter character =
                            session.QueryOver<MayhemCharacter>().Where(c => c.User == user)
                                .And(c => c.Id == operation.CharacterId).SingleOrDefault();

                        if (character == null)
                        {
                            serverPeer.SendOperationResponse(
                                new OperationResponse(message.Code)
                                {
                                    ReturnCode = (int) ErrorCode.InvalidCharacter,
                                    DebugMessage = "Invalid character",
                                    Parameters = para
                                }, new SendParameters());
                        }
                        else
                        {
                            Log.DebugFormat("Found Character {0}", character.Name);

                            if (character.Zone == null)
                            {
                                // Character is not deployed
                                MayhemZone zone =
                                    session.QueryOver<MayhemZone>()
                                        .Where(z => z.Id == operation.ZoneId)
                                        .SingleOrDefault();
                                Log.Debug("VAAA__SS_S_S_S_S Requested" + operation.ZoneId);
                                Log.Debug("VAAA__SS_S_S_S_S Got:" + zone.Id);
                                character.Zone = zone;
                                session.SaveOrUpdate(character);
                                para.Add((byte) ClientParameterCode.CharacterId, character.Id.ToByteArray());
                                para.Add((byte) ClientParameterCode.ZoneId, zone.Id.ToByteArray());
                                serverPeer.SendOperationResponse(
                                    new OperationResponse(message.Code)
                                    {
                                        Parameters = para,
                                        ReturnCode = (int) ErrorCode.OK
                                    },
                                    new SendParameters());
                            }
                            else if (character.Zone != null && character.Zone.Id == operation.ZoneId)
                            {
                                // Jumping to the zone where a character is deployed
                                para.Add((byte) ClientParameterCode.CharacterId, character.Id.ToByteArray());
                                para.Add((byte) ClientParameterCode.ZoneId, character.Zone.Id.ToByteArray());
                                serverPeer.SendOperationResponse(
                                    new OperationResponse(message.Code)
                                    {
                                        Parameters = para,
                                        ReturnCode = (int) ErrorCode.OK
                                    },
                                    new SendParameters());
                            }
                            else
                            {
                                para[(byte) ClientParameterCode.SubOperationCode] = (byte) MessageSubCode.Alert;
                                serverPeer.SendOperationResponse(new OperationResponse(message.Code)
                                {
                                    ReturnCode = (int) ErrorCode.InvalidCharacter,
                                    DebugMessage = "Character " + character.Name + " is deployed somewhere else",
                                    Parameters = para
                                }, new SendParameters());

                                // Character is deployed somewhere else
                                Log.Debug("Character " + character.Name + " is deployed somewhere else");
                            }

                            Log.Debug("Login response REALLY SENT");
                        }
                        transaction.Commit();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e);
                serverPeer.SendOperationResponse(
                    new OperationResponse(message.Code)
                    {
                        ReturnCode = (int) ErrorCode.InvalidCharacter,
                        DebugMessage = e.ToString(),
                        Parameters = para
                    }, new SendParameters());
            }
            return true;
        }
    }
}