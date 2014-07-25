using System;
using System.Collections.Generic;
using System.Linq;
using MayhemCommon;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;
using Photon.SocketServer;
using SubServerCommon;
using SubServerCommon.Data.ClientData;

namespace ProxyServer
{
    internal class ProxyConnectionCollection : PhotonConnectionCollection
    {
        public ProxyConnectionCollection()
        {
            LoginServer = null;
            GameServer = null;
            ChatServer = null;
        }

        public PhotonServerPeer LoginServer { get; set; }
        public PhotonServerPeer GameServer { get; set; }
        public PhotonServerPeer ChatServer { get; set; }

        public override void Disconnect(PhotonServerPeer serverPeer)
        {
            if (serverPeer.ServerId.HasValue)
            {
                if (LoginServer != null && LoginServer.ServerId == serverPeer.ServerId)
                {
                    LoginServer = null;
                }
                if (GameServer != null && GameServer.ServerId == serverPeer.ServerId)
                {
                    GameServer = null;
                }
                if (ChatServer != null && ChatServer.ServerId == serverPeer.ServerId)
                {
                    ChatServer = null;
                }
            }
        }

        public override void Connect(PhotonServerPeer serverPeer)
        {
            Log.Debug("Server connected");
            if ((serverPeer.ServerType & (int) ServerType.Game) != 0)
            {
                var parameters = new Dictionary<byte, object>();
                Dictionary<string, string> serverList = Servers.Where(
                    incomingSubServerPeer =>
                        incomingSubServerPeer.Value.ServerId.HasValue &&
                        !incomingSubServerPeer.Value.ServerId.Equals(serverPeer.ServerId) &&
                        (incomingSubServerPeer.Value.ServerType & (int) ServerType.Game) != 0)
                    .ToDictionary(incomingSubServerPeer => incomingSubServerPeer.Value.ApplicationName,
                        incomingSubServerPeer => incomingSubServerPeer.Value.TcpAddress);
                if (serverList.Count > 0)
                {
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("Sending list of {0} connected sub servers", serverList.Count);
                    }
                    parameters.Add((byte) ServerParameterCode.SubServerDictionary, serverList);
                    serverPeer.SendEvent(new EventData((byte) ServerEventCode.SubServerList, parameters),
                        new SendParameters());
                }
            }
        }

        public override void ClientConnect(PhotonClientPeer clientPeer)
        {
            var para = new Dictionary<byte, object>
            {
                {(byte) ClientParameterCode.CharacterId, clientPeer.ClientData<UserData>().CharacterId.ToByteArray()},
                {(byte) ClientParameterCode.PeerId, clientPeer.PeerID.ToByteArray()}
            };

            if (ChatServer != null)
            {
                ChatServer.SendEvent(new EventData((byte) ServerEventCode.CharacterRegister, para), new SendParameters());
            }

            if (clientPeer.CurrentServer != null)
            {
                Log.Debug("CurrentServer fount " + clientPeer.PeerID);
                clientPeer.CurrentServer.SendEvent(new EventData((byte) ServerEventCode.CharacterRegister, para),
                    new SendParameters());
            }
        }

        public override void ClientDisconnect(PhotonClientPeer clientPeer)
        {
            var para = new Dictionary<byte, object> {{(byte) ClientParameterCode.PeerId, clientPeer.PeerID.ToByteArray()}};

            if (ChatServer != null)
            {
                ChatServer.SendEvent(new EventData((byte) ServerEventCode.CharacterDeregister, para),
                    new SendParameters());
            }

            if (clientPeer.CurrentServer != null)
            {
                clientPeer.CurrentServer.SendEvent(new EventData((byte) ServerEventCode.CharacterDeregister, para),
                    new SendParameters());
            }

            LoginServer.SendEvent(new EventData((byte) ServerEventCode.UserLoggedOut, para), new SendParameters());
        }

        public override void ResetServers()
        {
            Log.DebugFormat("ResetServers called");
            if (LoginServer != null && LoginServer.ServerType != (int) ServerType.Login)
            {
                PhotonServerPeer peer =
                    Servers.Values.Where(subServerPeer => subServerPeer.ServerType == (int) ServerType.Login)
                        .FirstOrDefault();
                if (peer != null)
                {
                    LoginServer = peer;
                }
            }

            if (ChatServer != null && ChatServer.ServerType != (int) ServerType.Chat)
            {
                PhotonServerPeer peer =
                    Servers.Values.Where(subServerPeer => subServerPeer.ServerType == (int) ServerType.Chat)
                        .FirstOrDefault();
                if (peer != null)
                {
                    ChatServer = peer;
                }
            }

            if (GameServer != null && GameServer.ServerType != (int) ServerType.Game)
            {
                PhotonServerPeer peer =
                    Servers.Values.Where(subServerPeer => subServerPeer.ServerType == (int) ServerType.Game)
                        .FirstOrDefault();
                if (peer != null)
                {
                    GameServer = peer;
                }
            }

            if (LoginServer == null || LoginServer.ServerId == null)
            {
                LoginServer =
                    Servers.Values.Where(subServerPeer => subServerPeer.ServerType == (int) ServerType.Login)
                        .FirstOrDefault() ??
                    Servers.Values.Where(subServerPeer => (subServerPeer.ServerType & (int) ServerType.Login) != 0)
                        .FirstOrDefault();
            }

            if (ChatServer == null || ChatServer.ServerId == null)
            {
                ChatServer =
                    Servers.Values.Where(subServerPeer => subServerPeer.ServerType == (int) ServerType.Chat)
                        .FirstOrDefault() ??
                    Servers.Values.Where(subServerPeer => (subServerPeer.ServerType & (int) ServerType.Chat) != 0)
                        .FirstOrDefault();
            }

            if (GameServer == null || GameServer.ServerId == null)
            {
                GameServer =
                    Servers.Values.Where(subServerPeer => subServerPeer.ServerType == (int) ServerType.Game)
                        .FirstOrDefault() ??
                    Servers.Values.Where(subServerPeer => (subServerPeer.ServerType & (int) ServerType.Game) != 0)
                        .FirstOrDefault();
            }

            if (GameServer != null)
            {
                Log.DebugFormat("Game Server: {0}", GameServer.ConnectionId);
            }

            if (LoginServer != null)
            {
                Log.DebugFormat("Login Server: {0}", LoginServer.ConnectionId);
            }

            if (ChatServer != null)
            {
                Log.DebugFormat("Chat Server: {0}", ChatServer.ConnectionId);
            }
        }

        public override bool IsServerPeer(InitRequest initRequest)
        {
            Log.DebugFormat("Received init request to {0}:{1} - {2}", initRequest.LocalIP, initRequest.LocalPort,
                initRequest);
            if (initRequest.LocalPort == 4520) // TODO do not hardcode
            {
                return true;
            }
            return false;
        }

        public override PhotonServerPeer OnGetServerByType(int serverType)
        {
            PhotonServerPeer server = null;
            switch ((ServerType) Enum.ToObject(typeof (ServerType), serverType))
            {
                case ServerType.Login:
                    if (LoginServer != null)
                    {
                        Log.DebugFormat("Found Login Server");
                        server = LoginServer;
                    }
                    break;
                case ServerType.Chat:
                    if (ChatServer != null)
                    {
                        Log.DebugFormat("Found Chat Server");
                        server = ChatServer;
                    }
                    break;
                case ServerType.Game:
                    if (GameServer != null)
                    {
                        Log.DebugFormat("Found Game Server");
                        server = GameServer;
                    }
                    break;
            }
            return server;
        }

        public override void DisconnectAll()
        {
            foreach (var photonServerPeer in Servers)
            {
                photonServerPeer.Value.Disconnect();
            }
            foreach (var photonClientPeer in Clients)
            {
                photonClientPeer.Value.Disconnect();
            }
        }
    }
}