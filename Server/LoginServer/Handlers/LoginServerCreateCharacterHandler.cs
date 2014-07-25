using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using LoginServer.Operations;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Server;
using NHibernate;
using Photon.SocketServer;
using SubServerCommon;
using SubServerCommon.Data.NHibernate;

namespace LoginServer.Handlers
{
    internal class LoginServerCreateCharacterHandler : PhotonServerHandler
    {
        public const int DefaultCharacterInventorySize = 16;

        public LoginServerCreateCharacterHandler(PhotonApplication application) : base(application)
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
            get { return (int) MessageSubCode.CreateCharacter; }
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

            var operation = new CreateCharacter(serverPeer.Protocol, message);

            if (!operation.IsValid)
            {
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

                        IList<MayhemCharacter> characters =
                            session.QueryOver<MayhemCharacter>().Where(c => c.User == user).List();

                        if (user != null && user.CharacterSlots <= characters.Count)
                        {
                            serverPeer.SendOperationResponse(
                                new OperationResponse(message.Code)
                                {
                                    ReturnCode = (int) ErrorCode.InvalidCharacter,
                                    DebugMessage = "No free character slots",
                                    Parameters = para
                                }, new SendParameters());
                        }
                        else
                        {
                            var mySerializer = new XmlSerializer(typeof (CharacterCreateDetails));
                            var reader = new StringReader(operation.CharacterCreateDetails);
                            var createCharacter = (CharacterCreateDetails) mySerializer.Deserialize(reader);

                            MayhemCharacter character =
                                session.QueryOver<MayhemCharacter>()
                                    .Where(c => c.Name == createCharacter.CharacterName)
                                    .List()
                                    .FirstOrDefault();

                            if (character != null)
                            {
                                transaction.Commit();
                                serverPeer.SendOperationResponse(
                                    new OperationResponse(message.Code)
                                    {
                                        ReturnCode = (int) ErrorCode.InvalidCharacter,
                                        DebugMessage = "Character name taken",
                                        Parameters = para
                                    }, new SendParameters());
                            }
                            else
                            {
                                var newChar = new MayhemCharacter
                                {
                                    User = user,
                                    Name = createCharacter.CharacterName,
                                    Inventory = new ItemContainer
                                    {
                                        Size = DefaultCharacterInventorySize,
                                        Type = ContainerType.Inventory
                                    },
                                    Sex = createCharacter.Sex,
                                    Fame = 0,
                                    Equipment = new ItemContainer
                                    {
                                        Size = 0,
                                        Type = ContainerType.Equipment
                                    }
                                };
                                session.Save(newChar);
                                transaction.Commit();
                                serverPeer.SendOperationResponse(
                                    new OperationResponse(message.Code)
                                    {
                                        ReturnCode = (int) ErrorCode.OK,
                                        Parameters = para
                                    }, new SendParameters());
                            }
                        }
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
                        ReturnCode = (int) ErrorCode.OperationInvalid,
                        DebugMessage = e.ToString(),
                        Parameters = para
                    }, new SendParameters());
            }
            return true;
        }
    }
}