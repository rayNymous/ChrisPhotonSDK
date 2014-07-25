using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Server;

namespace SubServerCommon.Handlers
{
    public class ErrorResponseForwardHandler : DefaultResponseHandler
    {
        public ErrorResponseForwardHandler(PhotonApplication application)
            : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Response; }
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
            Log.ErrorFormat("No existing Response handler.");
            return true;
        }
    }
}