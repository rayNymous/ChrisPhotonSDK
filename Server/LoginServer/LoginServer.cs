using System.IO;
using System.Net;
using System.Reflection;
using System.Xml.Serialization;
using Autofac;
using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;
using Photon.SocketServer;
using SubServerCommon;
using SubServerCommon.data;
using SubServerCommon.Data.ClientData;
using SubServerCommon.Handlers;
using SubServerCommon.Operations;

namespace LoginServer
{
    public class LoginServer : PhotonApplication
    {
        private readonly IPEndPoint masterEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4520);
        private readonly IPAddress publicIpAddress = IPAddress.Parse("127.0.0.1");

        public override byte SubCodeParameterKey
        {
            get { return (byte) ClientParameterCode.SubOperationCode; }
        }

        public override IPEndPoint MasterEndPoint
        {
            get { return masterEndPoint; }
        }

        public override int? TcpPort
        {
            get { return 4531; }
        }

        public override int? UdpPort
        {
            get { return 5056; }
        }

        public override IPAddress PublicIpAddress
        {
            get { return publicIpAddress; }
        }

        public override int ServerType
        {
            get { return (int) SubServerCommon.ServerType.Login; }
        }

        protected override int ConnectRetryIntervalSeconds
        {
            get { return 14; }
        }

        protected override bool ConnectsToMaster
        {
            get { return true; }
        }

        protected override void RegisterContainerObjects(ContainerBuilder builder)
        {
            builder.RegisterType<SubServerConnectionCollection>().As<PhotonConnectionCollection>().SingleInstance();
            builder.RegisterInstance(this).As<PhotonApplication>().SingleInstance();
            builder.RegisterType<ErrorEventForwardHandler>().As<DefaultEventHandler>().SingleInstance();
            builder.RegisterType<ErrorRequestForwardHandler>().As<DefaultRequestHandler>().SingleInstance();
            builder.RegisterType<ErrorResponseForwardHandler>().As<DefaultResponseHandler>().SingleInstance();
            builder.RegisterType<SubServerClientPeer>();
            builder.RegisterType<UserData>().As<IClientData>();

            // Add Handlers
            builder.RegisterAssemblyTypes(Assembly.GetAssembly(GetType()))
                .Where(t => t.Name.EndsWith("Handler"))
                .As<PhotonServerHandler>()
                .SingleInstance();
        }

        protected override void ResolveParameters(IContainer container)
        {
        }

        public override void Register(PhotonServerPeer peer)
        {
            var registerSubServerOperation = new RegisterSubServerData
            {
                GameServerAddress = PublicIpAddress.ToString(),
                TcpPort = TcpPort,
                UdpPort = UdpPort,
                ServerId = ServerId,
                ServerType = ServerType,
                ApplicationName = ApplicationName
            };

            var mySerializer = new XmlSerializer(typeof (RegisterSubServerData));
            var outString = new StringWriter();
            mySerializer.Serialize(outString, registerSubServerOperation);

            peer.SendOperationRequest(
                new OperationRequest((byte) ServerOperationCode.RegisterSubServer,
                    new RegisterSubServer {RegisterSubServerOperation = outString.ToString()}), new SendParameters());
        }
    }
}