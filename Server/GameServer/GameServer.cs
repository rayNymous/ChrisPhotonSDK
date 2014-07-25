using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Autofac;
using GameServer.Model;
using GameServer.Model.Interfaces;
using GameServer.Quests;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;
using Photon.SocketServer;
using ProtoBuf.Meta;
using SubServerCommon;
using SubServerCommon.data;
using SubServerCommon.Handlers;
using SubServerCommon.Operations;
using IContainer = Autofac.IContainer;

namespace GameServer
{
    public class GameServer : PhotonApplication
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
            get { return 4532; }
        }

        public override int? UdpPort
        {
            get { return 5057; }
        }

        public override IPAddress PublicIpAddress
        {
            get { return publicIpAddress; }
        }

        public override int ServerType
        {
            get { return (int) SubServerCommon.ServerType.Game; }
        }

        protected override int ConnectRetryIntervalSeconds
        {
            get { return 14; }
        }

        protected override bool ConnectsToMaster
        {
            get { return true; }
        }

        public static async void Schedule(Action action, TimeSpan delay)
        {
            await Task.Delay(delay);
            action();
        }

        protected override void RegisterContainerObjects(ContainerBuilder builder)
        {
            RuntimeTypeModel.Default.Add(typeof (ContainerData), true)[3].SupportNull = true;
            builder.RegisterType<SubServerConnectionCollection>().As<PhotonConnectionCollection>().SingleInstance();
            builder.RegisterInstance(this).As<PhotonApplication>().SingleInstance();
            builder.RegisterType<ErrorEventForwardHandler>().As<DefaultEventHandler>().SingleInstance();
            builder.RegisterType<ErrorRequestForwardHandler>().As<DefaultRequestHandler>().SingleInstance();
            builder.RegisterType<ErrorResponseForwardHandler>().As<DefaultResponseHandler>().SingleInstance();
            builder.RegisterType<SubServerClientPeer>();
            builder.RegisterType<GPlayerInstance>();

            builder.RegisterAssemblyTypes(Assembly.GetAssembly(GetType())).AsImplementedInterfaces();

            builder.RegisterType<GWorld>().As<IWorld>().SingleInstance();

            // Add Quests
            builder.RegisterAssemblyTypes(typeof (Quest).Assembly)
                .Where(t => t.IsSubclassOf(typeof (Quest)))
                .As<Quest>()
                .SingleInstance();

            // Add Factories
            builder.RegisterAssemblyTypes(Assembly.GetAssembly(GetType()))
                .Where(t => t.Name.EndsWith("Factory")).AsSelf()
                .SingleInstance();

            // Add Handlers
            builder.RegisterAssemblyTypes(Assembly.GetAssembly(GetType()))
                .Where(t => t.Name.EndsWith("Handler"))
                .As<PhotonServerHandler>()
                .SingleInstance();
        }

        protected override void ResolveParameters(IContainer container)
        {
            container.Resolve<IWorld>();
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