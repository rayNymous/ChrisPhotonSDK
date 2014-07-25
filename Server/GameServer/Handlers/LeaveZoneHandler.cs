using System;
using System.Collections.Generic;
using GameServer.Model;
using GameServer.Model.ServerEvents;
using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;
using NHibernate;
using SubServerCommon;
using SubServerCommon.Data.NHibernate;

namespace GameServer.Handlers
{
    public class LeaveZoneHandler : GameRequestHandler
    {
        public LeaveZoneHandler(PhotonApplication application) : base(application)
        {
        }

        public override int? SubCode
        {
            get { return (int) MessageSubCode.LeaveZone; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var peerId = new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]);
            Dictionary<Guid, SubServerClientPeer> clients =
                Server.ConnectionCollection<SubServerConnectionCollection>().Clients;
            var instance = clients[peerId].ClientData<GPlayerInstance>();

            bool isLogOut = Convert.ToBoolean(message.Parameters[(byte) ClientParameterCode.Object]);

            try
            {
                instance.Deregister();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }


            if (!isLogOut)
            {
                try
                {
                    using (ISession session = NHibernateHelper.OpenSession())
                    {
                        using (ITransaction transaction = session.BeginTransaction())
                        {
                            MayhemCharacter character =
                                session.QueryOver<MayhemCharacter>()
                                    .Where(c => c.Id == instance.GlobalId)
                                    .SingleOrDefault();

                            if (character != null)
                            {
                                character.Zone = null;
                                character.Position = null;
                                session.SaveOrUpdate(character);
                            }
                            transaction.Commit();
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Debug(e);
                }
            }

            instance.SendPacket(new LeaveZone());
            return true;
        }
    }
}