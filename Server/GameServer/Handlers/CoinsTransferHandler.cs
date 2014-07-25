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
    public class CoinsTransferHandler : GameRequestHandler
    {
        public CoinsTransferHandler(PhotonApplication application) : base(application)
        {
        }

        public override int? SubCode
        {
            get { return (int) MessageSubCode.CoinsTransfer; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var peerId = new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]);
            Dictionary<Guid, SubServerClientPeer> clients =
                Server.ConnectionCollection<SubServerConnectionCollection>().Clients;
            var instance = clients[peerId].ClientData<GPlayerInstance>();

            try
            {
                int amount = Math.Abs(Convert.ToInt32((string) message.Parameters[(byte) ClientParameterCode.Object]));
                var withdraw = (bool) message.Parameters[(byte) ClientParameterCode.Object2];
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        MayhemCharacter character = session.QueryOver<MayhemCharacter>()
                            .Where(c => c.Id == instance.GlobalId).SingleOrDefault();

                        if (withdraw)
                        {
                            if (character.User.Coins >= amount)
                            {
                                character.User.Coins -= amount;
                                instance.AddCoins(amount);
                                session.SaveOrUpdate(character.User);
                            }
                        }
                        else
                        {
                            // Deposit
                            if (instance.Coins >= amount)
                            {
                                instance.AddCoins(-amount);
                                character.User.Coins += amount;
                                session.SaveOrUpdate(character.User);
                            }
                        }

                        instance.Storage.Coins = character.User.Coins;

                        instance.SendPacket(new PlayerStatusUpdate(new Dictionary<byte, object>
                        {
                            {(byte) PlayerStatusValue.GlobalCoins, character.User.Coins}
                        }));

                        transaction.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return true;
        }
    }
}