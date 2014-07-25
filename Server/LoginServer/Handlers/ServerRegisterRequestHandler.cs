using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
    public class ServerRegisterRequestHandler : PhotonServerHandler
    {
        public const int DefaultInventorySize = 8;
        public const int DefaultCharacterSlots = 3;

        public ServerRegisterRequestHandler(PhotonApplication application)
            : base(application)
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
            get { return (int) MessageSubCode.Register; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var operation = new RegisterSecurely(serverPeer.Protocol, message);
            if (!operation.IsValid)
            {
                serverPeer.SendOperationResponse(
                    new OperationResponse(message.Code,
                        new Dictionary<byte, object>
                        {
                            {(byte) ClientParameterCode.PeerId, message.Parameters[(byte) ClientParameterCode.PeerId]},
                            {
                                (byte) ClientParameterCode.SubOperationCode,
                                message.Parameters[(byte) ClientParameterCode.SubOperationCode]
                            },
                        })
                    {
                        ReturnCode = (int) ErrorCode.OperationInvalid,
                        DebugMessage = operation.GetErrorMessage()
                    }, new SendParameters());
                return true;
            }

            if (!IsViableUsername(operation.UserName) || !IsViableEmail(operation.Email) ||
                !IsViablePassword(operation.Password))
            {
                serverPeer.SendOperationResponse(new OperationResponse(message.Code, new Dictionary<byte, object>())
                {
                    ReturnCode = (int) ErrorCode.OperationInvalid,
                    DebugMessage = "One of the fields is invalid " + IsViableUsername(operation.UserName) + " "
                                   + IsViableEmail(operation.Email) + " " + IsViablePassword(operation.Password),
                    Parameters = new Dictionary<byte, object>
                    {
                        {(byte) ClientParameterCode.PeerId, message.Parameters[(byte) ClientParameterCode.PeerId]},
                        {
                            (byte) ClientParameterCode.SubOperationCode,
                            message.Parameters[(byte) ClientParameterCode.SubOperationCode]
                        },
                    },
                },
                    new SendParameters());
                return true;
            }

            try
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        Log.Debug("About to look for user account");
                        IList<User> userList =
                            session.QueryOver<User>().Where(u => u.Username == operation.UserName).List();
                        if (userList.Count > 0)
                        {
                            Log.DebugFormat("Found account name already in use");
                            transaction.Commit();
                            serverPeer.SendOperationResponse(
                                new OperationResponse(message.Code)
                                {
                                    ReturnCode = (int) ErrorCode.UsernameInUse,
                                    DebugMessage = "Account name already in use, please use another",
                                    Parameters = new Dictionary<byte, object>
                                    {
                                        {
                                            (byte) ClientParameterCode.PeerId,
                                            message.Parameters[(byte) ClientParameterCode.PeerId]
                                        },
                                        {
                                            (byte) ClientParameterCode.SubOperationCode,
                                            message.Parameters[(byte) ClientParameterCode.SubOperationCode]
                                        },
                                    },
                                }, new SendParameters());
                            return true;
                        }
                        string salt = Guid.NewGuid().ToString().Replace("-", "");
                        Log.DebugFormat("Created salt {0}", salt);
                        var newUser = new User
                        {
                            Id = new Guid(),
                            Email = operation.Email,
                            Username = operation.UserName,
                            Password = BitConverter.ToString(SHA1.Create().ComputeHash(
                                Encoding.UTF8.GetBytes(salt + operation.Password))).Replace("-", ""),
                            Salt = salt,
                            Created = DateTime.Now,
                            Edited = DateTime.Now,
                            Inventory =
                                new ItemContainer
                                {
                                    Id = new Guid(),
                                    Size = DefaultInventorySize,
                                    Type = ContainerType.GlobalStorage
                                },
                            CharacterSlots = DefaultCharacterSlots,
                        };

                        Log.DebugFormat("Built user object");
                        session.Save(newUser);
                        Log.DebugFormat("Saved new user");
                        transaction.Commit();
                    }
                    //using (var transaction = session.BeginTransaction())
                    //{
                    //    Log.DebugFormat("Looking up newly created user");
                    //    var userList = session.QueryOver<User>().Where(u => u.Username == operation.UserName).List();
                    //    if (userList.Count > 0)
                    //    {
                    //        Log.DebugFormat("Creating Profile");
                    //        UserProfile profile = new UserProfile() {CharacterSlots = 1, User = userList[0]};
                    //        session.Save(profile);
                    //        Log.DebugFormat("Saved profile");
                    //        transaction.Commit();
                    //    }
                    //}
                    serverPeer.SendOperationResponse(
                        new OperationResponse(message.Code)
                        {
                            ReturnCode = (byte) ClientReturnCode.UserCreated,
                            Parameters = new Dictionary<byte, object>
                            {
                                {
                                    (byte) ClientParameterCode.PeerId,
                                    message.Parameters[(byte) ClientParameterCode.PeerId]
                                },
                                {
                                    (byte) ClientParameterCode.SubOperationCode,
                                    message.Parameters[(byte) ClientParameterCode.SubOperationCode]
                                },
                            }
                        },
                        new SendParameters());
                }
            }
            catch (Exception e)
            {
                Log.Error("Error Occured", e);
                serverPeer.SendOperationResponse(
                    new OperationResponse(message.Code,
                        new Dictionary<byte, object>
                        {
                            {(byte) ClientParameterCode.PeerId, message.Parameters[(byte) ClientParameterCode.PeerId]},
                            {
                                (byte) ClientParameterCode.SubOperationCode,
                                message.Parameters[(byte) ClientParameterCode.SubOperationCode]
                            },
                        })
                    {
                        ReturnCode = (int) ErrorCode.UsernameInUse,
                        DebugMessage = e.ToString()
                    }, new SendParameters());
            }
            return true;
        }

        public bool IsViableEmail(String email)
        {
            return true;
        }

        public bool IsViablePassword(String password)
        {
            if (password.Length < 6)
            {
                return false;
            }
            return true;
        }

        public bool IsViableUsername(String username)
        {
            if (username.Length < 5)
            {
                return false;
            }

            string cleaned = RemoveSpecialCharacters(username);

            if (!cleaned.Equals(username, StringComparison.Ordinal))
            {
                return false;
            }

            return true;
        }

        public static string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9]+", "", RegexOptions.Compiled);
        }
    }
}