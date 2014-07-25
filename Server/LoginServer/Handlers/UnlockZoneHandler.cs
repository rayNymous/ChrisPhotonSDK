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
    public class UnlockZoneHandler : PhotonServerHandler
    {
        public UnlockZoneHandler(PhotonApplication application) : base(application)
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
            get { return (int) MessageSubCode.ZoneUnlock; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var operation = new ZoneUnlock(serverPeer.Protocol, message);

            var para = new Dictionary<byte, object>
            {
                {(byte) ClientParameterCode.PeerId, message.Parameters[(byte) ClientParameterCode.PeerId]},
                {
                    (byte) ClientParameterCode.SubOperationCode,
                    message.Parameters[(byte) ClientParameterCode.SubOperationCode]
                }
            };

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

                        UserZone userZone =
                            session.QueryOver<UserZone>()
                                .Where(c => c.User == user && c.Zone.Id == operation.ZoneId)
                                .SingleOrDefault();

                        MayhemZone zone =
                            session.QueryOver<MayhemZone>().Where(z => z.Id == operation.ZoneId).SingleOrDefault();

                        Log.Debug("About to unlock a zone " + (userZone == null) + " " + (zone != null) + " " +
                                  (user != null));

                        if (userZone == null && zone != null && user != null)
                        {
                            Log.Debug("Preparing to unlock a zone " + user.Coins + " > " + zone.Price);
                            if (user.Coins >= zone.Price)
                            {
                                Log.Debug("Very close to unlocking a zone");
                                user.Coins -= zone.Price;
                                var newUserZone = new UserZone
                                {
                                    Stars = 0,
                                    User = user,
                                    Zone = zone
                                };
                                session.SaveOrUpdate(user);
                                session.Save(newUserZone);

                                para.Add((byte) ClientParameterCode.ZoneId, zone.Id.ToByteArray());
                                para[(byte) ClientParameterCode.SubOperationCode] = MessageSubCode.ZoneItemUpdate;
                                para.Add((byte) ClientParameterCode.Object3, user.Coins);

                                // Send zone update
                                serverPeer.SendOperationResponse(new OperationResponse(message.Code)
                                {
                                    ReturnCode = (int) ErrorCode.OK,
                                    Parameters = para
                                }, new SendParameters());
                            }
                            else
                            {
                                para[(byte) ClientParameterCode.SubOperationCode] = (byte) MessageSubCode.Alert;
                                serverPeer.SendOperationResponse(new OperationResponse(message.Code)
                                {
                                    ReturnCode = (int) ErrorCode.InvalidCharacter,
                                    DebugMessage =
                                        "Not enough coins to unlock a zone (" + (zone.Price - user.Coins) + " short)",
                                    Parameters = para
                                }, new SendParameters());
                            }
                        }

                        transaction.Commit();
                    }
                }
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