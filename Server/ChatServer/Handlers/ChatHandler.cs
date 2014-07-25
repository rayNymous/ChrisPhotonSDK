using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Server;
using Photon.SocketServer;
using SubServerCommon.Data.ClientData;

namespace ChatServer.Handlers
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
                var peerId = new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]);
                var mySerializer = new XmlSerializer(typeof (ChatItem));
                var inStream = new StringReader((string) message.Parameters[(byte) ClientParameterCode.Object]);
                var chatItem = (ChatItem) mySerializer.Deserialize(inStream);


                switch (chatItem.Type)
                {
                    case ChatType.General:
                        chatItem.Text = string.Format("[General] {0} : {1}",
                            Server.ConnectionCollection<SubServerConnectionCollection>().Clients[peerId]
                                .ClientData<ChatPlayer>().CharacterName, chatItem.Text);
                        var outStream = new StringWriter();

                        mySerializer.Serialize(outStream, chatItem);

                        foreach (var client in Server.ConnectionCollection<SubServerConnectionCollection>().Clients)
                        {
                            var para = new Dictionary<byte, object>
                            {
                                {(byte) ClientParameterCode.PeerId, client.Key.ToByteArray()},
                                {(byte) ClientParameterCode.Object, outStream.ToString()}
                            };
                            var eventData = new EventData {Code = (byte) ClientEventCode.Chat, Parameters = para};

                            client.Value.ClientData<ServerData>().ServerPeer.SendEvent(eventData, new SendParameters());
                        }
                        break;
                    default:
                        break;
                }
            }
            return true;
        }
    }
}