using System;
using System.Collections.Generic;
using System.Linq;
using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;
using NHibernate;
using SubServerCommon;
using SubServerCommon.Data.ClientData;
using SubServerCommon.Data.NHibernate;

namespace ChatServer.Handlers
{
    public class RegisterEventHandler : PhotonServerHandler
    {
        private readonly SubServerClientPeer.Factory clientFactory;

        public RegisterEventHandler(PhotonApplication application, SubServerClientPeer.Factory clientFactory)
            : base(application)
        {
            this.clientFactory = clientFactory;
        }

        public override MessageType Type
        {
            get { return MessageType.Async; }
        }

        public override byte Code
        {
            get { return (byte) ServerEventCode.CharacterRegister; }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var characterId = new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.CharacterId]);
            var peerId = new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]);
            try
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        MayhemCharacter character =
                            session.QueryOver<MayhemCharacter>().Where(c => c.Id == characterId).List().FirstOrDefault();
                        transaction.Commit();

                        if (character != null)
                        {
                            Dictionary<Guid, SubServerClientPeer> clients =
                                Server.ConnectionCollection<SubServerConnectionCollection>().Clients;

                            if (!clients.ContainsKey(peerId))
                            {
                                clients.Add(peerId, clientFactory(peerId));
                            }

                            // TODO Add character data to the client list for chat
                            clients[peerId].ClientData<ChatPlayer>().CharacterName = character.Name;
                            clients[peerId].ClientData<ServerData>().ServerPeer = serverPeer;
                            // Notify Guild members that someone logged in
                            // Notify friend list that someone logged in
                        }
                        else
                        {
                            Log.FatalFormat(
                                "ChatServerRegisterEventhandler: Should not reach - Character not found in database " +
                                characterId);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
            return true;
        }
    }
}