using System;
using System.Collections.Generic;
using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;
using Photon.SocketServer;
using SubServerCommon;
using SubServerCommon.Data.ClientData;

namespace ProxyServer.Handlers
{
    public class SelectCharacterResponseHandler : PhotonServerHandler
    {
        public SelectCharacterResponseHandler(PhotonApplication application) : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Response; }
        }

        public override byte Code
        {
            get { return (byte) ClientOperationCode.Login; }
        }

        public override int? SubCode
        {
            get { return (byte) MessageSubCode.SelectCharacter; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            if (message.Parameters.ContainsKey((byte) ClientParameterCode.PeerId))
            {
                PhotonClientPeer peer;
                Server.ConnectionCollection<ProxyConnectionCollection>()
                    .Clients.TryGetValue(new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]),
                        out peer);

                if (peer != null)
                {
                    var characterId = new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.CharacterId]);

                    peer.ClientData<UserData>().CharacterId = characterId;

                    var para = new Dictionary<byte, object>
                    {
                        {(byte) ClientParameterCode.CharacterId, characterId.ToByteArray()},
                        {
                            (byte) ClientParameterCode.PeerId,
                            message.Parameters[(byte) ClientParameterCode.PeerId]
                        },
                        {(byte) ClientParameterCode.UserId, peer.ClientData<UserData>().UserId.ToByteArray()},
                        {(byte) ClientParameterCode.ZoneId, message.Parameters[(byte) ClientParameterCode.ZoneId]},
                    };

                    PhotonServerPeer chatServer =
                        Server.ConnectionCollection<ProxyConnectionCollection>()
                            .OnGetServerByType((int) ServerType.Chat);
                    if (chatServer != null)
                    {
                        chatServer.SendEvent(
                            new EventData((byte) ServerEventCode.CharacterRegister) {Parameters = para},
                            new SendParameters());
                    }

                    PhotonServerPeer gameServer =
                        Server.ConnectionCollection<ProxyConnectionCollection>()
                            .OnGetServerByType((int) ServerType.Game);
                    if (gameServer != null)
                    {
                        peer.CurrentServer = gameServer;
                        gameServer.SendEvent(
                            new EventData((byte) ServerEventCode.CharacterRegister) {Parameters = para},
                            new SendParameters());
                    }

                    message.Parameters.Remove((byte) ClientParameterCode.PeerId);
                    message.Parameters.Remove((byte) ClientParameterCode.UserId);
                    message.Parameters.Remove((byte) ClientParameterCode.CharacterId);

                    var response = message as PhotonResponse;

                    if (response != null)
                    {
                        peer.SendOperationResponse(
                            new OperationResponse(response.Code)
                            {
                                Parameters = message.Parameters,
                                DebugMessage = response.DebugMessage,
                                ReturnCode = response.ReturnCode
                            }, new SendParameters());
                    }
                    else
                    {
                        peer.SendOperationResponse(new OperationResponse(message.Code, message.Parameters),
                            new SendParameters());
                    }
                }
            }
            return true;
        }
    }
}