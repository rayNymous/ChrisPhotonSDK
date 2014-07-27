using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using GameServer.Model;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;
using Photon.SocketServer;
using SubServerCommon.Data.ClientData;

namespace GameServer.Handlers
{
    public class ChatHandler : PhotonServerHandler
    {
        public ChatHandler(PhotonApplication application) : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Request; }
        }

        public override byte Code
        {
            get { return (byte) ClientOperationCode.Chat; }
        }

        public override int? SubCode
        {
            get { return (int) MessageSubCode.Chat; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            if (message.Parameters.ContainsKey((byte) ClientParameterCode.Object))
            {
                var peerId = new Guid((Byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);
                Dictionary<Guid, SubServerClientPeer> clients =
                    Server.ConnectionCollection<SubServerConnectionCollection>().Clients;
                var player = clients[peerId].ClientData<GPlayerInstance>();

                var messageText = (string) message.Parameters[(byte) ClientParameterCode.Object];

                Log.Debug("CHAT MESSAGE RECEIVED: " + messageText);

                if (messageText.StartsWith("/"))
                {
                    // Some sort of command
                }
                else
                {
                    // Local chat message
                    player.Say(messageText);
                }
            }
            return true;
        }
    }
}