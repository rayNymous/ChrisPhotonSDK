using MayhemCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Server;

namespace GameServer.Handlers
{
    public abstract class GameRequestHandler : PhotonServerHandler
    {
        public GameRequestHandler(PhotonApplication application) : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Request; }
        }

        public override byte Code
        {
            get { return (byte) ClientOperationCode.Game; }
        }
    }
}