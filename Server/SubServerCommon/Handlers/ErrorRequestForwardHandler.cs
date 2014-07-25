using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Server;

namespace SubServerCommon.Handlers
{
    public class ErrorRequestForwardHandler : DefaultRequestHandler
    {
        public ErrorRequestForwardHandler(PhotonApplication application)
            : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Request; }
        }

        // TODO limited usability. Fix it
        public override byte Code
        {
            get { return (byte) (ClientOperationCode.Chat | ClientOperationCode.Game | ClientOperationCode.Login); }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            Log.ErrorFormat("No existing Request handler.");
            return true;
        }
    }
}