using System;
using System.Net;
using Autofac;
using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;
using ProxyServer.Handlers;
using SubServerCommon.Data.ClientData;

namespace ProxyServer
{
    public class ProxyServer : PhotonApplication
    {
        public override byte SubCodeParameterKey
        {
            get { return (byte) ClientParameterCode.SubOperationCode; }
        }

        public override IPEndPoint MasterEndPoint
        {
            get { throw new NotImplementedException(); }
        }

        public override int? TcpPort
        {
            get { throw new NotImplementedException(); }
        }

        public override int? UdpPort
        {
            get { throw new NotImplementedException(); }
        }

        public override IPAddress PublicIpAddress
        {
            get { throw new NotImplementedException(); }
        }

        public override int ServerType
        {
            get { throw new NotImplementedException(); }
        }

        protected override int ConnectRetryIntervalSeconds
        {
            get { throw new NotImplementedException(); }
        }

        protected override bool ConnectsToMaster
        {
            get { return false; }
        }

        protected override void RegisterContainerObjects(ContainerBuilder builder)
        {
            builder.RegisterType<ProxyConnectionCollection>().As<PhotonConnectionCollection>().SingleInstance();
            builder.RegisterInstance(this).As<PhotonApplication>().SingleInstance();
            builder.RegisterType<UserData>().As<IClientData>();
            builder.RegisterType<ServerRegistrationHandler>().As<PhotonServerHandler>().SingleInstance();

            builder.RegisterType<EventForwardHandler>().As<DefaultEventHandler>();
            builder.RegisterType<RequestForwardHandler>().As<DefaultRequestHandler>();
            builder.RegisterType<ResponseForwardHandler>().As<DefaultResponseHandler>();

            // Add Handlers
            builder.RegisterType<ClientLoginRequestHandler>().As<PhotonClientHandler>().SingleInstance();
            builder.RegisterType<ClientChatRequestHandler>().As<PhotonClientHandler>().SingleInstance();
            builder.RegisterType<ClientGameRequestHandler>().As<PhotonClientHandler>().SingleInstance();
            builder.RegisterType<LoginResponseHandler>().As<PhotonServerHandler>().SingleInstance();
            builder.RegisterType<SelectCharacterResponseHandler>().As<PhotonServerHandler>().SingleInstance();
            builder.RegisterType<CharacterListResponseHandler>().As<PhotonServerHandler>().SingleInstance();
        }

        protected override void ResolveParameters(IContainer container)
        {
        }

        public override void Register(PhotonServerPeer peer)
        {
        }
    }
}