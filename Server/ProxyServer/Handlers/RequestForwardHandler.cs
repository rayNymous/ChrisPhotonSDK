using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Server;

namespace ProxyServer.Handlers
{
    public class RequestForwardHandler : DefaultRequestHandler
    {
        public RequestForwardHandler(PhotonApplication application)
            : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Request; }
        }

        public override byte Code
        {
            get { return (byte) (ClientOperationCode.Chat | ClientOperationCode.Login | ClientOperationCode.Game); }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            Log.ErrorFormat("No Existing Request handler");
            return true;
        }
    }
}