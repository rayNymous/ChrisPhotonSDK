using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using LoginServer.Operations;
using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;
using NHibernate;
using Photon.SocketServer;
using SubServerCommon;
using SubServerCommon.Data.ClientData;
using SubServerCommon.Data.NHibernate;

namespace LoginServer.Handlers
{
    public class ServerLoginRequestHandler : PhotonServerHandler
    {
        private readonly SubServerClientPeer.Factory _clientFactory;

        public ServerLoginRequestHandler(PhotonApplication application, SubServerClientPeer.Factory clientFactory)
            : base(application)
        {
            this._clientFactory = clientFactory;
        }

        public override MessageType Type
        {
            get { return MessageType.Request; }
        }

        public override byte Code
        {
            get { return (byte) ClientOperationCode.Game; }
        }

        public override int? SubCode
        {
            get { return (int) MessageSubCode.Login; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var operation = new LoginSecurely(serverPeer.Protocol, message);
            if (!operation.IsValid)
            {
                serverPeer.SendOperationResponse(
                    new OperationResponse(message.Code,
                        new Dictionary<byte, object>
                        {
                            {(byte) ClientParameterCode.PeerId, message.Parameters[(byte) ClientParameterCode.PeerId]}
                        })
                    {
                        ReturnCode = (int) ErrorCode.IncorrectUsernameOrPassword,
                        DebugMessage = "Invalid operation. Username or password is incorrect."
                    }, new SendParameters());
                return true;
            }

            if (operation.UserName == "" || operation.Password == "")
            {
                serverPeer.SendOperationResponse(new OperationResponse(message.Code, new Dictionary<byte, object>())
                {
                    ReturnCode = (int) ErrorCode.IncorrectUsernameOrPassword,
                    DebugMessage = "Username or password is incorrect (empty)"
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
                            User user = userList[0];

                            string hash = BitConverter.ToString(SHA1.Create().ComputeHash(
                                Encoding.UTF8.GetBytes(user.Salt + operation.Password))).Replace("-", "");

                            if (String.Equals(hash.Trim(), user.Password.Trim(), StringComparison.OrdinalIgnoreCase))
                            {
                                var server = Server as LoginServer;
                                if (server != null)
                                {
                                    bool foundUser = false;
                                    foreach (
                                        var subServerClientPeer in
                                            server.ConnectionCollection<SubServerConnectionCollection>().Clients)
                                    {
                                        if (subServerClientPeer.Value.ClientData<UserData>().UserId == user.Id)
                                        {
                                            foundUser = true;
                                        }
                                    }

                                    if (foundUser)
                                    {
                                        var para = new Dictionary<byte, object>
                                        {
                                            {
                                                (byte) ClientParameterCode.PeerId,
                                                message.Parameters[(byte) ClientParameterCode.PeerId]
                                            },
                                            {
                                                (byte) ClientParameterCode.SubOperationCode,
                                                message.Parameters[(byte) ClientParameterCode.SubOperationCode]
                                            },
                                        };

                                        serverPeer.SendOperationResponse(
                                            new OperationResponse((byte) ClientOperationCode.Login)
                                            {
                                                Parameters = para,
                                                ReturnCode = (short) ErrorCode.UserCurrentlyLoggedIn,
                                                DebugMessage = "User is currently logged in"
                                            },
                                            new SendParameters());
                                    }
                                    else
                                    {
                                        server.ConnectionCollection<SubServerConnectionCollection>().Clients.Add(
                                            new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]),
                                            _clientFactory(
                                                new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId])));

                                        server.ConnectionCollection<SubServerConnectionCollection>().Clients[
                                            new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId])]
                                            .ClientData<UserData>().UserId = user.Id;


                                        Log.Debug("Login Handler successfully found user to log in.");

                                        var para = new Dictionary<byte, object>
                                        {
                                            {
                                                (byte) ClientParameterCode.PeerId,
                                                message.Parameters[(byte) ClientParameterCode.PeerId]
                                            },
                                            {
                                                (byte) ClientParameterCode.SubOperationCode,
                                                message.Parameters[(byte) ClientParameterCode.SubOperationCode]
                                            },
                                            {(byte) ClientParameterCode.UserId, user.Id.ToByteArray()}
                                        };

                                        serverPeer.SendOperationResponse(
                                            new OperationResponse((byte) ClientOperationCode.Login) {Parameters = para},
                                            new SendParameters());
                                    }
                                }
                                transaction.Commit();
                                return true;
                            }
                            serverPeer.SendOperationResponse(new OperationResponse(message.Code)
                            {
                                ReturnCode = (int) ErrorCode.IncorrectUsernameOrPassword,
                                DebugMessage = "Username or password is incorrect (1)",
                                Parameters = new Dictionary<byte, object>
                                {
                                    {
                                        (byte) ClientParameterCode.PeerId,
                                        message.Parameters[(byte) ClientParameterCode.PeerId]
                                    },
                                    {
                                        (byte) ClientParameterCode.SubOperationCode,
                                        message.Parameters[(byte) ClientParameterCode.SubOperationCode]
                                    }
                                }
                            },
                                new SendParameters());
                            transaction.Commit();
                            return true;
                        }
                        Log.DebugFormat("Account name does not exist {0}", operation.UserName);
                        transaction.Commit();
                        serverPeer.SendOperationResponse(
                            new OperationResponse(message.Code)
                            {
                                ReturnCode = (int) ErrorCode.IncorrectUsernameOrPassword,
                                DebugMessage = "Username or password is incorrect (2)",
                                Parameters = new Dictionary<byte, object>
                                {
                                    {
                                        (byte) ClientParameterCode.PeerId,
                                        message.Parameters[(byte) ClientParameterCode.PeerId]
                                    },
                                    {
                                        (byte) ClientParameterCode.SubOperationCode,
                                        message.Parameters[(byte) ClientParameterCode.SubOperationCode]
                                    }
                                }
                            }, new SendParameters());
                        return true;
                    }
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
                            }
                        })
                    {
                        ReturnCode = (int) ErrorCode.UsernameInUse,
                        DebugMessage = e.ToString()
                    }, new SendParameters());
            }
            return true;
        }
    }
}